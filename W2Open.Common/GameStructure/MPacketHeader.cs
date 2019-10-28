using System.Runtime.InteropServices;

namespace W2Open.Common.GameStructure
{
    /// <summary>
    /// All the packet structures must implement this interface.
    /// </summary>
    public interface DBSRVPackets
    {
        MSG_HEADER Header { get; set; }
    }

    /// <summary>
    /// Header present in all the valid game packets.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK)]
    public struct MSG_HEADER
    {
        public ushort Size; // 1

        public byte Key; // 2
        public byte CheckSum; // 3

        public ushort PacketID;
        public ushort ClientId;

        public uint TimeStamp;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct _MSG_SIGNAL : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct _MSG_SIGNALPARM : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }
        public int parm;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct _MSG_SIGNALPARM2 : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }
        public int parm;
        public int parm2;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct _MSG_SIGNALPARM3 : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }
        public int parm;
        public int parm1;
        public int parm2;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct MPingPacket : DBSRVPackets
    {
        public const ushort PacketID = 0x3A0;

        public MSG_HEADER Header { get; set; }
    }
}