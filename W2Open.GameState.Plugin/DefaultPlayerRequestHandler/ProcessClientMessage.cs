using System.Threading.Tasks;
using W2Open.Common;
using W2Open.Common.GameStructure;
using W2Open.Common.Utility;
using W2Open.GameState.Plugin.DefaultPlayerRequestHandler.PacketControl;


namespace W2Open.GameState.Plugin.DefaultPlayerRequestHandler
{
    public class ProcessClientMessage : IGameStatePlugin
    {
        public void Install()
        {
            DBController.OnProcessPacket += ProcessPacketControl;
        }

        private DBResult ProcessPacketControl(DBController gs, pServer GameServer)
        {

            MSG_HEADER packet = W2Marshal.GetStructure<MSG_HEADER>(GameServer.RecvPacket.RawBuffer);


            if (0 == (packet.PacketID & BaseDef.FLAG_GAME2DB) || (packet.ClientId < 0) || (packet.ClientId >= 1000))
            {

                W2Log.Write($"err,ProcessDBMessage packet Type:({packet.PacketID}) ID:{packet.ClientId} Size:{packet.Size}");
                return DBResult.PACKET_NOT_HANDLED;
            }



            switch (packet.PacketID)
            {

                case BaseDef._MSG_GuildInfo:
                    return Packet.Exec_MSG_GuildInfo(gs, GameServer);

                case BaseDef._MSG_War:
                    return Packet.Exec_MSG_War(gs, GameServer);

                case BaseDef._MSG_GuildAlly:
                    return Packet.Exec_MSG_GuildAlly(gs, GameServer);

                case _MSG_GuildZoneReport.PacketID:
                    return Packet.Exec_MSG_GuildZoneReport(gs, GameServer);

                case BaseDef._MSG_DBAccountLogin:
                    return Packet.Exec_MSG_DBAccountLogin(gs, GameServer);

                case _MSG_DBCharacterLogin2.Opcode:
                    return Packet.Exec_MSG_DBCharacterLogin2(gs, GameServer);

                case BaseDef._MSG_DBSaveMob:
                    return Packet.Exec_MSG_DBSaveMob(gs, GameServer);

                case BaseDef._MSG_SavingQuit:
                    return Packet.Exec_MSG_SavingQuit(gs, GameServer);

                case BaseDef._MSG_DBSendItem:
                    return Packet.Exec_MSG_DBSendItem(gs, GameServer);

                case BaseDef._MSG_DBCreateCharacter:
                    return Packet.Exec_MSG_DBCreateCharacter(gs, GameServer);

                case BaseDef._MSG_DBDeleteCharacter:
                    return Packet.Exec_MSG_DBCNFDeleteCharacter(gs, GameServer);

                case BaseDef._MSG_DBNoNeedSave:
                    return Packet.Exec_MSG_DBNoNeedSave(gs, GameServer);

                case BaseDef._MSG_DBUpdateSapphire:
                    return Packet.Exec_MSG_DBUpdateSapphire(gs, GameServer);

                case BaseDef._MSG_MessageDBRecord:
                    return Packet.Exec_MSG_MessageDBRecord(gs, GameServer);

                case BaseDef._MSG_DBCreateArchCharacter:
                    return Packet.Exec_MSG_DBCreateArchCharacter(gs, GameServer);

                case BaseDef._MSG_DBCapsuleInfo:
                    return Packet.Exec_MSG_DBCapsuleInfo(gs, GameServer);

                case BaseDef._MSG_DBPutInCapsule:
                    return Packet.Exec_MSG_DBPutInCapsule(gs, GameServer);

                case BaseDef._MSG_DBOutCapsule:
                    return Packet.Exec_MSG_DBOutCapsule(gs, GameServer);

                case BaseDef._MSG_DBItemDayLog:
                    return Packet.Exec_MSG_DBItemDayLog(gs, GameServer);


                default: return DBResult.PACKET_NOT_HANDLED;
            }
        }
        private async Task seTimerPacketControl(DBController gs, pServer GameServer)
        {

            while (true)
            {
                ProcessPacketControl(gs, GameServer);

                await Task.Delay(500);
            }
        }
    }
}