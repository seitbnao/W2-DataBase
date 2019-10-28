namespace W2Open.GameState
{
    public enum DBResult
    {
        NO_ERROR,
        CHECKSUM_FAIL,
        PACKET_NOT_HANDLED,
        /// <summary>
        /// This error is caused when the GameServer's state is not the right to receive the given packet.
        /// This is CRITICAL and should result in a forced disconnection.
        /// </summary>
        PLAYER_INCONSISTENT_STATE,
        /// <summary>
        /// Some unknown error occour. Treat this as a CRITICAL error! The best approach is disconnect the GameServer.
        /// </summary>
        UNKNOWN,

        SUCCESS,

        WAIT,
    }
}