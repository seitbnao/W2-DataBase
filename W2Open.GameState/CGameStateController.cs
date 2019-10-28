using System;
using System.ComponentModel;
using System.Timers;
using W2Open.Common.Utility;
using System.IO;
using Newtonsoft.Json;
using W2Open.Common.GameStructure;
using System.Threading.Tasks;
using W2Open.GameState.ProcessSecTimer;
using W2Open.Common;

namespace W2Open.GameState
{
#pragma warning disable 4014
    public class DBController
    {
        /*
         * TODO: nesta classe devem ficar todos os objetos que representam o canal da "tmsrv".
         * Para cada canal, o caller deve instanciar 1 objeto deste.
         * 
         * Criar coisas como: MobGridMap, SpawnedMobs, etc.
         */

        public pServer Server { get; set; }
        public STRUCT_PUSUER[] AccountList { get; set; }
        public STRUCT_ITEMLOG[] ItemDayLog { get; set; }
       


        public ConfigServer Config;

        public MYSQL MySQL;
        public STRUCT_MOB[] BaseMob;
        public int[,] ChargedGuildList;
        public int PlayerCount;
        public short[] g_pGuildWar;
        public short[] g_pGuildAlly;
        public STRUCT_GUILDINFO[] g_pGuildInfo;
         
       
        public readonly DateTime SinceInit;

        public DBController(ISynchronizeInvoke syncObj)
        {
            Server = new pServer();
            BaseMob = new STRUCT_MOB[4];
            AccountList = new STRUCT_PUSUER[BaseDef.MAX_MAXUSER];
            ItemDayLog = new STRUCT_ITEMLOG[BaseDef.MAX_ITEMLIST];
            ChargedGuildList = new int[BaseDef.MAX_CHANNEL, BaseDef.MAX_GUILDZONE];
            SinceInit = DateTime.Now;
          
            PlayerCount = 0;
            MySQL = new MYSQL();
            g_pGuildWar = new short[BaseDef.MAX_GUILD];
            g_pGuildAlly = new short[BaseDef.MAX_GUILD];
            g_pGuildInfo = new STRUCT_GUILDINFO[BaseDef.MAX_GUILD];
            Config = new ConfigServer();
            onTask();
       
        }
       
        public bool AddAccountList(int Idx)
        {
            
            if (AccountList[Idx].State != EServerStatus.CLOSED)
            {
                W2Log.Write(String.Format("err,addAccountlist - already added {0}/{1}", AccountList[Idx].Account.Info.AccountName, AccountList[Idx].State), ELogType.GAME_EVENT);
                return false;
            }

            int conn = Idx / BaseDef.MAX_MAXUSER;

            AccountList[Idx].State = EServerStatus.WAITING_TO_LOGIN;
            AccountList[Idx].Slot = -1;
            PlayerCount++;
            return true;
        }


        public void SendGuildInfo(int Guild)
        {
            MSG_GuildInfo sm = W2Marshal.CreatePacket<MSG_GuildInfo>(BaseDef._MSG_GuildInfo);

            if (Guild < 1 || Guild >= 5000)
                return;

            sm.Guild = Guild;
            sm.GuildInfo = this.g_pGuildInfo[Guild];
            this.Server.SendPacket(sm);
        }

        public  void RemoveAccountList(int Idx)
        {
            if (AccountList[Idx].State == EServerStatus.CLOSED)
                return;

            int conn = Idx / BaseDef.MAX_MAXUSER;

            AccountList[Idx] = new STRUCT_PUSUER(EServerStatus.CLOSED);
            PlayerCount--;
        }


        public int GetIndex(int id)
        {
            int ret = 0 * BaseDef.MAX_MAXUSER + id;

            return ret;
        }

        public int GetIndex(string account)
        {
            int i;

            for (i = 0; i < BaseDef.MAX_MAXUSER; i++)
            {
                if (AccountList[i].State == EServerStatus.CLOSED)
                    continue;


                if (String.Compare(AccountList[i].Account.Info.AccountName, account) == 0)
                    return i;
            }

            return 0;
        }
        public int GetIndex_OL2(string account)
        {
            int i;

            for (i = 0; i < BaseDef.MAX_MAXUSER; i++)
            {
                if (AccountList[i].State == EServerStatus.CLOSED)
                    continue;

                if (String.Compare(AccountList[i].Account.Info.AccountName, account) == 0)
                    return i;
            }

            return 0;
        }


