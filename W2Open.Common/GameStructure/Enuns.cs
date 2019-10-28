using System;
using System.Runtime.InteropServices;

namespace W2Open.Common.GameStructure
{
    /// <summary>
    /// The character class in the game
    /// </summary>
    public enum ECharClass : byte
    {
        /// <summary>
        /// Trans Knight.
        /// </summary>
        TK = 0,
        /// <summary>
        /// Foema.
        /// </summary>
        FM = 1,
        /// <summary>
        /// Beast Master.
        /// </summary>
        BM = 2,
        /// <summary>
        /// Huntress.
        /// </summary>
        HT = 3
    }

    public enum ClassType : short
    {
        Arch = 1,
        Mortal = 2,
        Celestial = 3,
        CelestialCS = 4,
        SubCelestial = 5
    };
}