using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using W2Open.Common.GameStructure;
 

namespace W2Open.Common.Utility
{

    public static class Functions
    {
   
        public static T LoadFile<T>(byte[] rawData) where T : struct
        {
            var pinnedRawData = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            try
            {
                var pinnedRawDataPtr = pinnedRawData.AddrOfPinnedObject();
                return (T)Marshal.PtrToStructure(pinnedRawDataPtr, typeof(T));
            }
            finally
            {
                pinnedRawData.Free();
            }
        }
        public static byte[] FileToByteArray(string fileName)
        {
            byte[] fileData = null;

            using (FileStream fs = File.OpenRead(fileName))
            {
                using (BinaryReader binaryReader = new BinaryReader(fs))
                {
                    fileData = binaryReader.ReadBytes((int)fs.Length);
                }
            }
            return fileData;
        }
        public static T ReadAccount<T>(string FilePatch) where T : struct
        {
            return LoadFile<T>(FileToByteArray(FilePatch));
        }

        public static T ReadAccount<T>(string FilePatch,bool correctPatch) where T : struct
        {
            if (correctPatch)
            {
                return LoadFile<T>(FileToByteArray(getCorrectPath(FilePatch)));
            }

            return LoadFile<T>(FileToByteArray(FilePatch));
        }
        public static void WritePacket<T>(T Packet) where T : struct
        {
            MSG_HEADER header = W2Marshal.GetStructure<MSG_HEADER>(W2Marshal.GetBytes(Packet));
            try
            {
                string CorrectPatch = $"./PacketDebug/{ header.PacketID} .json";
                if (File.Exists(CorrectPatch))
                    return;

                using (StreamWriter file = File.CreateText(CorrectPatch))
                {
                    string indented = JsonConvert.SerializeObject(Packet, Formatting.Indented);
                    file.Write(indented);
                }
            }
            catch (Exception e)
            {
                return;
            }
            return;
        }

        public static string getCorrectPath(string AccoutName)
        {
            string BaseDir = AccoutName.Substring(0, 1).ToUpper();
            if (!isAlphaNumeric(BaseDir))
            {
                BaseDir = "etc";
            }
            return String.Format("{0}/{1}/{2}", "./DataBase/account/", BaseDir, AccoutName);
        }
        public static string getCorrectCharPath(string AccoutName)
        {
            string BaseDir = AccoutName.Substring(0, 1).ToUpper();
            if (!isAlphaNumeric(BaseDir))
            {
                BaseDir = "etc";
            }
            return String.Format("{0}/{1}/{2}", "./DataBase/char/", BaseDir, AccoutName);
        }


        public static bool isAlphaNumeric(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9\s,]*$");
            return rg.IsMatch(strToCheck);
        }



        public static STRUCT_GUILDINFO ReadGuildInfo(int Index)
        {
            STRUCT_GUILDINFO Guild = STRUCT_GUILDINFO.Empty();
            try
            {
                string CorrectPatch = "./Database/Guilds/" + Index + ".json";
                if (!File.Exists(CorrectPatch))
                    return Guild;
                using (StreamReader r = new StreamReader(CorrectPatch))
                {
                    string json = r.ReadToEnd();
                    Guild = JsonConvert.DeserializeObject<STRUCT_GUILDINFO>(json);
                }
              //  W2Log.Write(String.Format("read Guild sucess: {0}", Index), ELogType.GAME_EVENT);
            }
            catch (Exception e)
            {
                W2Log.Write(String.Format("read Guild fail {0}", e.Message), ELogType.GAME_EVENT);
                return STRUCT_GUILDINFO.Empty();
            }
            return Guild;
        }
        public static bool WriteGuildInfo(STRUCT_GUILDINFO[] Guild)
        {
            try
            {
                int Counter = 0;
                for (int i = 1; i < 5000; i++)
                {
                    if (String.IsNullOrEmpty(Guild[i].GuildName))
                        continue;

                    string CorrectPatch = "./DataBase/Guilds/" + i + ".json";
                    using (StreamWriter file = File.CreateText(CorrectPatch))
                    {
                        string indented = JsonConvert.SerializeObject(Guild[i], Formatting.Indented);
                        file.Write(indented);
                        file.Close();
                        Counter++;
                    }
                }
                if (Counter > 0)
                    W2Log.Write("save guild sucess", ELogType.GAME_EVENT);

                return true;
            }
            catch (Exception e)
            {
               // W2Log.Write(String.Format("save guild fail {0}", e.Message), ELogType.CRITICAL_ERROR);
                return false;
            }
        }