        public bool SaveAccount(ushort conn)
        {
            int Idx = GetIndex(conn);
            try
            {
                string CorrectPatch = Functions.getCorrectPath(AccountList[Idx].Account.Info.AccountName);
                string indented = "";
                using (StreamWriter file = File.CreateText(CorrectPatch + ".json"))
                {
                    indented = JsonConvert.SerializeObject(AccountList[Idx].Account, Formatting.Indented);
                    file.Write(indented);
                }

                W2Log.Write(String.Format("save account sucess: {0}/{1}", Idx, AccountList[Idx].Account.Info.AccountName), ELogType.GAME_EVENT);


                indented.Replace(AccountList[Idx].Account.Info.AccountPass, "null");
                int update = MySQL.nQuery(string.Format("INSERT INTO `accounts_json` (`login`, `conteudo`) VALUES ('{0}', '{1}') ON DUPLICATE KEY UPDATE `login` = '{2}' ", AccountList[Idx].Account.Info.AccountName.ToLower(), indented, AccountList[Idx].Account.Info.AccountName.ToLower()));
                if (update != 0)
                    W2Log.Write(string.Format("Sucess DBSaveAccount update json: {0} - {1}", Idx, AccountList[Idx].Account.Info.AccountName));


                return true;
            }
            catch (Exception e)
            {
                W2Log.Write(String.Format("save account fail: {0}/{1}", AccountList[Idx].Account.Info.AccountName, e.Message), ELogType.GAME_EVENT);
                return false;
            }
        }


        public bool SaveAccount(STRUCT_ACCOUNTFILE accfile)
        {
             
            try
            {
                string CorrectPatch = Functions.getCorrectPath(accfile.Info.AccountName);
                using (StreamWriter file = File.CreateText(CorrectPatch + ".json"))
                {
                    string indented = JsonConvert.SerializeObject(accfile, Formatting.Indented);
                    file.Write(indented);
                }

                W2Log.Write(String.Format("save account sucess: {0}", accfile.Info.AccountName), ELogType.GAME_EVENT);
                return true;
            }
            catch (Exception e)
            {
                W2Log.Write(String.Format("save account fail: {0}/{1}", accfile.Info.AccountName, e.Message), ELogType.GAME_EVENT);
                return false;
            }
        }

        public bool ReadAccount(string Accountname, out STRUCT_ACCOUNTFILE CurrentAccount)
        {
            CurrentAccount = new STRUCT_ACCOUNTFILE();
            try
            {
                string correctPatch = Functions.getCorrectPath(Accountname);
                using (StreamReader r = new StreamReader(correctPatch + ".json"))
                {
                    string json = r.ReadToEnd();
                    CurrentAccount = JsonConvert.DeserializeObject<STRUCT_ACCOUNTFILE>(json);
                }
                return true;
            }
            catch (Exception e)
            {
                W2Log.Write(e.Message);
                return false;
            }
        }
        public bool CreateChar(string CharName,string AccountName)
        {
            try
            {
                string path = Functions.getCorrectCharPath(CharName);
                if (File.Exists(path))
                    return false;
                else
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(AccountName.ToLower());
                    }
                    return true;
                }
            }
            catch(Exception  e)
            {
                W2Log.Write(e.Message);
                return false;
            }
        }
      

        /// <summary>
        /// Insert the GameServer in the game state. This method must be called when the GameServer just stablishes a connection by sending the INIT_CODE to the server.
        /// </summary>
        public bool TryInsertPlayer(pServer GameServer)
        {
            GameServer.Index = 1;
            Server = GameServer;
            return true;
        }
 

        public void DisconnectPlayer(pServer GameServer)
        {
            W2Log.Write($"O GameServer {GameServer.Index} foi desconectado do servidor.", ELogType.GAME_EVENT);
        }


        /// <summary>
        /// Process the GameServer requests.
        /// This method fires the <see cref="OnProcessPacket"/> event to be hooked up by plugins.
        /// </summary>
        public DBResult ProcessPlayerRequest(pServer GameServer)
        {
            DBResult result = DBResult.NO_ERROR;

            foreach (DProcessPacket target in OnProcessPacket.GetInvocationList())
            {
                result = target(this, GameServer);

                if (result != DBResult.NO_ERROR && result != DBResult.PACKET_NOT_HANDLED && result != DBResult.WAIT)
                {
                    W2Log.Write("CRITICAL ERROR", ELogType.CRITICAL_ERROR);
                    break;
                }
            }
            return result;
        }
       


        public void ReadBaseMob(DBController gs)
        {
            try
            {
                string[] MobName = { "TransKnight", "Foema", "BeastMaster", "Huntress" };
                for (int i = 0; i < MobName.Length; i++)
                {
                    string BaseDir = "./DataBase/BaseMob/" + MobName[i];
                    if (!File.Exists(BaseDir + ".json"))
                    {

                        gs.BaseMob[i] = Functions.ReadAccount<STRUCT_MOB>(BaseDir);
                        using (StreamWriter file = File.CreateText(BaseDir + ".json"))
                        {
                            string indented = JsonConvert.SerializeObject(gs.BaseMob[i], Formatting.Indented);
                            file.Write(indented);
                        }
                    }
                    using (StreamReader r = new StreamReader(BaseDir + ".json"))
                    {
                        string json = r.ReadToEnd();
                        gs.BaseMob[i] = JsonConvert.DeserializeObject<STRUCT_MOB>(json);
                    }
                }
            }
            catch (Exception e)
            {
                W2Log.Write("can't read BaseMobs" + e.Message);
            }
        }









        #region Static fields
        public static event DProcessPacket OnProcessPacket;
        #endregion
        public delegate DBResult DProcessPacket(DBController gs, pServer GameServer);

 
        private async Task onTask()
        {
            while (true)
            {
                SecTimer.Start(this, this.Server);
                await Task.Delay(1000);
            }
        }

    }
}