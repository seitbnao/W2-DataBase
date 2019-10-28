using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using W2Open.Common.GameStructure;
using W2Open.Common.Utility;

namespace W2Open.GameState.ProcessSecTimer
{
    public class ProcessUpdateUser
    {

        static public bool UpdateUser(DBController gs, string Login, string Senha)
        {

            STRUCT_ACCOUNTFILE target = new STRUCT_ACCOUNTFILE();

            if (!gs.ReadAccount(Login, out target))
            {
                W2Log.Write(String.Format("fail to read account file {0}", Login), ELogType.GAME_EVENT);
                return false;
            }

            if (0 != String.Compare(target.Info.AccountName, Login))
            {
                W2Log.Write(String.Format("fail to read account file {0}/{1}", Login, target.Info.AccountName), ELogType.GAME_EVENT);
                return false;
            }
           // W2Log.Write(String.Format("pass teste file {0}/{1}", target.Info.AccountPass, Senha), ELogType.GAME_EVENT);
            target.Info.AccountPass = Senha;

            try
            {
                string CorrectPatch = Functions.getCorrectPath(target.Info.AccountName);
                using (StreamWriter file = File.CreateText(CorrectPatch + ".json"))
                {
                    string indented = JsonConvert.SerializeObject(target, Formatting.Indented);
                    file.Write(indented);
                }

                W2Log.Write(String.Format("save account sucess: {0}", target.Info.AccountName), ELogType.GAME_EVENT);
                return true;
            }
            catch (Exception e)
            {
                W2Log.Write(String.Format("save account fail: {0}/{1}", target.Info.AccountName, e.Message), ELogType.GAME_EVENT);
                return false;
            }
        }
    

        static public void Start(DBController gs)
        {
            bool Importend = false;

            foreach (string file in Directory.EnumerateFiles("./DataBase/ImportInfo/", "*.update"))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    string line;
                    int countLine = 0;
                    string Login = string.Empty, Senha = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {
                   
                        if (countLine == 0)
                            Login = line;
                        if (countLine == 1)
                            Senha = line;
                         
                        countLine++;
                        if (!string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Senha))
                            break;
                    }

                    if (File.Exists(Functions.getCorrectPath(Login) + ".json"))
                    {
                        STRUCT_ACCOUNTFILE target = new STRUCT_ACCOUNTFILE();

                        if (!gs.ReadAccount(Login, out target))
                        {
                            W2Log.Write(String.Format("fail to read account file {0}", Login), ELogType.GAME_EVENT);
                            Importend = false;
                        }

                        if (0 != String.Compare(target.Info.AccountName, Login))
                        {
                            W2Log.Write(String.Format("fail to read account file {0}/{1}", Login, target.Info.AccountName), ELogType.GAME_EVENT);
                            Importend = false;
                        }
                       // W2Log.Write(String.Format("pass teste file {0}/{1}", target.Info.AccountPass, Senha), ELogType.GAME_EVENT);
                        target.Info.AccountPass = Senha;

                        try
                        {
                            string CorrectPatch = Functions.getCorrectPath(target.Info.AccountName);
                            using (StreamWriter pfile = File.CreateText(CorrectPatch + ".json"))
                            {
                                string indented = JsonConvert.SerializeObject(target, Formatting.Indented);
                                pfile.Write(indented);
                            }

                            W2Log.Write(String.Format("save account sucess: {0}", target.Info.AccountName), ELogType.GAME_EVENT);
                            Importend = true;
                        }
                        catch (Exception e)
                        {
                            W2Log.Write(String.Format("save account fail: {0}/{1}", target.Info.AccountName, e.Message), ELogType.GAME_EVENT);
                            Importend = false;
                        }
                    }
                    if (Importend)
                    {
                        sr.Close();
                        File.Delete(file);
                        Importend = false;
                        W2Log.Write(String.Format($"sucess update user: {Login}"), ELogType.CRITICAL_ERROR);
                        return;
                    }
                   
                        
                }
            }
        }
    }
}
