using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using W2Open.Common;
 
using W2Open.Common.GameStructure;
using W2Open.Common.Utility;

namespace W2Open.GameState
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct STRUCT_ITEMLOG
    {
        public int Count;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct STRUCT_PUSUER
    {
        public short Index { get; set; }
        public short Slot { get; set; }
        public STRUCT_ACCOUNTFILE Account;
        public EServerStatus State { get; set; }

        public STRUCT_PUSUER(EServerStatus x)
        {
            Slot = -1;
            Index = 0;
            Account = new STRUCT_ACCOUNTFILE();
            State = x;
        }

        public MItem GetItemPointer(int type, int pos)
        {
            MItem sour = MItem.Empty();

            if (type == 0)
            {
                if (pos < 0 || pos >= 16)
                    return MItem.Empty();

                sour = Account.Mob[Slot].Equip.Items[pos];
            }
            else if (type == 1)
            {
                if (pos < 0 || pos >= 64)
                    return MItem.Empty();

                sour = Account.Mob[Slot].Carry.Items[pos];
            }
            else if (type == 2)
            {
                if (pos < 0 || pos >= 128)
                    return MItem.Empty();

                sour = Account.Cargo.Items[pos];
            }
            else
                return MItem.Empty();

            if (sour.Index < 0 || sour.Index >= 6500)
                return MItem.Empty();

            return sour;
        }
    };
    /// <summary>
    /// Represents the actual state of the GameServer in the game.
    /// </summary>
    public enum EServerStatus
    {
        /// <summary>
        /// Setted when the system asked to shutdown the GameServer.
        /// </summary>
        CLOSED = 0,
        /// <summary>
        /// Waiting to be inserted in the GameController. The GameServer have just been created and don't sent the INIT_CODE packet yet.
        /// </summary>
        WAITING_TO_LOGIN,
        /// <summary>
        /// Dentro do jogo, na senha numerica
        /// </summary>
        NUMERIC_SCREEN,
        /// <summary>
        /// Setted when the login process have success. The GameServer is in the character selecion step.
        /// </summary>
        SEL_CHAR,
        /// <summary>
        /// Setted when the GameServer picks a character and enters the game world.
        /// </summary>
        AT_WORLD,

        INGAME,
    }

    // System block account
    public enum LockAccount : byte
    {
        UnBlocked = 0,                                                                              // Conta desbloqueada
        Grid_5MP = 1,                                                                               // Bloqueio ao errar a senha mais de 5 vezes
        Grid_5MN = 2,                                                                               // Bloqueio ao errar a senha numérica mais de 3 vezes
        Grid_3H = 4,                                                                                // Bloqueio por 3 horas
        Grid_A = 8,                                                                                 // Bloqueio por 15 dias
        Grid_B = 16,                                                                                // Bloqueio por 30 dias
        Grid_C = 32,                                                                                // Bloqueio por 90 dias
        Grid_X = 64                                                                                 // Bloqueio permanete
    }

    /// <summary>
    /// Represents each connected GameServer.
    /// </summary>
    public class pServer
    {
        public NetworkStream m_Stream;
        public short Index { get; set; }
        public EServerStatus State { get; set; }
        public CCompoundBuffer RecvPacket { get; private set; }
 
        public pServer(NetworkStream _stream)
        {
            m_Stream = _stream;
            Index = -1;
            State = EServerStatus.WAITING_TO_LOGIN;
            RecvPacket = new CCompoundBuffer(BaseDef.MAXL_PACKET);
   
        }
 
        public int GetIndex(int server, int id)
        {
            int ret = server * 1000 + id;
            return ret;
        }

        public pServer()
        {
            Index = -1;
            State = EServerStatus.WAITING_TO_LOGIN;
            RecvPacket = new CCompoundBuffer(BaseDef.MAXL_PACKET);
 
        }

       
        public unsafe byte[] EncryptBuffer<T>(T DataBuffer)
        {
            byte[] pBuffer = null;

            try
            {
                pBuffer = new byte[Marshal.SizeOf(DataBuffer)];

                fixed (byte* ptr = pBuffer)
                {
                    Marshal.StructureToPtr<T>(DataBuffer, new IntPtr(ptr), false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return pBuffer;
        }

        /// <summary>
        /// Send a given game packet to the GameServer.
        /// </summary>
        public void SendPacket<T>(T packet) where T: struct, DBSRVPackets
        {
            if (m_Stream.CanWrite)
            {
                byte[] rawPacket = W2Marshal.GetBytes(packet);

                PacketSecurity.Encrypt(rawPacket, 0);

                m_Stream.Write(rawPacket, 0, rawPacket.Length);

                W2Log.Write(String.Format("O pacote 0x{0:X} foi enviado para o GameServer(Index:{1}).", packet.Header.PacketID, Index), ELogType.NETWORK);
            }
        }


        public void SendSignal(ushort id, ushort signal)
        {
            _MSG_SIGNALPARM packet = W2Marshal.CreatePacket<_MSG_SIGNALPARM>(signal,id);
            SendPacket<_MSG_SIGNALPARM>(packet);
        }
        public void SendSignalParm(ushort id, ushort signal, int parm)
        {
            _MSG_SIGNALPARM packet = W2Marshal.CreatePacket<_MSG_SIGNALPARM>(signal, id);

            packet.parm = parm;
            SendPacket<_MSG_SIGNALPARM>(packet);
        }
        public void SendSignalParm(ushort id, ushort signal, int parm,int parm2)
        {
            _MSG_SIGNALPARM2 packet = W2Marshal.CreatePacket<_MSG_SIGNALPARM2>(signal, id);

            packet.parm = parm;
            packet.parm2 = parm2;
            SendPacket<_MSG_SIGNALPARM2>(packet);
        }
        public void SendSignalParm(ushort id, ushort signal, int parm, int parm2,int parm3)
        {
            _MSG_SIGNALPARM3 packet = W2Marshal.CreatePacket<_MSG_SIGNALPARM3>(signal, id);

            packet.parm = parm;
            packet.parm1 = parm2;
            packet.parm2 = parm3;
            SendPacket<_MSG_SIGNALPARM3>(packet);
        }


    
        #region Static fields
        public static bool IsValidIndex(int index)
        {
            return index >= 1 && index <= BaseDef.MAX_CHANNEL;
        }
        #endregion
    }
}