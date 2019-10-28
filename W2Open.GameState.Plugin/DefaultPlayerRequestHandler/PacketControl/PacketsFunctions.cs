using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using W2Open.Common;
using W2Open.Common.GameStructure;
using W2Open.Common.Utility;

namespace W2Open.GameState.Plugin.DefaultPlayerRequestHandler.PacketControl
{
#pragma warning disable 168
    public static class Packet
    {
        //Process Report Guild
        public static DBResult Exec_MSG_GuildZoneReport(DBController gs, pServer GameServer)
        {
            _MSG_GuildZoneReport sm = W2Marshal.GetStructure<_MSG_GuildZoneReport>(GameServer.RecvPacket.RawBuffer);
            _MSG_GuildReport packet = W2Marshal.CreatePacket<_MSG_GuildReport>(_MSG_GuildReport.PacketID);
            for (int i = 0; i < BaseDef.MAX_CHANNEL; i++)
            {
                for (int x = 0; x < BaseDef.MAX_GUILDZONE; x++)
                    gs.ChargedGuildList[i, x] = sm.Guild[x];
            }
            int Guilds = 0;
            for (int i = 0; i < BaseDef.MAX_GUILD; i++)
            {
                gs.g_pGuildInfo[i] = Functions.ReadGuildInfo(i);
                if (!String.IsNullOrEmpty(gs.g_pGuildInfo[i].GuildName))
                    Guilds++;
            }
            gs.Server.SendPacket(packet);
            for(int i = 0; i< BaseDef.MAX_GUILD; i++)
            {
                if (gs.g_pGuildInfo[i].GuildID == 0)
                    continue;
                gs.SendGuildInfo(i);
            }
            W2Log.SendUpdate();
            return DBResult.NO_ERROR;
        }


        //Pacote de Login (SelChar)
        public static DBResult Exec_MSG_DBAccountLogin(DBController gs, pServer GameServer)
        {
            STRUCT_ACCOUNTFILE CurrentAccount = new STRUCT_ACCOUNTFILE();

            MAccountLoginPacket sm = W2Marshal.GetStructure<MAccountLoginPacket>(GameServer.RecvPacket.RawBuffer);



            if (!gs.ReadAccount(sm.Login.Name, out CurrentAccount))
            {
                W2Log.Write(String.Format("fail to read account file {0}", sm.Login.Name), ELogType.GAME_EVENT);
                return DBResult.NO_ERROR;
            }

            if (String.Compare(sm.Login.Pass, CurrentAccount.Info.AccountPass) != 0)
            {
                W2Log.Write(String.Format("Senha incorreta {0}/{1}", sm.Login.Pass, CurrentAccount.Info.AccountPass), ELogType.GAME_EVENT);
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBAccountLoginFail_Pass);
                return DBResult.NO_ERROR;
            }


            MLoginSuccessfulPacket answer = W2Marshal.CreatePacket<MLoginSuccessfulPacket>(BaseDef._MLoginSuccessfulPacket, sm.Header.ClientId);


            int Idx = gs.GetIndex(sm.Header.ClientId);
            int IdxName = gs.GetIndex(sm.Login.Name);




            answer.AccName = CurrentAccount.Info.AccountName;
            answer.Cargo = CurrentAccount.Cargo;
            answer.CargoCoin = CurrentAccount.Coin;


            for (int i = 0; i < GameBasics.MAXL_ACC_MOB; i++)
            {
                unsafe
                {
                    answer.SelChar.Coin[i] = CurrentAccount.Mob[i].Coin;
                    for (int x = 0; x < GameBasics.MAXL_EQUIP; x++)
                    {
                        if (x == 0)
                        {
                            int Face = answer.SelChar.Equip[i].Items[x].Index;
                            if (Face == 22 || Face == 23 || Face == 24 || Face == 25 || Face == 32)
                                Face = CurrentAccount.MobExtra[i].ClassMaster == 0 ? 21 : (ushort)CurrentAccount.MobExtra[i].MortalFace + 7;

                            answer.SelChar.Equip[i].Items[x].Index = (short)Face;
                            answer.SelChar.Equip[i].Items[x].Effects[0].Code = 43;
                            answer.SelChar.Equip[i].Items[x].Effects[0].Value = 0;
                            answer.SelChar.Equip[i].Items[x].Effects[1].Code = 86;
                            answer.SelChar.Equip[i].Items[x].Effects[1].Value = Functions.GetClassType(CurrentAccount.Mob[i]);
                            answer.SelChar.Equip[i].Items[x].Effects[2].Code = 28;
                            answer.SelChar.Equip[i].Items[x].Effects[2].Value = (byte)CurrentAccount.MobExtra[i].Citizen;
                        }



                        answer.SelChar.Equip[i].Items[x] = CurrentAccount.Mob[i].Equip.Items[x];
                    }

                    answer.SelChar.Exp[i] = (int)CurrentAccount.Mob[i].Exp;
                    answer.SelChar.Guild[i] = CurrentAccount.Mob[i].Guild;
                    answer.SelChar.Name[i].Value = CurrentAccount.Mob[i].Name;
                    answer.SelChar.Score[i] = CurrentAccount.Mob[i].BaseScore;
                    answer.SelChar.SPosX[i] = CurrentAccount.Mob[i].SPX;
                    answer.SelChar.SPosY[i] = CurrentAccount.Mob[i].SPY;
                }
            }




            if (IdxName == Idx)
            {
               // W2Log.Write(String.Format("IdxName {0} / Idx {1}", IdxName, Idx), ELogType.GAME_EVENT);
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBStillPlaying);
                return DBResult.NO_ERROR;
            }