        public static STRUCT_GUILDINFO[] ReadGuildInfo()
        {
            STRUCT_GUILDINFO[] Guild = new STRUCT_GUILDINFO[5000];
            try
            {
                for (int i = 0; i < 5000; i++)
                {
                    Guild[i] = STRUCT_GUILDINFO.Empty();

                    string CorrectPatch = "./Guilds/" + i + ".json";
                    using (StreamReader r = new StreamReader(CorrectPatch))
                    {
                        string json = r.ReadToEnd();
                        Guild[i] = JsonConvert.DeserializeObject<STRUCT_GUILDINFO>(json);
                    }

                    //W2Log.Write(String.Format("read guild sucess: {0}/{1}", i, Guild[i].GuildName), ELogType.GAME_EVENT);
                }
            }
            catch (Exception e)
            {
                W2Log.Write(String.Format("read guild fail {0}", e.Message), ELogType.GAME_EVENT);
                return null;
            }
            return Guild;
        }







        public static bool ReadCapsule(int Index,out STRUCT_CAPSULE Seal)
        {
            Seal = STRUCT_CAPSULE.Empty();
            try
            {
                string CorrectPatch = "./DataBase/Selo_da_Alma/" + Index + ".json";
                if (!File.Exists(CorrectPatch))
                    return false;
                using (StreamReader r = new StreamReader(CorrectPatch))
                {
                    string json = r.ReadToEnd();
                    Seal = JsonConvert.DeserializeObject<STRUCT_CAPSULE>(json);
                }
                W2Log.Write(String.Format("read seal sucess: {0}", Index), ELogType.GAME_EVENT);
            }
            catch (Exception e)
            {
                W2Log.Write(String.Format("read seal fail {0}", e.Message), ELogType.GAME_EVENT);
                return false;
            }
            return true;
        }

        public static byte GetClassType(STRUCT_MOB Mob)
        {
            int chck = Mob.LearnedSkill & 0x40000000;
            int Face = Mob.Equip.Items[0].Index % 10;
            if (Face <= 5)//Mortal
                return 0;
            else if (chck != 0) //Cele
                return 2;
            else
                return 1; //Arch
          
        }

        
public static void DBGetSelChar(   MSelChar SelChar,STRUCT_ACCOUNTFILE File)
        {
             
            for (int i = 0; i < GameBasics.MAXL_ACC_MOB; i++)
            {
                unsafe
                {
                    byte evoType = GetClassType(File.Mob[i]);
                    SelChar.Coin[i] = File.Mob[i].Coin;
                    for (int x = 0; x < GameBasics.MAXL_EQUIP; x++)
                    {
                        if (x == 0)
                        {
                            int Face = SelChar.Equip[i].Items[x].Index;
                            if (Face == 22 || Face == 23 || Face == 24 || Face == 25 || Face == 32)
                                Face = File.MobExtra[i].ClassMaster == 0 ? 21 : (ushort)File.MobExtra[i].MortalFace + 7;

                            SelChar.Equip[i].Items[x].Index = (short)Face;
                            SelChar.Equip[i].Items[x].Effects[0].Code = 43;
                            SelChar.Equip[i].Items[x].Effects[0].Value = 0;
                            SelChar.Equip[i].Items[x].Effects[1].Code = 86;
                            SelChar.Equip[i].Items[x].Effects[1].Value = evoType;
                            SelChar.Equip[i].Items[x].Effects[2].Code = 28;
                            SelChar.Equip[i].Items[x].Effects[2].Value = (byte)File.MobExtra[i].Citizen;
                        }
                        else
                        {
                            SelChar.Equip[i].Items[x] = File.Mob[i].Equip.Items[x];
                        }
                    }

                    SelChar.Exp[i] = (int)File.Mob[i].Exp;
                    SelChar.Guild[i] = File.Mob[i].Guild;
                    SelChar.Name[i].Value = File.Mob[i].Name;
                    SelChar.Score[i] = File.Mob[i].BaseScore;
                    SelChar.SPosX[i] = File.Mob[i].SPX;
                    SelChar.SPosY[i] = File.Mob[i].SPY;
                }
            }
          
        }

        public static bool WriteCapsule(int Index, STRUCT_CAPSULE Seal)
        {
            try
            {
                string CorrectPatch = "./DataBase/Selo_da_Alma/" + Index + ".json";
                if (File.Exists(CorrectPatch))
                    return false;


                using (StreamWriter file = File.CreateText(CorrectPatch))
                {
                    string indented = JsonConvert.SerializeObject(Seal, Formatting.Indented);
                    file.Write(indented);
                }

                W2Log.Write(String.Format("read seal sucess: {0}", Index), ELogType.GAME_EVENT);
            }
            catch (Exception e)
            {
                W2Log.Write(String.Format("read seal fail {0}", e.Message), ELogType.GAME_EVENT);
                return false;
            }
            return true;
        }


