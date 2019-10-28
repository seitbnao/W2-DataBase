using System;

namespace W2Open.Common.Utility
{
    /// <summary>
    /// Encapsulates a singleton System.Random class implementation.
    /// </summary>
    public class W2Random : Random
    {
        private static W2Random m_Instance;

        public static W2Random Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new W2Random();

                return m_Instance;
            }
        }

        private W2Random() { }
    }
}