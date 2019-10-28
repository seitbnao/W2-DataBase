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
    public class ProcessImportUser
    {





        static public void Start( )
        {
            bool Importend = false;

            foreach (string file in Directory.EnumerateFiles("./DataBase/ImportInfo/", "*.user"))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    string line;
                    int countLine = 0;
                    string Login = string.Empty, Senha = string.Empty, Nome = string.Empty, Email = string.Empty, IP = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line.Trim().Trim(' ');
                        
                        if (countLine == 0)
                            Login = line;
                        if (countLine == 1)
                            Senha = line;
                        if (countLine == 2)
                            Nome = line;
                        if (countLine == 3)
                            Email = line;
                        if (countLine == 4)
                            IP = line;
                        countLine++;
                    }
                    if (!string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Senha) && !string.IsNullOrEmpty(Nome) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(IP))
                    {
                        if(!File.Exists(Functions.getCorrectPath(Login) + ".json"))
                            Importend = Functions.CreateEmptyAccount(Login, Senha, IP, Email, Nome);
                    }
                    if (Importend)
                    {
                        sr.Close();
                        File.Delete(file);
                        Importend = false;
                        W2Log.Write(String.Format($"sucess import user: {Login} - {Email}"), ELogType.CRITICAL_ERROR);
                        return;
                    }
                    else
                        W2Log.Write(String.Format($"can't import user: {Login}"), ELogType.CRITICAL_ERROR);
                }
            }
        }
    }
}
