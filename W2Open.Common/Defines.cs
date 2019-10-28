namespace W2Open.Common
{
    /// <summary>
    /// Contains the basic definitions related to the server's network connection.
    /// </summary>
    public static class BaseDef
    {


        /// <summary>
        /// Max packet length in bytes.
        /// </summary>
        public const int MAXL_PACKET = 128000;

        /// <summary>
        /// Code used in the game network protocol to initiate the connection. Every GameServer must send this 4-byte value as the first packet.
        /// </summary>
        public const int INIT_CODE = 0x1F11F311;

        /// <summary>
        /// Max simultaneous connected amount of GameServers.
        /// </summary>
        /// 




        public const int MAX_CHANNEL = 10;
        public const int MAX_GUILDZONE = 5;
        public const int MAX_MAXUSER = 1000;
        public const int MAX_GUILD = 5000;
        public const int MAX_ITEMLIST = 5000;
        public const int FLAG_GAME2CLIENT = 0x0100;
        public const int FLAG_CLIENT2GAME = 0x0200;

        public const int FLAG_DB2GAME = 0x0400;
        public const int FLAG_GAME2DB = 0x0800;

        public const int FLAG_DB2NP = 0x1000;
        public const int FLAG_NP2DB = 0x2000;

        public const int FLAG_NEW = 0x4000;
        //0xDC3
        public const ushort _MSG_DBCapsuleInfo = (60 | FLAG_DB2GAME | FLAG_GAME2DB);
        public const ushort _MSG_DBPutInCapsule = (61 | FLAG_DB2GAME | FLAG_GAME2DB);
        public const ushort _MSG_DBOutCapsule = (62 | FLAG_DB2GAME | FLAG_GAME2DB);
        public const ushort _MSG_CNFDBCapsuleInfo = (31 | FLAG_DB2GAME | FLAG_GAME2DB | FLAG_GAME2CLIENT);
        public const ushort _MSG_DBCNFCapsuleSucess = (18 | FLAG_DB2GAME);
        public const ushort _MSG_DBClientMessage = (19 | FLAG_DB2GAME);
        public const ushort _MSG_DBCNFArchCharacterSucess = (14 | FLAG_DB2GAME);
        public const ushort _MSG_DBCNFArchCharacterFail = (15 | FLAG_DB2GAME);
        public const ushort _MSG_DBCNFCapsuleCharacterFail = (16 | FLAG_DB2GAME);
        public const ushort _MSG_DBCNFCapsuleCharacterFail2 = (17 | FLAG_DB2GAME);
        public const ushort _MSG_DBDeleteCharacter = (9 | FLAG_GAME2DB);
        public const ushort _MSG_DBUpdateSapphire = (10 | FLAG_GAME2DB);
        public const ushort _MSG_DBCNFAccountLogOut = (11 | FLAG_DB2GAME);
        public const ushort _MSG_MessageDBRecord = (12 | FLAG_GAME2DB);
        public const ushort _MSG_DBSavingQuit = (10 | FLAG_DB2GAME);
        public const ushort _MSG_DBCNFCharacterLogin = (23 | FLAG_DB2GAME);
        public const ushort _MSG_DBCNFNewCharacter = (24 | FLAG_DB2GAME);
        public const ushort _MSG_DBCreateArchCharacter = (31 | FLAG_GAME2DB);
        public const ushort _MSG_DBCNFDeleteCharacter = (25 | FLAG_DB2GAME);
        public const ushort _MSG_DBNewAccountFail = (26 | FLAG_DB2GAME);
        public const ushort _MSG_DBCharacterLoginFail = (28 | FLAG_DB2GAME);
        public const ushort _MSG_DBNewCharacterFail = (29 | FLAG_DB2GAME);
        public const ushort _MSG_DBDeleteCharacterFail = (30 | FLAG_DB2GAME);
        public const ushort _MSG_DBAlreadyPlaying = (31 | FLAG_DB2GAME);
        public const ushort _MSG_DBStillPlaying = (32 | FLAG_DB2GAME);
        public const ushort _MSG_DBAccountLoginFail_Account = (33 | FLAG_DB2GAME);
        public const ushort _MSG_DBAccountLoginFail_Pass = (34 | FLAG_DB2GAME);
        public const ushort _MSG_DBSetIndex = (35 | FLAG_DB2GAME);
        public const ushort _MSG_DBAccountLoginFail_Block = (36 | FLAG_DB2GAME);
        public const ushort _MSG_DBAccountLoginFail_Disable = (37 | FLAG_DB2GAME);
        public const ushort _MSG_DBOnlyOncePerDay = (38 | FLAG_DB2GAME);
        public const ushort _MSG_DBMessagePanel = (1 | FLAG_DB2GAME);
        public const ushort _MSG_DBMessageBoxOk = (2 | FLAG_DB2GAME);
        public const ushort _MSG_DBAccountLogin = (3 | FLAG_GAME2DB);
        public const ushort _MSG_DBCharacterLogin = (4 | FLAG_GAME2DB);
        public const ushort _MSG_DBNoNeedSave = (5 | FLAG_GAME2DB);
        public const ushort _MLoginSuccessfulPacket = 0x416;
        public const ushort _MSG_DBSaveMob = (7 | FLAG_GAME2DB);
        public const ushort _MSG_SavingQuit = (6 | FLAG_GAME2DB);
        public const ushort _MSG_TransperCharacter = (169 | FLAG_GAME2CLIENT | FLAG_CLIENT2GAME | FLAG_DB2GAME);
        public const ushort _MSG_ReqTransper = (170 | FLAG_GAME2CLIENT | FLAG_CLIENT2GAME | FLAG_DB2GAME | FLAG_GAME2DB);
        public const ushort _MSG_War = (14 | FLAG_CLIENT2GAME | FLAG_DB2GAME | FLAG_GAME2DB);
        public const ushort _MSG_DBCreateCharacter = (2 | FLAG_GAME2DB);
        public const ushort _MSG_DBSendItem = (15 | FLAG_GAME2DB | FLAG_DB2GAME); // 0xC0F
        public const ushort _MSG_GuildAlly = (18 | FLAG_CLIENT2GAME | FLAG_DB2GAME | FLAG_GAME2DB);
        public const ushort _MSG_GuildInfo = (19 | FLAG_CLIENT2GAME | FLAG_DB2GAME | FLAG_GAME2DB);
        public const ushort _MSG_CNFCharacterLogin = 0x417;
        public const ushort _MSG_DBItemDayLog = (21 | FLAG_GAME2DB);
        public const ushort _MSG_GuildReport = (39 | FLAG_DB2GAME);
    }
}