            if (IdxName != 0)
            {
                W2Log.Write(String.Format("err, desconectado. conexão anterior finalizada. {0} ", sm.Login.Name), ELogType.GAME_EVENT);

                if (sm.DBNeedSave == 0)
                {
                    gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBAlreadyPlaying);

                    return DBResult.NO_ERROR;
                }
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBStillPlaying);
                return DBResult.NO_ERROR;
            }

            if(!gs.AddAccountList(Idx))
            {
                W2Log.Write(String.Format("err, failue add accountlist. {0} ", sm.Login.Name), ELogType.GAME_EVENT);
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBStillPlaying);
                return DBResult.NO_ERROR;
            }

            gs.AccountList[Idx].Account = CurrentAccount;
            gs.AccountList[Idx].Index = (short)sm.Header.ClientId;
            gs.AccountList[Idx].State = EServerStatus.SEL_CHAR;



            
            gs.Server.SendPacket(answer);

            


            return DBResult.NO_ERROR;
        }


        //Pacote de Login (SelChar -> TMSRV -> ToWord)
        public static DBResult Exec_MSG_DBCharacterLogin2(DBController gs, pServer GameServer)
        {
            _MSG_DBCharacterLogin2 sm = W2Marshal.GetStructure<_MSG_DBCharacterLogin2>(GameServer.RecvPacket.RawBuffer);


            if (sm.Slot < 0 || sm.Slot >= 4)
            {
                W2Log.Write("err,charlogin slot illegal");
                return DBResult.NO_ERROR;
            }

            int idx = gs.GetIndex(sm.Header.ClientId);

            gs.AccountList[idx].Slot = (short)sm.Slot;

            _MSG_CNFCharacterLogin ret = W2Marshal.CreatePacket<_MSG_CNFCharacterLogin>(BaseDef._MSG_CNFCharacterLogin, sm.Header.ClientId);

            STRUCT_ACCOUNTFILE File = gs.AccountList[idx].Account;

            if (String.IsNullOrEmpty(File.Info.AccountName))
            {
                W2Log.Write(String.Format("err,charlogin mobname {0} empty", File.Info.AccountName));

                return DBResult.NO_ERROR;
            }



            ret.Slot = (short)sm.Slot;

            ret.Mob = File.Mob[sm.Slot];
            ret.Mob.CurrentScore = ret.Mob.BaseScore;


            ret.ShortSkill = File.Skillbar[sm.Slot].ShortSkill;



            ret.Affects = File.Affects[sm.Slot].Affects;

            ret.mobExtra = File.MobExtra[sm.Slot];

            ret.mobExtra.ClassMaster = File.MobExtra[sm.Slot].ClassMaster;
            gs.AccountList[idx].State = EServerStatus.INGAME;
            GameServer.SendPacket(ret);
            return DBResult.NO_ERROR;
        }


        //Process SaveMob (SaveUser)
        public static DBResult Exec_MSG_DBSaveMob(DBController gs, pServer GameServer)
        {
            _MSG_DBSaveMob sm = W2Marshal.GetStructure<_MSG_DBSaveMob>(GameServer.RecvPacket.RawBuffer);

            int Idx = gs.GetIndex(sm.Header.ClientId);

            int Slot = gs.AccountList[Idx].Slot;

            if (Slot < 0 || Slot >= 4)
            {
                
                W2Log.Write(String.Format("err,savemob1 {0} {1} {2} {3} {4}", sm.Header.ClientId, Slot, sm.Slot, gs.AccountList[Idx].Account.Info.AccountName, sm.Accountname.Value));
                return DBResult.NO_ERROR;
            }

            if (Slot != sm.Slot)
            {
                
                W2Log.Write(String.Format("err,savemob2 {0} {1} {2} {3} {4}", sm.Header.ClientId, Slot, sm.Slot, gs.AccountList[Idx].Account.Info.AccountName, sm.Accountname.Value));
                //[19:52:28] err,savemob2 2 0 -1 wedbr123 wedbr123

                return DBResult.NO_ERROR;
            }

            if (gs.AccountList[Idx].State == EServerStatus.CLOSED)
            {
                W2Log.Write(String.Format("err,savemob3 {0} {1} {2} {3} {4}", sm.Header.ClientId, Slot, sm.Slot, gs.AccountList[Idx].Account.Info.AccountName, sm.Accountname.Value));
                return DBResult.NO_ERROR;
            }

          //  

            
            gs.AccountList[Idx].Account.Donate = sm.Donate;
            gs.AccountList[Idx].Account.Mob[Slot] = sm.Mob;
            gs.AccountList[Idx].Account.MobExtra[Slot] = sm.Extra;
            gs.AccountList[Idx].Account.other = sm.Other;
            
            gs.AccountList[Idx].Account.Affects[Slot].Affects = sm.Affects;
            
            gs.AccountList[Idx].Account.Cargo = sm.Cargo;
            gs.AccountList[Idx].Account.Skillbar[Slot].ShortSkill = sm.ShortSkill;
            gs.AccountList[Idx].Account.Coin = sm.Coin;


            gs.SaveAccount(sm.Header.ClientId);



            return DBResult.NO_ERROR;
        }


        //Process SaveMob (CloseUser)
        public static DBResult Exec_MSG_SavingQuit(DBController gs, pServer GameServer)
        {
            _MSG_DBSaveMob sm = W2Marshal.GetStructure<_MSG_DBSaveMob>(GameServer.RecvPacket.RawBuffer);

            int Idx = gs.GetIndex(sm.Header.ClientId);


            int Slot = gs.AccountList[Idx].Slot;

            if (Slot < 0 || Slot >= 4)
            {
                W2Log.Write(String.Format("err,savenquit1 {0} {1} {2} {3} {4}", sm.Header.ClientId, Slot, sm.Slot, gs.AccountList[Idx].Account.Info.AccountName, sm.Accountname.Value));
                return DBResult.NO_ERROR;
            }

            if (Slot != sm.Slot)
            {
                W2Log.Write(String.Format("err,savenquit2 {0} {1} {2} {3} {4}", sm.Header.ClientId, Slot, sm.Slot, gs.AccountList[Idx].Account.Info.AccountName, sm.Accountname.Value));
                return DBResult.NO_ERROR;
            }

            if (gs.AccountList[Idx].State == EServerStatus.CLOSED)
            {
                W2Log.Write(String.Format("err,savenquit3 {0} {1} {2} {3} {4}", sm.Header.ClientId, Slot, sm.Slot, gs.AccountList[Idx].Account.Info.AccountName, sm.Accountname.Value));
                return DBResult.NO_ERROR;
            }


            gs.AccountList[Idx].Account.Donate = sm.Donate;
            gs.AccountList[Idx].Account.Mob[Slot] = sm.Mob;
            gs.AccountList[Idx].Account.MobExtra[Slot] = sm.Extra;
            gs.AccountList[Idx].Account.other = sm.Other;
            gs.AccountList[Idx].Account.Affects[Slot].Affects = sm.Affects;
            gs.AccountList[Idx].Account.Cargo = sm.Cargo;
            gs.AccountList[Idx].Account.Skillbar[Slot].ShortSkill = sm.ShortSkill;
            gs.AccountList[Idx].Account.Coin = sm.Coin;


            if (gs.SaveAccount(sm.Header.ClientId))
            {
                W2Log.Write(String.Format("account logout: {0}/{1}", Idx, sm.Accountname.Value), ELogType.CRITICAL_ERROR);
                gs.RemoveAccountList(Idx);
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBCNFAccountLogOut);
                return DBResult.NO_ERROR;
            }

            return DBResult.NO_ERROR;
        }


        //Process ImportItem
        public static DBResult Exec_MSG_DBSendItem(DBController gs, pServer GameServer)
        {
            MSG_DBSendItem sm = W2Marshal.GetStructure<MSG_DBSendItem>(GameServer.RecvPacket.RawBuffer);

            int Idx = gs.GetIndex(sm.Header.ClientId);


            if (String.IsNullOrEmpty(gs.AccountList[Idx].Account.Info.AccountName))
                W2Log.Write("MSG_DBSendItem error user is empty");

            CultureInfo Culture = new CultureInfo("pt-BR");
            DateTime localDate = DateTime.Now;
            string atual = localDate.ToString(Culture).Replace('/', '-').ToString();


            if (sm.Result == 0)//Sucesso
            {
                int updated = gs.MySQL.nQuery(string.Format("UPDATE `lista_compras` SET `data_envio` = '{0}' WHERE `id` = '{1}' and `login_conta` = '{2}' ", atual, sm.id_pedido, gs.AccountList[Idx].Account.Info.AccountName));

                if (updated != 0)
                    W2Log.Write(string.Format("Sucess DBSendItem: {0} - {1}", atual, sm.id_pedido));
                else
                    W2Log.Write(string.Format("Fail DBSendItem: {0} - {1}", atual, sm.id_pedido));

                return DBResult.NO_ERROR;
            }

            if (sm.Result == 3)//Falta espaço no Cargo recriar o arquivo
            {
                W2Log.Write(string.Format("Fail DBSendItem: {0} - {1} no empty cargo slot", atual, sm.id_pedido));
                bool recreatedItem = Functions.CreateImportItem(gs.AccountList[Idx].Account.Info.AccountName, sm.Items, sm.id_pedido,  false);
                if(recreatedItem)
                    W2Log.Write(string.Format("Recreated DBSendItem: {0} - {1}/{2}", atual, sm.id_pedido, gs.AccountList[Idx].Account.Info.AccountName));
            }

            if (sm.id_pedido == 344359 && sm.Result == 5)//Selo da alma enviado com sucesso
                W2Log.Write(string.Format("Sucess send seal of soul: {0} - {1}", atual, sm.id_pedido));



            return DBResult.NO_ERROR;
        }


        //Process CreateChar (SelChar)
        public static DBResult Exec_MSG_DBCreateCharacter(DBController gs, pServer GameServer)
        {
            STMSG_CreateCharacter sm = W2Marshal.GetStructure<STMSG_CreateCharacter>(GameServer.RecvPacket.RawBuffer);



            int Slot = sm.Slot;
            int cls = sm.MobClass;
            bool ret = false;

            int Idx = gs.GetIndex(sm.Header.ClientId);

            if (Slot < 0 || Slot >= 4)
            {
                W2Log.Write(string.Format("err,newchar  slot out of range - {0}", gs.AccountList[Idx].Account.Info.AccountName), ELogType.GAME_EVENT);
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);
                return DBResult.NO_ERROR;
            }
            if (cls < 0 || cls >= 4)
            {
                W2Log.Write(string.Format("err,newchar  class out of range - {0}", gs.AccountList[Idx].Account.Info.AccountName), ELogType.GAME_EVENT);
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);
                return DBResult.NO_ERROR;
            }

            string[] BadNames = { "KING", "KINGDOM", "GRITAR", "RELO" };
            for (int i = 0; i < BadNames.Length; i++)
            {
                if (!sm.PlayerName.Contains(BadNames[i]))
                    continue;

                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);
                return DBResult.NO_ERROR;
            }


            STRUCT_MOB mob = gs.AccountList[Idx].Account.Mob[Slot];

            if (!String.IsNullOrEmpty(mob.Name))
            {
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);
                W2Log.Write(string.Format("err,newchar already charged - {0}", gs.AccountList[Idx].Account.Info.AccountName), ELogType.GAME_EVENT);

                return DBResult.NO_ERROR;
            }


            ret = gs.CreateChar(sm.PlayerName, gs.AccountList[Idx].Account.Info.AccountName);

            if (ret == false)
            {
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);
                return DBResult.NO_ERROR;
            }

            gs.AccountList[Idx].Account.Mob[Slot] = STRUCT_MOB.ClearProperty();
            gs.AccountList[Idx].Account.MobExtra[Slot] = new STRUCT_MOBEXTRA();
            gs.AccountList[Idx].Account.Skillbar[Slot] = STRUCT_SKILLBAR.Clear();
            gs.AccountList[Idx].Account.MobExtra[Slot].ClassMaster = (short)ClassType.Mortal;



            gs.AccountList[Idx].Account.Mob[Slot] = gs.BaseMob[cls];
 
            gs.AccountList[Idx].Account.MobExtra[Slot].MortalFace = gs.AccountList[Idx].Account.Mob[Slot].Equip.Items[0].Index;
            gs.AccountList[Idx].Account.Mob[Slot].Name = sm.PlayerName;

            gs.AccountList[Idx].Account.Mob[Slot].CurrentScore = gs.AccountList[Idx].Account.Mob[Slot].BaseScore;

            ret = gs.SaveAccount(sm.Header.ClientId);

            if (ret == false)
            {

                W2Log.Write(string.Format("err,newchar fail - create file - {0}", sm.PlayerName), ELogType.GAME_EVENT);

                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);
                return DBResult.NO_ERROR;
            }
            W2Log.Write(string.Format("create character [{0}]", sm.PlayerName), ELogType.GAME_EVENT);


            MSG_CNFNewCharacter answer = W2Marshal.CreatePacket<MSG_CNFNewCharacter>(BaseDef._MSG_DBCNFNewCharacter, sm.Header.ClientId);

            answer.Slot = Slot;


            for (int i = 0; i < GameBasics.MAXL_ACC_MOB; i++)
            {
                unsafe
                {
                    answer.SelChar.Coin[i] = gs.AccountList[Idx].Account.Mob[i].Coin;
                    for (int x = 0; x < GameBasics.MAXL_EQUIP; x++)
                    {
                        if (x == 0)
                        {
                            int Face = answer.SelChar.Equip[i].Items[x].Index;
                            if (Face == 22 || Face == 23 || Face == 24 || Face == 25 || Face == 32)
                                Face = gs.AccountList[Idx].Account.MobExtra[i].ClassMaster == 0 ? 21 : (ushort)gs.AccountList[Idx].Account.MobExtra[i].MortalFace + 7;

                            answer.SelChar.Equip[i].Items[x].Index = (short)Face;
                            answer.SelChar.Equip[i].Items[x].Effects[0].Code = 43;
                            answer.SelChar.Equip[i].Items[x].Effects[0].Value = 0;
                            answer.SelChar.Equip[i].Items[x].Effects[1].Code = 86;
                            answer.SelChar.Equip[i].Items[x].Effects[1].Value = Functions.GetClassType(gs.AccountList[Idx].Account.Mob[i]);
                            answer.SelChar.Equip[i].Items[x].Effects[2].Code = 28;
                            answer.SelChar.Equip[i].Items[x].Effects[2].Value = (byte)gs.AccountList[Idx].Account.MobExtra[i].Citizen;
                        }



                        answer.SelChar.Equip[i].Items[x] = gs.AccountList[Idx].Account.Mob[i].Equip.Items[x];
                    }

                    answer.SelChar.Exp[i] = (int)gs.AccountList[Idx].Account.Mob[i].Exp;
                    answer.SelChar.Guild[i] = gs.AccountList[Idx].Account.Mob[i].Guild;
                    answer.SelChar.Name[i].Value = gs.AccountList[Idx].Account.Mob[i].Name;
                    answer.SelChar.Score[i] = gs.AccountList[Idx].Account.Mob[i].BaseScore;
                    answer.SelChar.SPosX[i] = gs.AccountList[Idx].Account.Mob[i].SPX;
                    answer.SelChar.SPosY[i] = gs.AccountList[Idx].Account.Mob[i].SPY;
                }
            }
            gs.Server.SendPacket(answer);
            return DBResult.NO_ERROR;
        }


        //Process DeletChar (SelChar)
        public static DBResult Exec_MSG_DBCNFDeleteCharacter(DBController gs, pServer GameServer)
        {
            STMSG_DeleteCharacter sm = W2Marshal.GetStructure<STMSG_DeleteCharacter>(GameServer.RecvPacket.RawBuffer);



            int Slot = sm.Slot;

            bool ret = false;

            int Idx = gs.GetIndex(sm.Header.ClientId);

            if (Slot < 0 || Slot >= 4)
            {
                W2Log.Write(string.Format("err,deletchar  slot out of range - {0}", gs.AccountList[Idx].Account.Info.AccountName), ELogType.GAME_EVENT);
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBDeleteCharacterFail);
                return DBResult.NO_ERROR;
            }



            STRUCT_MOB mob = gs.AccountList[Idx].Account.Mob[Slot];

            if (String.IsNullOrEmpty(mob.Name))
            {
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBDeleteCharacterFail);
                W2Log.Write(string.Format("err,deletchar already charged - {0}", gs.AccountList[Idx].Account.Info.AccountName), ELogType.GAME_EVENT);

                return DBResult.NO_ERROR;
            }

            if (String.Compare(gs.AccountList[Idx].Account.Info.AccountPass, sm.Pass) != 0)
            {
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBDeleteCharacterFail);
                W2Log.Write(string.Format("err,deletchar incorret pass - {0}", gs.AccountList[Idx].Account.Info.AccountName), ELogType.GAME_EVENT);
                return DBResult.NO_ERROR;
            }



            ret = false;
            try
            {
                File.Delete(Functions.getCorrectCharPath(sm.PlayerName));
                ret = true;
            }
            catch (Exception e)
            {
                W2Log.Write(string.Format("err,deletchar - {0}/{1}", gs.AccountList[Idx].Account.Info.AccountName, sm.PlayerName), ELogType.GAME_EVENT);
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBDeleteCharacterFail);
                return DBResult.NO_ERROR;
            }

            if (ret == false)
            {
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBDeleteCharacterFail);
                return DBResult.NO_ERROR;
            }

            gs.AccountList[Idx].Account.Mob[Slot] = STRUCT_MOB.ClearProperty();
            gs.AccountList[Idx].Account.MobExtra[Slot] = new STRUCT_MOBEXTRA();
            gs.AccountList[Idx].Account.Skillbar[Slot] = STRUCT_SKILLBAR.Clear();
            for (int i = 0; i <32; i++)
            {
                gs.AccountList[Idx].Account.Affects[Slot].Affects[i] = STRUCT_AFFECT.Clear();
            }



            ret = gs.SaveAccount(sm.Header.ClientId);

            if (ret == false)
            {

                W2Log.Write(string.Format("err,deletchar fail - save account - {0}", sm.PlayerName), ELogType.GAME_EVENT);

                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);
                return DBResult.NO_ERROR;
            }
            W2Log.Write(string.Format("delete character [{0}]", sm.PlayerName), ELogType.GAME_EVENT);


            MSG_CNFNewCharacter answer = W2Marshal.CreatePacket<MSG_CNFNewCharacter>(BaseDef._MSG_DBCNFDeleteCharacter, sm.Header.ClientId);

            answer.Slot = Slot;

            for (int i = 0; i < GameBasics.MAXL_ACC_MOB; i++)
            {
                unsafe
                {
                    answer.SelChar.Coin[i] = gs.AccountList[Idx].Account.Mob[i].Coin;
                    for (int x = 0; x < GameBasics.MAXL_EQUIP; x++)
                    {
                        if (x == 0)
                        {
                            int Face = answer.SelChar.Equip[i].Items[x].Index;
                            if (Face == 22 || Face == 23 || Face == 24 || Face == 25 || Face == 32)
                                Face = gs.AccountList[Idx].Account.MobExtra[i].ClassMaster == 0 ? 21 : (ushort)gs.AccountList[Idx].Account.MobExtra[i].MortalFace + 7;

                            answer.SelChar.Equip[i].Items[x].Index = (short)Face;
                        }



                        answer.SelChar.Equip[i].Items[x] = gs.AccountList[Idx].Account.Mob[i].Equip.Items[x];
                    }

                    answer.SelChar.Exp[i] = (int)gs.AccountList[Idx].Account.Mob[i].Exp;
                    answer.SelChar.Guild[i] = gs.AccountList[Idx].Account.Mob[i].Guild;
                    answer.SelChar.Name[i].Value = gs.AccountList[Idx].Account.Mob[i].Name;
                    answer.SelChar.Score[i] = gs.AccountList[Idx].Account.Mob[i].BaseScore;
                    answer.SelChar.SPosX[i] = gs.AccountList[Idx].Account.Mob[i].SPX;
                    answer.SelChar.SPosY[i] = gs.AccountList[Idx].Account.Mob[i].SPY;
                }
            }

            gs.Server.SendPacket(answer);
            return DBResult.NO_ERROR;
        }


        //Process NeedSave (RemoveAccountList)
        public static DBResult Exec_MSG_DBNoNeedSave(DBController gs, pServer GameServer)
        {
            _MSG_SIGNAL sm = W2Marshal.GetStructure<_MSG_SIGNAL>(GameServer.RecvPacket.RawBuffer);

            int Idx = gs.GetIndex(sm.Header.ClientId);


            gs.RemoveAccountList(Idx);

            return DBResult.NO_ERROR;
        }


        //Process GuildWar (Requesição de Guerra)
        public static DBResult Exec_MSG_War(DBController gs, pServer GameServer)
        {
            _MSG_SIGNALPARM2 sm = W2Marshal.GetStructure<_MSG_SIGNALPARM2>(GameServer.RecvPacket.RawBuffer);

            int myguild = sm.parm;

            if (myguild <= 0 || myguild >= 5000)
            {
                W2Log.Write($"Guild out or range {myguild}", ELogType.CRITICAL_ERROR);
                return DBResult.NO_ERROR;
            }

            W2Log.Write($"guild war request { sm.parm}, { sm.parm2} ");

            gs.g_pGuildWar[myguild] = (short)sm.parm2;

            gs.Server.SendSignalParm(0, BaseDef._MSG_War, sm.parm, sm.parm2);

            return DBResult.NO_ERROR;
        }


        //Process GuildAlly (Requisição de Aliança)
        public static DBResult Exec_MSG_GuildAlly(DBController gs, pServer GameServer)
        {
            _MSG_SIGNALPARM2 sm = W2Marshal.GetStructure<_MSG_SIGNALPARM2>(GameServer.RecvPacket.RawBuffer);

            int myguild = sm.parm;

            if (myguild <= 0 || myguild >= 5000)
            {
                W2Log.Write($"Guild out or range {myguild}", ELogType.CRITICAL_ERROR);
                return DBResult.NO_ERROR;
            }

            W2Log.Write($"guild ally request { sm.parm}, { sm.parm2} ");

            gs.g_pGuildAlly[myguild] = (short)sm.parm2;

            gs.Server.SendSignalParm(0, BaseDef._MSG_GuildAlly, sm.parm, sm.parm2);

            return DBResult.NO_ERROR;
        }


        //Process GuildIndo (Envia as Guildas Para TMSRV)
        public static DBResult Exec_MSG_GuildInfo(DBController gs, pServer GameServer)
        {
            MSG_GuildInfo sm = W2Marshal.GetStructure<MSG_GuildInfo>(GameServer.RecvPacket.RawBuffer);

            int myguild = sm.Guild;

            if (myguild <= 0 || myguild >= 5000)
            {
                W2Log.Write($"guild index out or range { sm.Guild}");
                return DBResult.NO_ERROR;
            }

            gs.g_pGuildInfo[myguild] = sm.GuildInfo;

            if (myguild != gs.g_pGuildInfo[myguild].GuildID)
            {
                W2Log.Write($"guild index fixed {gs.g_pGuildInfo[myguild].GuildID } to {myguild}");
                gs.g_pGuildInfo[myguild].GuildID = myguild;
            }


            gs.SendGuildInfo(myguild);

            W2Log.Write($"guild update {gs.g_pGuildInfo[myguild].GuildID } - {gs.g_pGuildInfo[myguild].GuildName}");

            if (!Functions.WriteGuildInfo(gs.g_pGuildInfo))
                W2Log.Write("fail to write guild info");

          

            return DBResult.NO_ERROR;
        }


        //Process UpdateSaphire (Atualiza as Saphriras requerida pelos Reis)
        public static DBResult Exec_MSG_DBUpdateSapphire(DBController gs, pServer GameServer)
        {
            _MSG_SIGNALPARM sm = W2Marshal.GetStructure<_MSG_SIGNALPARM>(GameServer.RecvPacket.RawBuffer);
            ConfigServer.ReadConfigFile(gs.Config);

            int Old = gs.Config.Sapphire;
            if (sm.parm == 1)
                gs.Config.Sapphire *= 2;
            else
                gs.Config.Sapphire /= 2;

            if (gs.Config.Sapphire < 1)
                gs.Config.Sapphire = 1;
            else if (gs.Config.Sapphire > 64)
                gs.Config.Sapphire = 64;


            gs.Server.SendSignalParm(0, BaseDef._MSG_DBSetIndex, -1, gs.Config.Sapphire, sm.Header.ClientId);

            ConfigServer.saveConfig(gs.Config);
            W2Log.Write($"sucess db set saphire: {Old} -> {gs.Config.Sapphire}");
            return DBResult.NO_ERROR;
        }


        //Process Record (Cria o Record de Users online em X time)
        public static DBResult Exec_MSG_MessageDBRecord(DBController gs, pServer GameServer)
        {
            MSG_MessageDBRecord sm = W2Marshal.GetStructure<MSG_MessageDBRecord>(GameServer.RecvPacket.RawBuffer);
            bool ret = Functions.ProcessRecord(gs.PlayerCount, sm.Record);

            W2Log.Write(sm.Record);

            if (ret == false)
                W2Log.Write("DBRecord Fail");

            return DBResult.NO_ERROR;
        }


        //Process CreateChar Arch (SelChar)
        public static DBResult Exec_MSG_DBCreateArchCharacter(DBController gs, pServer GameServer)
        {
            MSG_DBCreateArchCharacter sm = W2Marshal.GetStructure<MSG_DBCreateArchCharacter>(GameServer.RecvPacket.RawBuffer);
            int cls = sm.MobClass;
            bool ret = false;

            int Idx = gs.GetIndex(sm.Header.ClientId);

            int Slot = 0;

            for (Slot = 0; Slot < 5; Slot++)
            {
                if (string.IsNullOrEmpty(gs.AccountList[Idx].Account.Mob[Slot].Name))
                    break;
            }

            if (Slot < 0 || Slot >= 4)
            {
                W2Log.Write(string.Format("err,arch newchar  slot out of range - {0}", gs.AccountList[Idx].Account.Info.AccountName), ELogType.GAME_EVENT);
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);
                return DBResult.NO_ERROR;
            }
            if (cls < 0 || cls >= 4)
            {
                W2Log.Write(string.Format("err,arch newchar  class out of range - {0}", gs.AccountList[Idx].Account.Info.AccountName), ELogType.GAME_EVENT);
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);
                return DBResult.NO_ERROR;
            }

            string[] BadNames = { "KING", "KINGDOM", "GRITAR", "RELO" };
            for (int i = 0; i < BadNames.Length; i++)
            {
                if (!sm.PlayerName.Contains(BadNames[i]))
                    continue;

                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);
                return DBResult.NO_ERROR;
            }


            STRUCT_MOB mob = gs.AccountList[Idx].Account.Mob[Slot];

            if (!String.IsNullOrEmpty(mob.Name))
            {
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);
                W2Log.Write(string.Format("err,arch newchar already charged - {0}", gs.AccountList[Idx].Account.Info.AccountName), ELogType.GAME_EVENT);

                return DBResult.NO_ERROR;
            }


            ret = gs.CreateChar(sm.PlayerName, gs.AccountList[Idx].Account.Info.AccountName);

            if (ret == false)
            {
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);
                return DBResult.NO_ERROR;
            }

            gs.AccountList[Idx].Account.Mob[Slot] = STRUCT_MOB.ClearProperty();
            gs.AccountList[Idx].Account.MobExtra[Slot] = new STRUCT_MOBEXTRA();
            gs.AccountList[Idx].Account.Skillbar[Slot] = STRUCT_SKILLBAR.Clear();




            gs.AccountList[Idx].Account.Mob[Slot] = gs.BaseMob[cls];

            gs.AccountList[Idx].Account.MobExtra[Slot].ClassMaster = (short)ClassType.Arch;
            gs.AccountList[Idx].Account.MobExtra[Slot].Arch.MortalSlot = (sbyte)sm.MortalSlot;



            gs.AccountList[Idx].Account.Mob[Slot].Equip.Items[0].Index = (short)(sm.MortalFace + 5 + cls);



            gs.AccountList[Idx].Account.MobExtra[Slot].MortalFace = gs.AccountList[Idx].Account.Mob[Slot].Equip.Items[0].Index;
            gs.AccountList[Idx].Account.Mob[Slot].Name = sm.PlayerName;

            gs.AccountList[Idx].Account.Mob[Slot].CurrentScore = gs.AccountList[Idx].Account.Mob[Slot].BaseScore;

            ret = gs.SaveAccount(sm.Header.ClientId);

            if (ret == false)
            {

                W2Log.Write(string.Format("err,arch newchar fail - create file - {0}", sm.PlayerName), ELogType.GAME_EVENT);

                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);
                return DBResult.NO_ERROR;
            }
            W2Log.Write(string.Format("create arch character [{0}]", sm.PlayerName), ELogType.GAME_EVENT);


            MSG_CNFNewCharacter answer = W2Marshal.CreatePacket<MSG_CNFNewCharacter>(BaseDef._MSG_DBCNFNewCharacter, sm.Header.ClientId);

            answer.Slot = Slot;


            for (int i = 0; i < GameBasics.MAXL_ACC_MOB; i++)
            {
                unsafe
                {
                    answer.SelChar.Coin[i] = gs.AccountList[Idx].Account.Mob[i].Coin;
                    for (int x = 0; x < GameBasics.MAXL_EQUIP; x++)
                    {
                        if (x == 0)
                        {
                            int Face = answer.SelChar.Equip[i].Items[x].Index;
                            if (Face == 22 || Face == 23 || Face == 24 || Face == 25 || Face == 32)
                                Face = gs.AccountList[Idx].Account.MobExtra[i].ClassMaster == 0 ? 21 : (ushort)gs.AccountList[Idx].Account.MobExtra[i].MortalFace + 7;

                            answer.SelChar.Equip[i].Items[x].Index = (short)Face;
                            answer.SelChar.Equip[i].Items[x].Effects[0].Code = 43;
                            answer.SelChar.Equip[i].Items[x].Effects[0].Value = 0;
                            answer.SelChar.Equip[i].Items[x].Effects[1].Code = 86;
                            answer.SelChar.Equip[i].Items[x].Effects[1].Value = Functions.GetClassType(gs.AccountList[Idx].Account.Mob[i]);
                            answer.SelChar.Equip[i].Items[x].Effects[2].Code = 28;
                            answer.SelChar.Equip[i].Items[x].Effects[2].Value = (byte)gs.AccountList[Idx].Account.MobExtra[i].Citizen;
                        }



                        answer.SelChar.Equip[i].Items[x] = gs.AccountList[Idx].Account.Mob[i].Equip.Items[x];
                    }

                    answer.SelChar.Exp[i] = (int)gs.AccountList[Idx].Account.Mob[i].Exp;
                    answer.SelChar.Guild[i] = gs.AccountList[Idx].Account.Mob[i].Guild;
                    answer.SelChar.Name[i].Value = gs.AccountList[Idx].Account.Mob[i].Name;
                    answer.SelChar.Score[i] = gs.AccountList[Idx].Account.Mob[i].BaseScore;
                    answer.SelChar.SPosX[i] = gs.AccountList[Idx].Account.Mob[i].SPX;
                    answer.SelChar.SPosY[i] = gs.AccountList[Idx].Account.Mob[i].SPY;
                }
            }

            gs.Server.SendPacket(answer);
            return DBResult.NO_ERROR;
        }


        //Process Capsule (Envia o Selo da Alma para a TMSRV)
        public static DBResult Exec_MSG_DBCapsuleInfo(DBController gs, pServer GameServer)
        {
            _MSG_SIGNALPARM sm = W2Marshal.GetStructure<_MSG_SIGNALPARM>(GameServer.RecvPacket.RawBuffer);
            ConfigServer.ReadConfigFile(gs.Config);

            int Index = sm.parm;


            STRUCT_CAPSULE file = new STRUCT_CAPSULE();

            if (!Functions.ReadCapsule(Index,out file))
            {
                W2Log.Write($"Exec_MSG_DBCapsuleInfo error read capsule index: {Index}");
                return DBResult.NO_ERROR;
            }

            MSG_CNFDBCapsuleInfo p = W2Marshal.CreatePacket<MSG_CNFDBCapsuleInfo>(BaseDef._MSG_CNFDBCapsuleInfo, sm.Header.ClientId);


            p.Index = Index;

            p.Capsule.Read = 1;
            p.Capsule.MortalClass = file.Mob.Class;
            p.Capsule.ClassCele = file.Extra.SaveCelestial.Class;
            p.Capsule.SubClass = file.Mob.Class;
            p.Capsule.LevelCele = file.Extra.SaveCelestial.BaseScore.Level;
            p.Capsule.LevelSub = file.Mob.BaseScore.Level;

            p.Capsule.For = file.Mob.BaseScore.Str;
            p.Capsule.Int = file.Mob.BaseScore.Int;
            p.Capsule.Dex = file.Mob.BaseScore.Dex;
            p.Capsule.Con = file.Mob.BaseScore.Con;

            p.Capsule.ScoreBonus = file.Mob.ScoreBonus;
            p.Capsule.SkillPoint = file.Mob.SkillBonus;

            p.Capsule.ArchQuest = file.Extra.Arch.Cristal;
            p.Capsule.CelestialQuest = file.Extra.Celestial.Reset;
            p.Capsule.ArcanaQuest = file.Extra.Circle;


            gs.Server.SendPacket(p);

            W2Log.Write($"error read capsule index: {Index}");
            return DBResult.NO_ERROR;
        }


        //Process PutCapsule (Cria o Selo da Alma e Apaga o Char)
        public static DBResult Exec_MSG_DBPutInCapsule(DBController gs, pServer GameServer)
        {
            _MSG_SIGNALPARM sm = W2Marshal.GetStructure<_MSG_SIGNALPARM>(GameServer.RecvPacket.RawBuffer);
            ConfigServer.ReadConfigFile(gs.Config);

            int Slot = sm.parm;

            int Idx = gs.GetIndex(sm.Header.ClientId);

            if (Slot < 0 || Slot >= 4)
            {
                W2Log.Write($"err,putincapsule slot out of range - {gs.AccountList[Idx].Account.Info.AccountName}");

                return DBResult.NO_ERROR;
            }






            gs.Config.LastCapsule++;

            MItem Cap = MItem.Empty();


            Cap.Index = 3443;
            Cap.Effects[0].Code = 59;
            Cap.Effects[0].Value = (byte)(gs.Config.LastCapsule / 256);
            Cap.Effects[1].Code = 59;
            Cap.Effects[1].Value = (byte)gs.Config.LastCapsule;
            Cap.Effects[2].Code = 59;
            Cap.Effects[2].Value = 0;
            short Face = gs.AccountList[Idx].Account.Mob[Slot].Equip.Items[0].Index;
            if ((Face % 10) >= 6 && (gs.AccountList[Idx].Account.Mob[Slot].LearnedSkill & 0x40000000) != 0)
                Cap.Effects[3].Value = 1;





            STRUCT_CAPSULE File = STRUCT_CAPSULE.Empty();

            //Sava a capsule
            File.Mob = gs.AccountList[Idx].Account.Mob[Slot];
            File.Extra = gs.AccountList[Idx].Account.MobExtra[Slot];
            File.Extra.Arch.MortalSlot = -1;

            if (!Functions.WriteCapsule(gs.Config.LastCapsule, File))
            {
                W2Log.Write($"fail putchar in capsule [{gs.AccountList[Idx].Account.Info.AccountName}]");
                return DBResult.NO_ERROR;
            }
            W2Log.Write($"putchar in capsule [{gs.AccountList[Idx].Account.Info.AccountName}]");
            ConfigServer.saveConfig(gs.Config);


            //Apaga o char do q fez a capsule

            gs.AccountList[Idx].Account.Mob[Slot] = STRUCT_MOB.ClearProperty();
            gs.AccountList[Idx].Account.MobExtra[Slot] = new STRUCT_MOBEXTRA();
            gs.AccountList[Idx].Account.Skillbar[Slot] = STRUCT_SKILLBAR.Clear();
            for (int i = 0; i < 32; i++)
            {
                gs.AccountList[Idx].Account.Affects[Slot].Affects[i] = STRUCT_AFFECT.Clear();
            }
            bool ret = gs.SaveAccount(sm.Header.ClientId);
            if (ret == false)
            {
                W2Log.Write(string.Format("err,putchar in capsule fail - save account - {0}", Idx), ELogType.GAME_EVENT);
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);
                return DBResult.NO_ERROR;
            }

            gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBCNFCapsuleSucess);



            MSG_CNFNewCharacter answer = W2Marshal.CreatePacket<MSG_CNFNewCharacter>(BaseDef._MSG_DBCNFDeleteCharacter, sm.Header.ClientId);
            for (int i = 0; i < GameBasics.MAXL_ACC_MOB; i++)
            {
                unsafe
                {
                    answer.SelChar.Coin[i] = gs.AccountList[Idx].Account.Mob[i].Coin;
                    for (int x = 0; x < GameBasics.MAXL_EQUIP; x++)
                    {
                        if (x == 0)
                        {
                            int Faces = answer.SelChar.Equip[i].Items[x].Index;
                            if (Faces == 22 || Faces == 23 || Faces == 24 || Faces == 25 || Faces == 32)
                                Faces = gs.AccountList[Idx].Account.MobExtra[i].ClassMaster == 0 ? 21 : (ushort)gs.AccountList[Idx].Account.MobExtra[i].MortalFace + 7;

                            answer.SelChar.Equip[i].Items[x].Index = (short)Faces;
                            answer.SelChar.Equip[i].Items[x].Effects[0].Code = 43;
                            answer.SelChar.Equip[i].Items[x].Effects[0].Value = 0;
                            answer.SelChar.Equip[i].Items[x].Effects[1].Code = 86;
                            answer.SelChar.Equip[i].Items[x].Effects[1].Value = Functions.GetClassType(gs.AccountList[Idx].Account.Mob[i]);
                            answer.SelChar.Equip[i].Items[x].Effects[2].Code = 28;
                            answer.SelChar.Equip[i].Items[x].Effects[2].Value = (byte)gs.AccountList[Idx].Account.MobExtra[i].Citizen;
                        }



                        answer.SelChar.Equip[i].Items[x] = gs.AccountList[Idx].Account.Mob[i].Equip.Items[x];
                    }

                    answer.SelChar.Exp[i] = (int)gs.AccountList[Idx].Account.Mob[i].Exp;
                    answer.SelChar.Guild[i] = gs.AccountList[Idx].Account.Mob[i].Guild;
                    answer.SelChar.Name[i].Value = gs.AccountList[Idx].Account.Mob[i].Name;
                    answer.SelChar.Score[i] = gs.AccountList[Idx].Account.Mob[i].BaseScore;
                    answer.SelChar.SPosX[i] = gs.AccountList[Idx].Account.Mob[i].SPX;
                    answer.SelChar.SPosY[i] = gs.AccountList[Idx].Account.Mob[i].SPY;

                }
            }
            gs.Server.SendPacket(answer);


            Functions.CreateImportItem(gs.AccountList[Idx].Account.Info.AccountName, Cap, 0, true);


            W2Log.Write($"sucess putchar in capsule [{gs.AccountList[Idx].Account.Info.AccountName}]");
            return DBResult.NO_ERROR;
        }


        //Process OutCapsule (Apaga o Selo da Alma e Cria o Char)
        public static DBResult Exec_MSG_DBOutCapsule(DBController gs, pServer GameServer)
        {
            MSG_DBOutCapsule sm = W2Marshal.GetStructure<MSG_DBOutCapsule>(GameServer.RecvPacket.RawBuffer);

            int Slot = sm.Slot;
            int NovoSlot = -1;
            int Idx = gs.GetIndex(sm.Header.ClientId);

            if (Slot < 0 || Slot >= 4)
            {
                W2Log.Write($"err,outcapsule  slot out of range: {gs.AccountList[Idx].Account.Info.AccountName}");
                return DBResult.NO_ERROR;
            }

            for (int i = 0; i < 4; i++)
            {
                if (String.IsNullOrEmpty(gs.AccountList[Idx].Account.Mob[i].Name))
                {
                    NovoSlot = i;
                    break;
                }
            }

            if (NovoSlot < 0 || NovoSlot >= 4)
            {
                W2Log.Write($"err,newcharcapsule  slot out of range: {NovoSlot} - {gs.AccountList[Idx].Account.Info.AccountName}");
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBCNFCapsuleCharacterFail);
                return DBResult.NO_ERROR; ;
            }

            bool ret = gs.CreateChar(sm.Name, gs.AccountList[Idx].Account.Info.AccountName);
            if (ret == false)
            {
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBCNFCapsuleCharacterFail2);
                return DBResult.NO_ERROR;
            }

            MItem item = gs.AccountList[Idx].GetItemPointer(sm.SourType, sm.SourPos);

            int index = item.Effects[0].Value * 256 + item.Effects[1].Value;

            STRUCT_CAPSULE file = new STRUCT_CAPSULE();

            if (!Functions.ReadCapsule(index, out file))
            {
                W2Log.Write($"Exec_MSG_DBOutCapsule error read capsule index: {index}");
                return DBResult.NO_ERROR;
            }

            STRUCT_MOB mob = gs.AccountList[Idx].Account.Mob[NovoSlot];

            if (!String.IsNullOrEmpty(mob.Name))
            {
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);

                W2Log.Write($"err,newchar already charged: { gs.AccountList[Idx].Account.Info.AccountName} / {mob.Name}");

                return DBResult.NO_ERROR;
            }

            gs.AccountList[Idx].Account.Mob[NovoSlot] = file.Mob;
            gs.AccountList[Idx].Account.MobExtra[NovoSlot] = file.Extra;

            for (int i = 0; i < 32; i++)
            {
                gs.AccountList[Idx].Account.Affects[NovoSlot].Affects[i] = STRUCT_AFFECT.Clear();
            }

            ret = gs.SaveAccount(sm.Header.ClientId);
            if (ret == false)
            {
                gs.Server.SendSignal(sm.Header.ClientId, BaseDef._MSG_DBNewCharacterFail);

                W2Log.Write($"err,newchar fail - create file: {sm.Name}");

                return DBResult.NO_ERROR;
            }


            MSG_CNFNewCharacter answer = W2Marshal.CreatePacket<MSG_CNFNewCharacter>(BaseDef._MSG_DBCNFNewCharacter, sm.Header.ClientId);

            for (int i = 0; i < GameBasics.MAXL_ACC_MOB; i++)
            {
                unsafe
                {
                    answer.SelChar.Coin[i] = gs.AccountList[Idx].Account.Mob[i].Coin;
                    for (int x = 0; x < GameBasics.MAXL_EQUIP; x++)
                    {
                        if (x == 0)
                        {
                            int Face = answer.SelChar.Equip[i].Items[x].Index;
                            if (Face == 22 || Face == 23 || Face == 24 || Face == 25 || Face == 32)
                                Face = gs.AccountList[Idx].Account.MobExtra[i].ClassMaster == 0 ? 21 : (ushort)gs.AccountList[Idx].Account.MobExtra[i].MortalFace + 7;

                            answer.SelChar.Equip[i].Items[x].Index = (short)Face;
                            answer.SelChar.Equip[i].Items[x].Effects[0].Code = 43;
                            answer.SelChar.Equip[i].Items[x].Effects[0].Value = 0;
                            answer.SelChar.Equip[i].Items[x].Effects[1].Code = 86;
                            answer.SelChar.Equip[i].Items[x].Effects[1].Value = Functions.GetClassType(gs.AccountList[Idx].Account.Mob[i]);
                            answer.SelChar.Equip[i].Items[x].Effects[2].Code = 28;
                            answer.SelChar.Equip[i].Items[x].Effects[2].Value = (byte)gs.AccountList[Idx].Account.MobExtra[i].Citizen;
                        }



                        answer.SelChar.Equip[i].Items[x] = gs.AccountList[Idx].Account.Mob[i].Equip.Items[x];
                    }

                    answer.SelChar.Exp[i] = (int)gs.AccountList[Idx].Account.Mob[i].Exp;
                    answer.SelChar.Guild[i] = gs.AccountList[Idx].Account.Mob[i].Guild;
                    answer.SelChar.Name[i].Value = gs.AccountList[Idx].Account.Mob[i].Name;
                    answer.SelChar.Score[i] = gs.AccountList[Idx].Account.Mob[i].BaseScore;
                    answer.SelChar.SPosX[i] = gs.AccountList[Idx].Account.Mob[i].SPX;
                    answer.SelChar.SPosY[i] = gs.AccountList[Idx].Account.Mob[i].SPY;
                }
            }

            gs.Server.SendPacket(answer);
            return DBResult.NO_ERROR;
        }


        //Process ItemLog (Log de itens, id e sua quantidade)
        public static DBResult Exec_MSG_DBItemDayLog(DBController gs, pServer GameServer)
        {
            _MSG_SIGNALPARM sm = W2Marshal.GetStructure<_MSG_SIGNALPARM>(GameServer.RecvPacket.RawBuffer);

            int item = sm.parm;

            if (item <= 0 || item >= BaseDef.MAX_ITEMLIST)
            {
                W2Log.Write($"err, itemid {item} invalid", ELogType.CRITICAL_ERROR);
                return DBResult.NO_ERROR;
            }

            gs.ItemDayLog[item].Count++;
            gs.Config.ItemCount++;
            W2Log.SendUpdate();
            return DBResult.NO_ERROR;
        }

 
    }
}