using System;
using System.Runtime.InteropServices;
using W2Open.Common.Utility;

namespace W2Open.Common.GameStructure
{
#pragma warning disable 660,661

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, CharSet = CharSet.Ansi)]
    public struct st_StateAccount
    {
        public Byte PassowordErrorCount;
        public Byte NumericErrorCount;
        //public DB.LockAccount Type;
        public DateTime Tick;
        public static st_StateAccount New()
        {
            return new st_StateAccount
            {
                PassowordErrorCount = 0,
                NumericErrorCount = 0,
                Tick = new DateTime()
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct _0x114 : DBSRVPackets
    {
        public const ushort PacketID = 0x114;

        public MSG_HEADER Header { get; set; }

        public MPosition mPosition;

        public STRUCT_MOB mob;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 208)]
        public byte[] Unk;

        public short Slot;
        public short ClientID;
        public short Weather;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] ShortSkill;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 645)]
        public byte[] Unk2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct _0x364 : DBSRVPackets
    {
        public const ushort PacketID = 0x364;

        public MSG_HEADER Header { get; set; }

        public MPosition mPos;

        public Int16 ClientID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public String Name;

        public Byte ChaosPoints;
        public Byte CurrentKill;
        public UInt16 TotalKill;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public Int16[] Equip;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public st_bAffect0x364[] bAffect;

        public ushort GuildIndex;

        public byte GuildLevel;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] unk1;

        public STRUCT_SCORE mStatus;

        public byte CreateType;

        public Byte SpawnMemberType;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public Byte[] AnctCode;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
        public String Tab;

        public int hold;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, Size = 8, CharSet = CharSet.Ansi)]
    public struct _0x3B9
    {
        public Byte Id;
        public Byte Master;
        public UInt16 Value;
        public Int32 Time;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, Size = 2, CharSet = CharSet.Ansi)]
    public struct st_bAffect0x364
    {
        public Byte Time;
        public Byte Id;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct _0x20F : DBSRVPackets
    {
        public const ushort PacketID = 0x20F;

        public MSG_HEADER Header { get; set; }

        public int Slot;            // 12 a 15	= 4

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string Name;  // 16 a 27	= 12

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Unk1;       // 28 a 31	= 4

        public int ClassInfo;       // 32 a 35	= 4

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct DeleteCharacter : DBSRVPackets
    {
        public const ushort PacketID = 0x211;

        public MSG_HEADER Header { get; set; }

        public int Slot;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public String Name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public String Password;

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct MSG_CNFDeleteCharacter : DBSRVPackets
    {
        public const ushort PacketID = 0x112;

        public MSG_HEADER Header { get; set; }

        public st_CharList cList;

        public static MSG_CNFDeleteCharacter ClearProperty()
        {
            var character = new MSG_CNFDeleteCharacter();

            unsafe
            {
                character.cList = new st_CharList();
            }

            return character;
        }

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct _0x110 : DBSRVPackets
    {
        public const ushort PacketID = 0x110;

        public MSG_HEADER Header;

        public UInt32 Unknown;

        public st_CharList cList;

        MSG_HEADER DBSRVPackets.Header { get => Header; set => Header = value; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct SCharListName
    {
        // Atributos
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string NameBytes;  // 0 a 15	= 16
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct SCharListEquip
    {
        // Atributos
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public MItem[] Slot;  // 0 a 127	= 128
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct st_CharList
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public short[] PosX;            // 4 a 11			= 8

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public short[] PosY;            // 12 a 19		= 8

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public SCharListName[] Name;    // 20 a 83		= 64

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public STRUCT_SCORE[] Status;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public SCharListEquip[] Equips;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public ushort[] GuildIndex;             // 788 a 795	= 8

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public int[] Gold;              // 796 a 811	= 16

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ulong[] Exp;             // 812 a 843	= 32


        public static st_CharList ClearProperty()
        {
            var Char = new st_CharList();

            Char.PosX = new short[4];
            Char.PosY = new short[4];

            Char.Name = new SCharListName[4];
            Char.Status = new STRUCT_SCORE[4];
            Char.Equips = new SCharListEquip[4];
            Char.GuildIndex = new ushort[4];

            Char.Gold = new int[4];
            Char.Exp = new ulong[4];

            return Char;

        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, Size = 8, CharSet = CharSet.Ansi)]
    public struct STRUCT_ITEM
    {
        public Int16 Id;
        public Byte EF1;
        public Byte EFV1;
        public Byte EF2;
        public Byte EFV2;
        public Byte EF3;
        public Byte EFV3;
        public STRUCT_ITEM(Int16 Id, Byte EF1 = 0, Byte EFV1 = 0, Byte EF2 = 0, Byte EFV2 = 0, Byte EF3 = 0, Byte EFV3 = 0)
        {
            this.Id = Id;
            this.EF1 = EF1;
            this.EFV1 = EFV1;
            this.EF2 = EF2;
            this.EFV2 = EFV2;
            this.EF3 = EF3;
            this.EFV3 = EFV3;
        }

        public static STRUCT_ITEM New()
        {
            return new STRUCT_ITEM
            {
                Id = new Int16(),
                EF1 = new Byte(),
                EFV1 = new Byte(),
                EF2 = new Byte(),
                EFV2 = new Byte(),
                EF3 = new Byte(),
                EFV3 = new Byte()
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK/*, Size = 2, CharSet = CharSet.Ansi*/)]
    public struct STRUCT_ATTACKRUN
    {
        public sbyte MotionSpeed;
        public sbyte AttackSpeed;
    };
    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK/*, Size = 2, CharSet = CharSet.Ansi*/)]
    public struct STRUCT_MERCHANT
    {
        public sbyte Merchant;
        public sbyte Direction;
 
    };
    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK/*, Size = 48, CharSet = CharSet.Ansi*/)]
    public struct STRUCT_SCORE
    {
        public int Level;   // The mob's level
        public int Ac;      // The mob's defense
        public int Damage;   // The mob's damage force
 
        public STRUCT_MERCHANT Merchant;
        public STRUCT_ATTACKRUN Speed;

        public int MaxHp;     // The max HP the mob can have
        public int MaxMp;    // The max MP the mob can have
        public int Hp;       // The current HP of the mob
        public int Mp;       // The current MP of the mob

        public short Str;        // The mob's strength points, affects it's attack power
        public short Int;        // The mob's intelligence points, affects it's skill attack powers and MP
        public short Dex;        // The mob's dexterity points, affects it's attack speed
        public short Con;       // The mob's constitution points, affects it's HP

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public short[] Special; // The mob's special points, affects it's skill tiers
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct STRUCT_MOB
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string Name;

        public byte Clan;
        public byte Merchant;
        public ushort Guild;
        public sbyte Class;
        public short Rsv;
        public byte Quest;

        public int Coin;


        public long Exp;

        public short SPX;
        public short SPY;

        public MScore BaseScore;
        public MScore CurrentScore;

        public MEquip Equip;
        public MCarry Carry;

        public int LearnedSkill;
        public int nLearnedSkill;

        public short ScoreBonus;
        public short SpecialBonus;
        public short SkillBonus;

        public byte Critical;
        public byte SaveMana;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public sbyte[] SkillBar;

        public byte GuildLevel;

        public byte Magic;

        public byte RegenHP;
        public byte RegenMP;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public sbyte[] Resist;

        public MScore SubScore;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
        public string TabName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string GuildName;

        public int NextMovement;

        public fixed sbyte NewBytes[108];

        public unsafe static STRUCT_MOB ClearProperty()
        {
            var MOB = new STRUCT_MOB();

            MOB.Equip.Items = new MItem[16];
            
            MOB.Carry.Items = new MItem[64];


            MOB.SkillBar = new sbyte[4];
            MOB.Resist = new sbyte[4];
            MOB.BaseScore = new MScore();
            MOB.SubScore = new MScore();
            MOB.SubScore.Special = new short[4];
            MOB.BaseScore = MScore.Clear();
            MOB.Coin = 0;
            MOB.Name = "";
            MOB.TabName = "";
            MOB.GuildName = "";
            MOB.BaseScore.Special = new short[4];
           
            for (int i = 0; i < 108; i++)
                MOB.NewBytes[i] = 0;

            MOB.SPX = 2112;
            MOB.SPY = 2112;
            for (int i = 0; i < MOB.Carry.Items.Length; i++)
            {
                if (i < 4)
                {
                    MOB.SkillBar[i] = 0;
                    MOB.Resist[i] = 0;
                }
                if (i < 16)
                {
                    MOB.Equip.Items[i] = MItem.Empty();
                }
                if (i < 64)
                    MOB.Carry.Items[i] = MItem.Empty();
                if (i < 4)
                {
                    MOB.SkillBar[i] = -1;

                    if (i < MOB.Equip.Items.Length)
                    {
                        MOB.Equip.Items[i].Effects = new MItem_Effects[3];
                    }
                    MOB.Carry.Items[i].Effects = new MItem_Effects[3];
                }
            }

            MOB.CurrentScore = MOB.BaseScore;
            return MOB;
        }

        public unsafe static STRUCT_MOB ClearProperty(string byName)
        {
            var MOB = new STRUCT_MOB();

            MOB.Name = byName;
            MOB.Equip = new MEquip();
            MOB.Carry = new MCarry();
            MOB.SkillBar = new sbyte[4];
            MOB.Resist = new sbyte[4];
            MOB.CurrentScore = new MScore();
            MOB.Coin = 0;
            for (int i = 0; i < MOB.Carry.Items.Length; i++)
            {
                if (i < 4)
                {
                    MOB.SkillBar[i] = -1;

                    if (i < MOB.Equip.Items.Length)
                    {
                        MOB.Equip.Items[i].Effects = new MItem_Effects[3];
                    }
                    MOB.Carry.Items[i].Effects = new MItem_Effects[3];
                }
            }

            MOB.CurrentScore = MOB.BaseScore;
            return MOB;
        }
    };



    [StructLayout(LayoutKind.Sequential)]
    public struct STRUCT_NPC
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string Name;
        public byte Clan;
        public byte Merchant;
        public short Guild;
        public sbyte Class;
        public short Rsv;
        public byte Quest;

        public int Coin;

        public long Exp;

        public short SPX;
        public short SPY;

        public STRUCT_SCORE BaseScore;
        public STRUCT_SCORE CurrentScore;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public STRUCT_ITEM[] Equip;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public STRUCT_ITEM[] Carry;

        public long LearnedSkill;

        public short ScoreBonus;
        public short SpecialBonus;
        public short SkillBonus;

        public byte Critical;
        public byte SaveMana;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public sbyte[] SkillBar;

        public byte GuildLevel;

        public short Magic;

        public short RegenHP;
        public short RegenMP;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public sbyte[] Resist;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, CharSet = CharSet.Ansi)]
    public struct STRUCT_FIXITEM
    {
        public String Name;
        public Int16 Value;
        public static STRUCT_FIXITEM New()
        {
            return new STRUCT_FIXITEM
            {
                Name = String.Empty,
                Value = 0
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, CharSet = CharSet.Ansi)]
    public struct STRUCT_ITEMLIST
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public String Name;
        public Int16 Mesh1;
        public Int32 Mesh2;
        public Int16 Level;
        public Int16 Str;
        public Int16 Int;
        public Int16 Dex;
        public Int16 Con;
        public UInt32 Unique;
        public UInt32 Price;
        public UInt16 Pos;
        public Int16 Extreme;
        public Int16 Grade;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public STRUCT_FIXITEM[] EFItem;
        public static STRUCT_ITEMLIST New()
        {
            STRUCT_ITEMLIST iList = new STRUCT_ITEMLIST();
            iList.Name = String.Empty;
            iList.Mesh1 = 0;
            iList.Mesh2 = 0;
            iList.Level = 0;
            iList.Str = 0;
            iList.Int = 0;
            iList.Dex = 0;
            iList.Con = 0;
            iList.Unique = 0;
            iList.Price = 0;
            iList.Pos = 0;
            iList.Extreme = 0;
            iList.Grade = 0;
            iList.EFItem = new STRUCT_FIXITEM[12];
            for (SByte loop = 0; loop < 12; loop++)
                iList.EFItem[loop] = STRUCT_FIXITEM.New();
            return iList;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, Size = 240, CharSet = CharSet.Ansi)]
    public struct STRUCT_ACCOUNTINFO
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public String AccountName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public String AccountPass;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public String RealName;
        public Int32 SSN1;
        public Int32 SSN2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
        public String Email;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public String Telephone;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 78)]
        public String Address;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public sbyte[] NumericToken;
        public int Year;
        public int YearDay;
    }

    [StructLayout(LayoutKind.Sequential/*, Pack = Defines.DEFAULT_PACK*/)]
    public struct STRUCT_AFFECT
    {
         
    
        public byte Type;
        public byte Value;
        public short Level;
        public int Time;

        public unsafe static STRUCT_AFFECT Clear()
        {
            var temp = new STRUCT_AFFECT();
            temp.Type = 0;
            temp.Value = 0;
            temp.Level = 0;
            temp.Time = 0;
            
            return temp;
        }
    };

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, Size = 256, CharSet = CharSet.Ansi)]
    public struct _STRUCT_AFFECT
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public STRUCT_AFFECT[] Affect;
    };

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, Size = 16, CharSet = CharSet.Ansi)]
    public struct STRUCT_SKILLBAR
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public sbyte[] ShortSkill;

        public static STRUCT_SKILLBAR Clear()
        {
            var temp = new STRUCT_SKILLBAR();
            temp.ShortSkill = new sbyte[16];
            unsafe
            {
                for (int i = 0; i < 16; i++)
                    temp.ShortSkill[i] = -1;
            }
            return temp;
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct STRUCT_MORTAL
    {
        public sbyte Newbie;//00_01_02_03_04  quest com quatro etapas
        public sbyte TerraMistica;//0 : não pegou a quest 1: pegou a quest e não concluiu 2: quest completa
        public sbyte MolarGargula;
        public sbyte PilulaOrc;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        //public sbyte[] EMPTY;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct STRUCT_ARCH
    {

        public sbyte MortalSlot;
        public sbyte MortalLevel;

        public sbyte Level355;
        public sbyte Level370;

        public short Cristal;//00_01_02_03_04 quest com quatro etapas

        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        //public sbyte[] EMPTY;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct STRUCT_CELESTIAL
    {

        public short ArchLevel;
        public short CelestialLevel;
        public short SubCelestialLevel;

        public sbyte Lv40;
        public sbyte Lv90;

        public sbyte Add120;
        public sbyte Add150;
        public sbyte Add180;
        public sbyte Add200;

        public sbyte Arcana;
        public sbyte Reset;
        public short Amunra;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
        //public sbyte[] EMPTY;
    };

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, Size = 144, CharSet = CharSet.Ansi)]
    public struct STRUCT_QUESTINFO2
    {
        public STRUCT_MORTAL Mortal;
        public STRUCT_ARCH Arch;
        public STRUCT_CELESTIAL Celestial;
        public sbyte Circle;
        public Int16 Rsv;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
        //public sbyte[] EMPTY;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct STRUCT_SAVEONCELESTIAL
    {

        public int Class;

        public long Exp;            // The ammount of experience the mob has to level up

        public short SPX;           // The Y position saved by the stellar gem, to teleport the mob there when using warp scroll
        public short SPY;           // The Y position saved by the stellar gem, to teleport the mob there when using warp scroll

        public STRUCT_SCORE BaseScore;    // The base score of the mob 

        public int LearnedSkill; // The skills the mob learned, divided into four categories (00 _ 00 _ 00 _ 00)
        public int nLearnedSkill; // The skills the mob learned, divided into four categories (00 _ 00 _ 00 _ 00)


        public short ScoreBonus;   // The points the mob can use to increase score (Str, Int, Dex, Con)
        public short SpecialBonus; // The points the mob can use to increase special, to increase effect of learned skills (score->Special[4])
        public short SkillBonus;   // The points the mob can use to buy skills

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public sbyte[] SkillBar;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public sbyte[] SkillBar2;
        public short Soul;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        //public sbyte[] EMPTY;
    };

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK)]
    public struct STRUCT_SAVECELESTIAL
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public STRUCT_SAVEONCELESTIAL[] SaveCelestial;
    };

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, Size = 12, CharSet = CharSet.Ansi)]
    public struct STRUCT_DAYLOG
    {

        public Int64 Exp;
        public Int32 YearDay;
    };

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, Size = 12, CharSet = CharSet.Ansi)]
    public struct STRUCT_DONATEINFO
    {

        public Int64 LastTime;
        public Int32 Count;
    };

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, CharSet = CharSet.Ansi)]
    public struct STRUCT_QUESTINFO
    {
        public STRUCT_QUESTINFO2 QuestInfo;
    };

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct STRUCT_MOBEXTRA
    {
        public short ClassMaster;//ok
        public sbyte Citizen;//ok
        public int Fame;//ok
        public short Soul;//ok
        public short MortalFace;//ok

    
        public STRUCT_MORTAL Mortal;//ok - 4
        public STRUCT_ARCH Arch;//ok 6
        public STRUCT_CELESTIAL Celestial;//ok 16
        public sbyte Circle;//ok
        public short Rsv;//ok

        
        public STRUCT_SAVEONCELESTIAL SaveCelestial;//ok 188
        public STRUCT_SAVEONCELESTIAL SaveCelestial2;

        public long LastNT;//8
        public int NT;
        public int KefraTicket;
        public long DivineEnd;//8
        public long Hold;

        public long Exp;
        public int ExYearDay;

        public long DonateLastTime;
        public int DonateCount;

        public long SephiraEnd;//8
        public long HealthEnd;//8
        public short SPX;
        public short SPY;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 248)]
        public sbyte[] skill;
        public sbyte CriticalMago;

        //public fixed sbyte EMPTY[70];


        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        public sbyte[] EMPTY;
        public unsafe static STRUCT_MOBEXTRA Empty()
        {
            STRUCT_MOBEXTRA temp = new STRUCT_MOBEXTRA();
            

            temp.skill = new sbyte[248];
            temp.SaveCelestial = new STRUCT_SAVEONCELESTIAL();
            temp.SaveCelestial2 = new STRUCT_SAVEONCELESTIAL();
            temp.Celestial = new STRUCT_CELESTIAL();
             
            temp.EMPTY = new sbyte[30];
            temp.SaveCelestial.BaseScore.Special = new short[4];
            temp.SaveCelestial2.BaseScore.Special = new short[4];
            temp.SaveCelestial.SkillBar = new sbyte[4];
            temp.SaveCelestial.SkillBar2 = new sbyte[16];
            temp.SaveCelestial2.SkillBar = new sbyte[4];
            temp.SaveCelestial2.SkillBar2 = new sbyte[16];
            for (int i = 0; i<248;i++)
            {
                if (i < 30)
                    temp.EMPTY[i] = 0;

                if(i < 16)
                {
                    if(i<4)
                    {
                        temp.SaveCelestial.SkillBar[i] = -1;
                        temp.SaveCelestial2.SkillBar[i] = -1;

                    }
                  
                    temp.SaveCelestial.SkillBar2[i] = -1;
                    temp.SaveCelestial2.SkillBar2[i] = 1;
                }

                temp.skill[i] = 0;
            }
            


            return temp;
        }
    };

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, Size = 2, CharSet = CharSet.Ansi)]
    public struct STRUCT_WARPINFO
    {
        sbyte concluida;//concluio a quest
        sbyte receive;//recebeu a quest
    };

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, Size = 14, CharSet = CharSet.Ansi)]
    public struct STRUCT_DAYS
    {
        Int16 reputation;//reputação que ganha nas quests
        sbyte CompleteMes;//completou todas as quest do mes
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public STRUCT_WARPINFO[] WarpInfo;
    };

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, CharSet = CharSet.Ansi)]
    public struct STRUCT_OTHER
    {
        public int NewInfo;
    };


    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, CharSet = CharSet.Ansi)]
    public struct MSTRUCT_AFFECT
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = GameBasics.MAXL_AFFECT)]
        public STRUCT_AFFECT[] Affects;

        public unsafe static MSTRUCT_AFFECT Clear()
        {
            var temp = new MSTRUCT_AFFECT();
            temp.Affects = new STRUCT_AFFECT[32];
            for (int i = 0; i < 32; i++)
            {
                temp.Affects[i] = STRUCT_AFFECT.Clear();
            }
            return temp;
        }
    };

    [StructLayout(LayoutKind.Sequential/*, Pack = Defines.DEFAULT_PACK*/, CharSet = CharSet.Ansi)]
    public struct STRUCT_ACCOUNTFILE
    {
        public STRUCT_ACCOUNTINFO Info;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public STRUCT_MOB[] Mob;

        public MCargo Cargo;
        public int Coin;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public STRUCT_SKILLBAR[] Skillbar;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public MSTRUCT_AFFECT[] Affects;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public STRUCT_MOBEXTRA[] MobExtra;

        public STRUCT_OTHER other;


        public int Donate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
        public sbyte[] TempKey;

        public unsafe static STRUCT_ACCOUNTFILE ClearProperty()
        {
            var acc = new STRUCT_ACCOUNTFILE();
            acc.Info = new STRUCT_ACCOUNTINFO();
            acc.Mob = new STRUCT_MOB[GameBasics.MAXL_ACC_MOB];
            acc.MobExtra = new STRUCT_MOBEXTRA[GameBasics.MAXL_ACC_MOB];
            acc.Skillbar = new STRUCT_SKILLBAR[GameBasics.MAXL_ACC_MOB];
            acc.Affects = new MSTRUCT_AFFECT[GameBasics.MAXL_ACC_MOB];
            acc.Cargo = new MCargo();
            acc.other = new STRUCT_OTHER();
            acc.Info.AccountName = "";
            acc.Info.AccountPass = "";
            acc.Info.Address = "";
            acc.Info.Email = "";
            acc.Info.RealName = "";
            acc.TempKey = new sbyte[36];
            acc.Cargo = MCargo.Clear();
            acc.Info.NumericToken = new sbyte[6];
            acc.other = new STRUCT_OTHER();
            for (int i = 0; i < 4; i++)
            {
                acc.Mob[i] = STRUCT_MOB.ClearProperty();
                acc.MobExtra[i] = STRUCT_MOBEXTRA.Empty();
                acc.Skillbar[i] = STRUCT_SKILLBAR.Clear();
                acc.Affects[i] = MSTRUCT_AFFECT.Clear();
            }

            return acc;
        }


        public int getEmptyCargo()
        {
            for (int i = 0; i< 120;i++)
            {
                if (Cargo.Items[i].Index == 0)
                    return i;
            }

            return -1;
        }
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct STRUCT_GUILDCONFIG
    {
        public sbyte GuildLevel;
        public int GuildExp;
        public sbyte Segment;
        public sbyte MaxMemberCount; //Maximo 150 começa com 15
        public sbyte MemberCount;

    };

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK)]
    public struct STRUCT_GUILDSUB
    {
        public short Index;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public String Name;
        public static STRUCT_GUILDSUB Empty()
        {
            STRUCT_GUILDSUB temp = new STRUCT_GUILDSUB();

            temp.Index = 0;
            temp.Name = "";

            return temp;
        }

    };

    [StructLayout(LayoutKind.Sequential/* ,Pack = 4*/)]
    public struct MScore
    {
        public int Level;
        public int Defesa;
        public int Ataque;

        public sbyte Merchant;
        public sbyte City;
        public sbyte MoveSpeed;
        public sbyte AttackSpeed;

        public int MaxHP;
        public int MaxMP;
        public int HP;
        public int MP;

        public short Str;
        public short Int;
        public short Dex;
        public short Con;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public short[] Special;

        public unsafe static MScore Clear()
        {
            MScore temp = new MScore();
            temp.Special = new short[4];
            temp.Level = 0;
            temp.Defesa = 0;
            temp.Ataque = 0;

   
           
            temp.MaxHP = 0;
            temp.MaxMP = 0;
            temp.HP = 0;
            temp.MP = 0;

            temp.Str = 0;
            temp.Int = 0;
            temp.Dex = 0;
            temp.Con = 0;
            for (int i = 0; i < 4; i++)
                temp.Special[i] = 0;

            return temp;
        }
    };

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK)]
    public struct MMobPointsLeft
    {
        public short Status; // Amount of status points the mob have to distribute.
        public short Special; // Amount of special points the mob have to distribute.
        public short Skill; // Amount of skill points the mob have to distribute.
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK)]
    public struct MMobName
    {
        public const int MAXL_NAME = 16;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXL_NAME)]
        public String Value;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK)]
    public struct MItem
    {
        public short Index;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = GameBasics.MAXL_ITEM_EFFECT)]
        public MItem_Effects[] Effects;

        //public MItem Items { get; internal set; }

        static unsafe public MItem Empty()
        {
            MItem temp = new MItem();

            temp.Index = 0;
            temp.Effects = new MItem_Effects[3];
            for (int i = 0; i < temp.Effects.Length; i++)
            {
                temp.Effects[i].Code = 0;
                temp.Effects[i].Value = 0;
            }

            return temp;
        }
    };

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK)]
    public struct MItem_Effects
    {
        public byte Code;
        public byte Value;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK)]
    public struct MEquip
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = GameBasics.MAXL_EQUIP)]
        public MItem[] Items;

        static unsafe public MItem Empty()
        {
            MItem temp = new MItem();

            temp.Index = 0;
            temp.Effects = new MItem_Effects[3];
            for (int i = 0; i < temp.Effects.Length; i++)
            {
                temp.Effects[i].Code = 0;
                temp.Effects[i].Value = 0;
            }

            return temp;
        }



    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct MLoginInfo
    {
        public const int MAXL_ACCNAME = 16;
        public const int MAXL_PSW = 16;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXL_ACCNAME)]
        public String AccName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXL_PSW)]
        public String Password;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct _MLoginInfo
    {
        public const int MAXL_ACCNAME = 16;
        public const int MAXL_PSW = 12;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXL_PSW)]
        public String Pass;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXL_ACCNAME)]
        public String Name;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK)]
    public struct MAccountInfo
    {
        public const int MAXL_REALNAME = 24;
        public const int MAXL_EMAIL = 48;
        public const int MAXL_TELEPHONE = 16;
        public const int MAXL_ADDRESS = 78;
        public const int MAXL_NUMTOKEN = 6;

        public MLoginInfo LoginInfo;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXL_REALNAME)]
        public String RealName;

        public int SSN1;
        public int SSN2;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXL_EMAIL)]
        public String Email;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXL_TELEPHONE)]
        public String Telephone;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXL_ADDRESS)]
        public String Address;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXL_NUMTOKEN)]
        public String NumericToken;

        public int Year;
        public int YearDay;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK)]
    public struct MAffect
    {
        public byte Type;
        public byte Value;
        public ushort Level;
        public uint Time;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK)]
    public struct MPosition
    {
        public short SPX;
        public short SPY;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK)]
    public struct MCarry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = GameBasics.MAXL_INVENTORY)]
        public MItem[] Items;

        //public unsafe static MCarry Clear()
        //{
        //    MCarry temp = new MCarry();
        //    temp.Items = new MItem[GameBasics.MAXL_INVENTORY];
        //    for (int i = 0; i < GameBasics.MAXL_INVENTORY; i++)
        //    {
        //        temp.Items[i] = MItem.Empty();
        //    }
        //    return temp;
        //}
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK)]
    public struct MCargo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = GameBasics.MAXL_CARGO_ITEM)]
        public MItem[] Items;

        public static MCargo Clear()
        {
            MCargo temp = new MCargo();
            temp.Items = new MItem[GameBasics.MAXL_CARGO_ITEM];
            for (int i = 0; i < GameBasics.MAXL_CARGO_ITEM; i++)
            {
                temp.Items[i] = MItem.Empty();
            }
            return temp;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK)]
    public struct MSelChar
    {
        public unsafe fixed short SPosX[GameBasics.MAXL_ACC_MOB];
        public unsafe fixed short SPosY[GameBasics.MAXL_ACC_MOB];

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = GameBasics.MAXL_ACC_MOB)]
        public MMobName[] Name;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = GameBasics.MAXL_ACC_MOB)]
        public MScore[] Score;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = GameBasics.MAXL_ACC_MOB)]
        public MEquip[] Equip;

        public unsafe fixed ushort Guild[GameBasics.MAXL_ACC_MOB];

        public unsafe fixed int Coin[GameBasics.MAXL_ACC_MOB];
        public unsafe fixed int Exp[GameBasics.MAXL_ACC_MOB];


        static public MSelChar Empty()
        {
            MSelChar p = new MSelChar();

            unsafe
            {
                for (int i = 0; i < GameBasics.MAXL_ACC_MOB; i++)
                {

                    p.Coin[i] = 0;
                    p.Exp[i] = 0;
                    p.Guild[i] = 0;
                    p.SPosX[i] = 0;
                    p.SPosY[i] = 0;
                }
            }
            return p;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = Defines.DEFAULT_PACK, Size = 120, CharSet = CharSet.Ansi)]
    public struct STRUCT_GUILDINFO
    {
        public Int32 GuildID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public String GuildName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public String GuildMark;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public String Lider;
        public Int16 Fama;
        public sbyte Reino;
        public sbyte Cidadania;

        public STRUCT_GUILDCONFIG Config;


        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public STRUCT_GUILDSUB[] SubLiders;


        public static STRUCT_GUILDINFO Empty()
        {
            STRUCT_GUILDINFO temp = new STRUCT_GUILDINFO();
            STRUCT_GUILDSUB[] Subs = new STRUCT_GUILDSUB[3];
            temp.GuildName = "";
            temp.Lider = "";
            unsafe
            {
                for (int i = 0; i < 3; i++)
                {
                    Subs[i] = STRUCT_GUILDSUB.Empty();
                }

                temp.SubLiders = Subs;
            }


            return temp;
        }
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct MAccountLoginPacket : DBSRVPackets
    {
        public const ushort Opcode = 0x20D;
        public MSG_HEADER Header { get; set; }
        public _MLoginInfo Login;
        public unsafe fixed byte Zero[24];
        public unsafe fixed byte CheckSun[2];
        public int LastUpdate;
        public int CallCont;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
        public String MacAddr;
        public int DBNeedSave;
        public unsafe fixed int AdapterName[4];
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct _MSG_DBCharacterLogin2 : DBSRVPackets
    {
        public const ushort Opcode = 0x804;
        public MSG_HEADER Header { get; set; }
        public int Slot;
        public int Force;
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    //[StructLayout(LayoutKind.Sequential/*, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK*/)]
    public struct _MSG_DBSaveMob : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }
        public int Slot;
        public STRUCT_MOB Mob;
        public MCargo Cargo;
        public int Coin;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public sbyte[] ShortSkill;
        public MMobName Accountname;
        public int Export;
        public STRUCT_MOBEXTRA Extra;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = GameBasics.MAXL_AFFECT)]
        public STRUCT_AFFECT[] Affects;
        public int Donate;
        public STRUCT_OTHER Other;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct MLoginSuccessfulPacket : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }

        public unsafe fixed sbyte HashKeyTable[16];

        public int Offset_28; // TODO: unknown!

        public MSelChar SelChar;

        public MCargo Cargo;

        public int CargoCoin;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MLoginInfo.MAXL_ACCNAME)]
        public String AccName;

        public unsafe fixed sbyte Keys[12];
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct MSG_ReqTransper : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }
        public int Result;
        public int Slot;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MLoginInfo.MAXL_ACCNAME)]
        public String OldName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MLoginInfo.MAXL_ACCNAME)]
        public String NewName;

    }

    [StructLayout(LayoutKind.Sequential/*, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK*/)]
    public struct MSG_DBSendItem : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }
        public int id_pedido;
        public int Result;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MLoginInfo.MAXL_ACCNAME)]
        public String Account;
        public MItem Items;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct STMSG_CreateCharacter : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }
        public int Slot;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MLoginInfo.MAXL_ACCNAME)]
        public String PlayerName;
        public int MobClass;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct MSG_DBCreateArchCharacter : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MLoginInfo.MAXL_ACCNAME)]
        public String PlayerName;
        public int MobClass;
        public int MortalFace;
        public int MortalSlot;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct STMSG_DeleteCharacter : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }
        public int Slot;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MLoginInfo.MAXL_ACCNAME)]
        public String PlayerName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MLoginInfo.MAXL_PSW)]
        public String Pass;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct MSG_CNFNewCharacter : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }
        public int Slot;
        public MSelChar SelChar;
    }

    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    //public struct _MSG_CNFCharacterLogin : DBSRVPackets
    //{
    //    public const ushort Opcode = 0x417;
    //    public MSG_HEADER Header { get; set; }
    //    public short PosX, PosY;
    //    public STRUCT_MOB Mob;
    //    public unsafe fixed byte unk[208];
    //    public ushort Slot;
    //    public ushort ClientID;
    //    public ushort Weather;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    //    public sbyte[] ShortSkill;
    //    public unsafe fixed byte Unk[2];
    //    public unsafe fixed byte Unk2[766];
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = GameBasics.MAXL_AFFECT)]
    //    public STRUCT_AFFECT[] AccountAffect;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = GameBasics.MAXL_AFFECT)]
    //    public STRUCT_AFFECT[] Affects;
    //    public STRUCT_MOBEXTRA mobExtra;
    //    public int Donate;
    //    public unsafe fixed int AdapterName[4];
    //}
    [StructLayout(LayoutKind.Sequential/*, CharSet = CharSet.Ansi, Pack = 8*/)]
    public unsafe struct _MSG_CNFCharacterLogin : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }
        public short PosX, PosY;
        public STRUCT_MOB Mob;//0k 1056
        public short Slot;
        public short ClientID;
        public short Weather;//1062
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public sbyte[] ShortSkill;//1078 - ok

        public fixed byte Unk1[448];//1526 - ok

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string AccountName;//1542 - ok
 
        public fixed byte Unk2[560];//2102

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = GameBasics.MAXL_AFFECT)]
        public STRUCT_AFFECT[] Affects;//2358

        public STRUCT_MOBEXTRA mobExtra;
        
    }



    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    //public struct _MSG_CNFCharacterLogin : DBSRVPackets
    //{
    //    public MSG_HEADER Header { get; set; }
    //    public short PosX, PosY;
    //    public STRUCT_MOB Mob;
    //    public STRUCT_MOBEXTRA mobExtra;
    //    public short Slot;
    //    public short ClientID;
    //    public short Weather;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    //    public sbyte[] ShortSkill;

    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    //    public string AccountName;

    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = GameBasics.MAXL_AFFECT)]
    //    public STRUCT_AFFECT[] Affects;

    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    //    public sbyte[] Unk2;

    //}


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct STRUCT_SEALOFSOUL
    {
        public int Read;
        public int MortalClass;
        public int ClassCele;
        public int SubClass;
        public int LevelCele;
        public int LevelSub;
        public int For;
        public int Int;
        public int Dex;
        public int Con;
        public int ScoreBonus;
        public int SkillPoint;
        public int ArchQuest;
        public int CelestialQuest;
        public int ArcanaQuest;

        static public STRUCT_SEALOFSOUL Empty()
        {
            STRUCT_SEALOFSOUL temp = new STRUCT_SEALOFSOUL();

            temp.Read = 0;
            temp.MortalClass = 0;
            temp.ClassCele = 0;
            temp.SubClass = 0;
            temp.LevelCele = 0;
            temp.LevelSub = 0;
            temp.For = 0;
            temp.Int = 0;
            temp.Dex = 0;
            temp.Con = 0;
            temp.ScoreBonus = 0;
            temp.SkillPoint = 0;
            temp.ArchQuest = 0;
            temp.CelestialQuest = 0;
            temp.ArcanaQuest = 0;

            return temp;
        }
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct MSG_CNFDBCapsuleInfo : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }
        public int Index;
        public STRUCT_SEALOFSOUL Capsule;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct MSG_DBOutCapsule : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }
        public int Slot;
        public int SourType;
        public int SourPos;
        public int DestType;
        public int DestPos;
        public ushort GridX, GridY;
        public ushort WarpID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string Name;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct STRUCT_CAPSULE
    {
        public STRUCT_MOB Mob;
        public STRUCT_MOBEXTRA Extra;

        static public STRUCT_CAPSULE Empty()
        {
            STRUCT_CAPSULE temp = new STRUCT_CAPSULE();

            temp.Mob = STRUCT_MOB.ClearProperty();
            temp.Extra = new STRUCT_MOBEXTRA();

            return temp;
        }
        public static bool operator ==(STRUCT_CAPSULE op1, STRUCT_CAPSULE op2)
        {
            return op1.Equals(op2);
        }

        public static bool operator !=(STRUCT_CAPSULE op1, STRUCT_CAPSULE op2)
        {
            return !op1.Equals(op2);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct stChargedGuildList
    {

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public int[] Guild;


        public stChargedGuildList(bool v)
        {
            Guild = new int[5];
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public unsafe struct _MSG_GuildReport : DBSRVPackets
    {
        public const ushort PacketID = (39 | BaseDef.FLAG_DB2GAME);
        public MSG_HEADER Header { get; set; }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BaseDef.MAX_CHANNEL)]
        public stChargedGuildList[] ChargedGuildList;



    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct MSG_GuildInfo : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }
        public int Guild;
        public STRUCT_GUILDINFO GuildInfo;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct MSG_MessageDBRecord : DBSRVPackets
    {
        public MSG_HEADER Header { get; set; }
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public String Record;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct _MSG_GuildZoneReport : DBSRVPackets
    {
        public const ushort PacketID = (13 | BaseDef.FLAG_GAME2DB);
        public MSG_HEADER Header { get; set; }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public int[] Guild;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = Defines.DEFAULT_PACK)]
    public struct MTextMessagePacket : DBSRVPackets
    {
        public const ushort PacketID = 0x101;

        public MSG_HEADER Header { get; set; }

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 96)]
        public String Message;

        public static MTextMessagePacket Create(String msg)
        {
            MTextMessagePacket packet = W2Marshal.CreatePacket<MTextMessagePacket>(PacketID);
            packet.Message = msg;
            return packet;
        }
    }
}