namespace W2Open.Common
{
    /// <summary>
    /// All the basic definitions related directly to the game content.
    /// </summary>
    public static class GameBasics
    {
        #region Persistent Related
        public const int MAXL_ACC_MOB = 4;
        public const int MAXL_CARGO_ITEM = 128;
        public const int MAXL_AFFECT = 32;
        public const int MAXL_ITEM_EFFECT = 3;
        public const int MAXL_EQUIP = 16;
        public const int MAXL_INVENTORY = 64;
        #endregion

        /// <summary>
        /// The highest move speed a mob can get in the game.
        /// </summary>
        public const byte MAX_MOB_SPEED = 6;

        public const short MAX_NumericERRORS = 3;
    }
}