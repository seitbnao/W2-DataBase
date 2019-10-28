using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using W2Open.Common.GameStructure;

namespace W2Open.Common.Utility
{
    //"Server=localhost;Database=w2pp;Uid=root;Pwd=;"
    public class ConfigServer
    {
        public string ServerName;
        public string IPAddrs;
        public int Port;
        public string MYSQL_Server;
        public string MYSQL_DataBase;
        public string MYSQL_User;
        public string MYSQL_Pass;
        public int[,] ChargedGuildList;
        public int Sapphire;
        public int LastCapsule;
        public int ItemCount;


        public ConfigServer()
        {
            ServerName = "WYD";
            IPAddrs = "25.19.43.5";
            Port = 7514;
            MYSQL_Server = "localhost";
            MYSQL_DataBase = "w2pp";
            MYSQL_User = "root";
            MYSQL_Pass = "";
            ChargedGuildList = new int[BaseDef.MAX_CHANNEL, BaseDef.MAX_GUILDZONE];
            Sapphire = 8;
            LastCapsule = 1;
            ItemCount = 0;
        }

        public static void ReadConfigFile(ConfigServer Config)
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





        public static void saveConfig(ConfigServer Config)
        {
            try
            {
                using (StreamWriter file = File.CreateText("config.json"))
                {
                    string indented = JsonConvert.SerializeObject(Config, Formatting.Indented);
                    file.Write(indented);
                }
            }
            catch (Exception e)
            {
                W2Log.Write("Erro ao salvar config.json " + e.Message);
                return;
            }
        }















    }
}
