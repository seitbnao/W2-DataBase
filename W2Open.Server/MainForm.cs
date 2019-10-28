using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using W2Open.Common;
using W2Open.Common.GameStructure;
 
using W2Open.Common.Utility;
using W2Open.GameState;
using W2Open.GameState.Plugin;
using W2Open.Server.ProcessSecTimer;

namespace W2Open.Server
{
#pragma warning disable 168
    public partial class MainForm : Form
    {
        private DBController gameController;
        public static ConfigServer Config;
        public MainForm()
        {
            InitializeComponent();

            PluginController.InstallPlugins();
            W2Log.DidLog += CLog_DidLog;
            MainSecTimer.MainTask();
           

            gameController = new DBController(this);

            gameController.ReadBaseMob(gameController);
            ReadConfigFile();
  
            gameController.MySQL = new MYSQL(Config.MYSQL_Server, Config.MYSQL_DataBase, Config.MYSQL_User, Config.MYSQL_Pass);
            

            label2.Text = Config.IPAddrs;
            label3.Text = Config.Port.ToString();
            MysqlStatus.Text = gameController.MySQL.bConnected == true ? "ON" : "OFF";
         
            MysqlStatus.ForeColor = gameController.MySQL.bConnected == true ? Color.Green : Color.Red;
            
            int Guilds = 0;
            for(int i = 0; i < BaseDef.MAX_GUILD; i++)
            {
                gameController.g_pGuildInfo[i] = Functions.ReadGuildInfo(i);
                if (!String.IsNullOrEmpty(gameController.g_pGuildInfo[i].GuildName))
                    Guilds++;
            }

             

            label5.Text = Guilds.ToString();
            StartServer_Channel1();

            //W2Log.Show($"STRUCT_MOB: {Marshal.SizeOf(typeof(STRUCT_MOB))}");
             
        }
        public static void ReadConfigFile()
        {
            if (!File.Exists("config.json"))
            {
                 Config = new ConfigServer();


                using (StreamWriter file = File.CreateText("config.json"))
                {
                    string indented = JsonConvert.SerializeObject(Config, Formatting.Indented);
                    file.Write(indented);
                }
            }
            using (StreamReader r = new StreamReader("config.json"))
            {
                string json = r.ReadToEnd();
                Config = JsonConvert.DeserializeObject<ConfigServer>(json);
            }
        }
        public void saveConfig()
        {
            try
            {
                using (StreamWriter file = File.CreateText("config.json"))
                {
                    string indented = JsonConvert.SerializeObject(Config, Formatting.Indented);
                    file.Write(indented);
                }
            }
            catch(Exception e)
            {
                W2Log.Write("Erro ao salvar config.json");
                return;
            }
        }
        public void CLog_DidLog(String txt, ELogType logType)
        {
            try
            {


                Color logColor = Color.Yellow;
                string Ename = "UNKNOW";
                switch (logType)
                {
                    case ELogType.SHOW_ONLY:
                    case ELogType.CRITICAL_ERROR:
                        logColor = Color.Red;
                        Ename = "CRITICAL_ERROR";
                        break;

                    case ELogType.GAME_EVENT:
                        logColor = Color.Cyan;
                        Ename = "GAME_EVENT";
                        break;

                    case ELogType.NETWORK:
                        logColor = Color.Azure;
                        Ename = "NETWORK";
                        break;

                    case ELogType.UPDATE_EVENT:
                        {

                            Config.ChargedGuildList = gameController.ChargedGuildList;

                            Armia.Text = gameController.ChargedGuildList[1,0].ToString();
                            label25.Text = gameController.g_pGuildInfo[gameController.ChargedGuildList[1, 0]].GuildName;

                            Arzram.Text = gameController.ChargedGuildList[1, 1].ToString();
                            label13.Text = gameController.g_pGuildInfo[gameController.ChargedGuildList[1, 1]].GuildName;

                            Erion.Text = gameController.ChargedGuildList[1, 2].ToString();
                            label10.Text = gameController.g_pGuildInfo[gameController.ChargedGuildList[1, 2]].GuildName;

                            Noatun.Text = gameController.ChargedGuildList[1, 3].ToString();
                            label21.Text = gameController.g_pGuildInfo[gameController.ChargedGuildList[1, 3]].GuildName;

                            Niflheim.Text = gameController.ChargedGuildList[1, 4].ToString();
                            label17.Text = gameController.g_pGuildInfo[gameController.ChargedGuildList[1, 4]].GuildName;

                            lbDrop.Text = gameController.Config.ItemCount.ToString();



                            saveConfig();
                         
                            return;
                        }


                    default:
                        logColor = Color.GreenYellow;
                        break;
                }

                if (logType == ELogType.SHOW_ONLY)
                    rtbMainLog.Clear();

                rtbMainLog.SelectionColor = Color.Yellow;
                rtbMainLog.AppendText(String.Format("[{0:D02}:{1:D02}:{2:D02}] ", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));

                rtbMainLog.SelectionColor = logColor;
                rtbMainLog.AppendText(txt + "\n");

                rtbMainLog.Focus();
                if (logType == ELogType.SHOW_ONLY)
                    return;

                String path = $"./DataBase/Logs/{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}.{Ename}";
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(txt);
                    }
                }
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(txt);
                }
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Process the newly connected client.
        /// </summary>
        /// <param name="client">The newly connected client.</param>
        private async void ProcessClient_Channel1(TcpClient client)
        {
            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                pServer GameServer = new pServer(stream);
                CCompoundBuffer packet = GameServer.RecvPacket;

                try
                {
                    // Iterate processing incoming GameServer packets until he disconnects.
                    while (GameServer.State != EServerStatus.CLOSED)
                    {
                        int readCount = 0;
                        
                        try
                        {
                             readCount = await stream.ReadAsync(packet.RawBuffer, 0, BaseDef.MAXL_PACKET);
                        }
                        catch(Exception e)
                        {
                            break;
                        }

                        if (readCount != 4 && (readCount < 12 || readCount > BaseDef.MAXL_PACKET)) // Invalid game packet.
                        {
                           // gameController.DisconnectPlayer(GameServer);
                            break;
                        }
                        else // Possible valid game packet chunk.
                        {
                            unsafe
                            {
                                packet.Offset = 0;
                                fixed (byte* PinnedPacketChunk = &packet.RawBuffer[packet.Offset])
                                {
                                    // Check for the init code.
                                    if (*(uint*)&PinnedPacketChunk[packet.Offset] == BaseDef.INIT_CODE)
                                    {
                                        packet.Offset = 4;

                                        // If a valid index can't be assigned to the GameServer, disconnect him 
                                        if (!gameController.TryInsertPlayer(GameServer))
                                        {
                                            GameServer.SendPacket(MTextMessagePacket.Create("O servidor está lotado. Tente novamente mais tarde."));
                                           // gameController.DisconnectPlayer(GameServer);
                                            continue;
                                        }

                                        // If all the received chunk resumes to the INIT_CODE, read the next packet.
                                        if (readCount == 4)
                                            continue;
                                    }

                                    // Process all possible packets that were possibly sent together.
                                    while (packet.Offset < readCount && GameServer.State != EServerStatus.CLOSED)
                                    {



                                        // Check if the game packet size is bigger than the remaining received chunk.
                                        if (packet.ReadNextUShort(0) > readCount - packet.Offset || packet.ReadNextUShort(0) < 12)
                                        {
                                            throw new Exception("Pacote recebido inválido.O pacote de leitura é maior que o restante do pacote.");
                                            //continue;
                                        }

                                        // Tries to decrypt the packet.
                                        if (!PacketSecurity.Decrypt(packet))
                                            throw new Exception($"Não é possível descriptografar um pacote recebido de {client.Client.RemoteEndPoint}.");

                                       // W2Log.Write(String.Format("Em processamento recv packet {{0x{0:X}/{1}}} a partir de {2}.", packet.ReadNextUShort(4), packet.ReadNextUShort(0), client.Client.RemoteEndPoint), ELogType.NETWORK);

                                        // Process the incoming packet.
                                        DBResult requestResult = gameController.ProcessPlayerRequest(GameServer);
                                     
                                        // Treat the processing packet return.
                                        switch (requestResult)
                                        {
                                            //case DBResult.PACKET_NOT_HANDLED:
                                            //{
                                            //    W2Log.Write(String.Format("Recv packet {{0x{0:X}/{1}}} de {2} não foi processado.",
                                            //        packet.ReadNextUShort(4), packet.ReadNextUShort(0), client.Client.RemoteEndPoint), ELogType.NETWORK);

                                            //    byte[] rawPacket = new byte[packet.ReadNextUShort(0)];
                                            //    for (int i = 0; i < rawPacket.Length; i++)
                                            //        rawPacket[i] = PinnedPacketChunk[i + packet.Offset];

                                            //    File.WriteAllBytes($@"..\..\{packet.ReadNextUShort(4)}.bin",
                                            //        rawPacket);
                                            //    break;
                                            //}

                                            case DBResult.CHECKSUM_FAIL:
                                            {
                                                W2Log.Write($"Recibo de pacote de { client.Client.RemoteEndPoint} tem soma de verificação inválida.", ELogType.CRITICAL_ERROR);
                                                //gameController.DisconnectPlayer(GameServer);
                                                break;
                                            }

                                            case DBResult.PLAYER_INCONSISTENT_STATE:
                                            {
                                                W2Log.Write($"Um GameServer foi desconectado devido ao DBResult inconsistente.", ELogType.CRITICAL_ERROR);
                                                //gameController.DisconnectPlayer(GameServer);
                                                break;
                                            }

                                            case DBResult.WAIT:
                                                {
                                                    break;
                                                }
                                        }

                                        // Correct the offset to process the next packet in the received chunk.
                                        PlayersCount.Text = gameController.PlayerCount.ToString();
                                        GameServer.RecvPacket.Offset += packet.ReadNextUShort(0);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    W2Log.Write($"Uma exceção não tratada aconteceu processando o GameServer {GameServer.Index}. MSG: {ex.Message}", ELogType.CRITICAL_ERROR);
                }
                finally
                {
                   // gameController.DisconnectPlayer(GameServer);
                }
            }

        }

        /// <summary>
        /// Start listenning the server connection.
        /// </summary>
        private async void StartServer_Channel1()
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(Config.IPAddrs),Config.Port);

            listener.Start();

            W2Log.Write($"DBserver iniciado com IP: {listener.Server.LocalEndPoint}.", ELogType.NETWORK);

            try
            {
                while(true)
                {
                    TcpClient thisClient = await listener.AcceptTcpClientAsync();

                    W2Log.Write($"Nova gameserver conectada: {thisClient.Client.RemoteEndPoint}.", ELogType.NETWORK);

                    ProcessClient_Channel1(thisClient);



                }
            }
            finally
            {
                listener.Stop();
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void clsLog_Click(object sender, EventArgs e)
        {
            int count = 0;
            foreach (string file in Directory.EnumerateFiles("./DataBase/Logs/"))
            {
                if (!String.IsNullOrEmpty(file))
                { File.Delete(file); count++; }
            }
            W2Log.Show($"[{count}] Logs limpos com sucesso!");
        }
    }
}
