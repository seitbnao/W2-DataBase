using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using W2Open.Common;
using W2Open.Common.GameStructure;
using W2Open.Common.Utility;

namespace W2Open.GameState.ProcessSecTimer
{
    public class ProcessImportItem
    {

        public static int getEmptyCargo(STRUCT_ACCOUNTFILE Account)
        {
            for (int i = 0; i < 120; i++)
            {

                if (Account.Cargo.Items[i].Index == 0)
                    return i;
            }
            return -1;
        }


        static public void Start(DBController gs)
        {
    
            foreach (string file in Directory.EnumerateFiles("./npc/"))
            {
                using (StreamReader sr = new StreamReader(file))
                {
            
                    try
                    {
                        STRUCT_MOB CurrentMob = Functions.ReadAccount<STRUCT_MOB>(file);
                        string indented = JsonConvert.SerializeObject(CurrentMob, Formatting.Indented);

                        int update = gs.MySQL.nQuery(string.Format("INSERT INTO `mobs_json` (`nome`, `conteudo`) VALUES ('{0}', '{1}') ON DUPLICATE KEY UPDATE `nome` = '{2}' ",CurrentMob.Name, indented, CurrentMob.Name));
                        if (update != 0)
                            W2Log.Write(string.Format("Sucess update drops: {0}", file));


                    }
                    catch (Exception e)
                    {
                        return;
                    }
                }
            }
        }




    }
}















        //static public void Start(DBController gs)
        //{
        //    bool Importend = false;
  
        //    foreach (string file in Directory.EnumerateFiles("./DataBase/ImportInfo/", "*.item"))
        //    {
        //        using (StreamReader sr = new StreamReader(file))
        //        {
        //            string line;
        //            while ((line = sr.ReadLine()) != null)
        //            {
        //                //*%d %s %d %d %d %d %d %d %d  ", &id_pedido, accountname, &Index, &Eff1, &Val1, &Eff2, &Val2, &Eff3, &Val3)
        //                string[] data = line.Split(' ');
 

        //                int Idx = gs.GetIndex(data[1]);
        //               // W2Log.Write(String.Format($"Data: {data[1]} AccountIDX: {gs.AccountList[Idx].Account.Info.AccountName} IDX: {Idx}"), ELogType.CRITICAL_ERROR);
        //                if (gs.AccountList[Idx].State != EServerStatus.INGAME)
        //                    return;

        //                int IdPedido = int.Parse(data[0].Replace('*', ' '));
        //                if (Idx != 0)
        //                {
        //                    int EmptyCargo = gs.AccountList[Idx].Account.getEmptyCargo();
        //                    if (EmptyCargo == -1)
        //                    {
        //                        W2Log.Write(String.Format($"can't importitem: {data[1]} no empty slot"), ELogType.CRITICAL_ERROR);
        //                        return;
        //                    }

        //                    short itemID = short.Parse(data[2]);
        //                    if (itemID < 1 || itemID >= BaseDef.MAX_ITEMLIST)
        //                    {
        //                        if (EmptyCargo == -1)
        //                        {
        //                            W2Log.Write(String.Format($"can't importitem: {data[1]} no valid item index"), ELogType.CRITICAL_ERROR);
        //                            return;
        //                        }
        //                    }


        
        //                    if (Idx > 0 && Idx < BaseDef.MAX_MAXUSER && gs.AccountList[Idx].Slot >= 0 && gs.AccountList[Idx].Slot <= 3 && IdPedido != 0)
        //                    {

        //                        gs.AccountList[Idx].Account.Cargo.Items[EmptyCargo].Index = itemID;
        //                        gs.AccountList[Idx].Account.Cargo.Items[EmptyCargo].Effects[0].Code = byte.Parse(data[3]);
        //                        gs.AccountList[Idx].Account.Cargo.Items[EmptyCargo].Effects[0].Value = byte.Parse(data[4]);
        //                        gs.AccountList[Idx].Account.Cargo.Items[EmptyCargo].Effects[1].Code = byte.Parse(data[5]);
        //                        gs.AccountList[Idx].Account.Cargo.Items[EmptyCargo].Effects[1].Value = byte.Parse(data[6]);
        //                        gs.AccountList[Idx].Account.Cargo.Items[EmptyCargo].Effects[2].Code = byte.Parse(data[7]);
        //                        gs.AccountList[Idx].Account.Cargo.Items[EmptyCargo].Effects[2].Value = byte.Parse(data[8]);

        //                        MSG_DBSendItem sm = W2Marshal.CreatePacket<MSG_DBSendItem>(BaseDef._MSG_DBSendItem, (ushort)Idx);

        //                        sm.id_pedido = IdPedido;
        //                        sm.Account = gs.AccountList[Idx].Account.Info.AccountName;
        //                        sm.Items = gs.AccountList[Idx].Account.Cargo.Items[EmptyCargo];
        //                        gs.Server.SendPacket(sm);
        //                        Importend = true;
        //                        W2Log.Write(String.Format($"importitem: {data[2]},{data[3]},{data[4]},{data[5]},{data[6]},{data[7]},{data[8]} to {data[1]} slot {EmptyCargo}"), ELogType.CRITICAL_ERROR);
        //                    }
        //                    else
        //                        Importend = false;
        //                }
        //            }
        //            if (Importend)
        //            {
        //                sr.Close();
        //                File.Delete(file);
        //                Importend = false;
        //                return;
        //            }
        //        }
        //    }
        //}
 