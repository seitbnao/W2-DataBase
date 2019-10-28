﻿namespace W2Open.Common
{
    /// <summary>
    /// Contains all the basic definitions related to the project.
    /// Mostly, lower level definitions about the patterns used in the code.
    /// </summary>
    public static class Defines
    {
        /// <summary>
        /// Memory pack layout used to correctly Marshals the raw data into structures.
        /// This is the memory pack alignment used in the game's client, so we have to respect that.
        /// </summary>
        public const int DEFAULT_PACK = 1;
    }
}