        public static string geCode(int tamanho = 6)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, tamanho)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
        public static bool CreateImportItem(string Name, MItem Item, int idPedido,bool capsule = false)
        {
            try
            {
                string CorrectPatch = "./DataBase/ImportInfo/" + Name + geCode() + ".item";

                //*%d %s %d %d %d %d %d %d %d  ", &id_pedido, ids, &Index, &Eff1, &Val1, &Eff2, &Val2, &Eff3, &Val3);
                string Line = String.Format("*{0} {1} {2} {3} {4} {5} {6} {7} {8}  ",
                    idPedido, Name,Item.Index, Item.Effects[0].Code, Item.Effects[0].Value,
                    Item.Effects[1].Code, Item.Effects[1].Value,
                    Item.Effects[2].Code, Item.Effects[2].Value
                    );

                if (capsule)
                {
                    int ids = 344359;
                    Line = String.Format("*{0} {1} {2} {3} {4} {5} {6} {7} {8}", ids,Name, Item.Index, Item.Effects[0].Code, Item.Effects[0].Value, Item.Effects[1].Code, Item.Effects[1].Value,  Item.Effects[2].Code, Item.Effects[2].Value );
                }

                using (StreamWriter file = File.CreateText(CorrectPatch))
                {
                    file.Write(Line);
                    file.Close();
                }

                W2Log.Write(String.Format("create import item: {0}", Line), ELogType.GAME_EVENT);
            }
            catch (Exception e)
            {
                W2Log.Write(String.Format("can't create importitem: {0}", e.Message), ELogType.GAME_EVENT);
                return false;
            }
            return true;
        }





        public static bool ProcessRecord(int PlayerCount,string Rec)
        {
            try
            {
                CultureInfo Culture = new CultureInfo("pt-BR");
                DateTime localDate = DateTime.Now;
                string atual = localDate.ToString(Culture).Replace('/', '-').ToString();

                string Line = String.Format("{0} {1}", atual,PlayerCount);
                using (StreamWriter file = File.CreateText("./DBRecord.txt"))
                {
                    file.Write(Line);
                    file.Write(Rec);
                    file.Close();
                }
                
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }


       
       static public unsafe bool CreateEmptyAccount()
        {
            STRUCT_ACCOUNTFILE acc = STRUCT_ACCOUNTFILE.ClearProperty();
            try
            {
                acc.Info.AccountName = "ADMIN";
                acc.Info.AccountPass = "admin";
                acc.Info.Address = "127.0.0.1";
                acc.Info.Email = "admin@server.com";
                acc.Info.Telephone = "(00) 0000-00000";
                byte[] data = new byte[Marshal.SizeOf(acc)];

                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(acc));
                Marshal.StructureToPtr(acc, ptr, false);
                Marshal.Copy(ptr, data, 0, Marshal.SizeOf(acc));
                Marshal.FreeHGlobal(ptr);

                File.WriteAllBytes(String.Format(@"./DataBase/Account/{0}/{1}", acc.Info.AccountName.Substring(0, 1).ToUpper(), acc.Info.AccountName.ToUpper()), data);

                using (StreamWriter file = File.CreateText(String.Format(@"./DataBase/Account/{0}/{1}" + ".json", acc.Info.AccountName.Substring(0, 1).ToUpper(), acc.Info.AccountName.ToUpper())))
                {
                    string indented = JsonConvert.SerializeObject(acc, Formatting.Indented);
                    file.Write(indented);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        static public unsafe bool CreateEmptyAccount(string Name,string Pass,string IP,string Email,string Nomepesoal = " ",string Tel = " ")
        {
            STRUCT_ACCOUNTFILE acc = STRUCT_ACCOUNTFILE.ClearProperty();
            try
            {
                acc.Info.AccountName = Name;
                acc.Info.AccountPass = Pass;
                acc.Info.Address = IP;
                acc.Info.Email = Email;
                acc.Info.Telephone = Tel;
                acc.Info.RealName = Nomepesoal;


                byte[] data = new byte[Marshal.SizeOf(acc)];

                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(acc));
                Marshal.StructureToPtr(acc, ptr, false);
                Marshal.Copy(ptr, data, 0, Marshal.SizeOf(acc));
                Marshal.FreeHGlobal(ptr);

                File.WriteAllBytes(String.Format(@"./DataBase/Account/{0}/{1}", acc.Info.AccountName.Substring(0, 1).ToUpper(), acc.Info.AccountName.ToUpper()), data);

                using (StreamWriter file = File.CreateText(String.Format(@"./DataBase/Account/{0}/{1}" + ".json", acc.Info.AccountName.Substring(0, 1).ToUpper(), acc.Info.AccountName.ToUpper())))
                {
                    string indented = JsonConvert.SerializeObject(acc, Formatting.Indented);
                    file.Write(indented);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }







    }
}