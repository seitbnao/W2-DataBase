namespace W2Open.Common.Utility
{
    /// <summary>
    /// The implementation of a compounded raw byte buffer.
    /// This object encapsulates the process of reading and writing to a byte buffer when this activity is will be done many times.
    /// After instantiating a CCoumpoundBuffer, you read or write in the byte buffer via the Read/Write methods.
    /// </summary>
    public class CCompoundBuffer
    {
        /// <summary>
        /// The raw packet buffer.
        /// </summary>
        public byte[] RawBuffer { get; set; }

        /// <summary>
        /// This variable must be called only in his property!
        /// </summary>
        private int m_Offset;
        /// <summary>
        /// The actual offset to the valid data in the buffer.
        /// </summary>
        public int Offset
        {
            get { return m_Offset; }

            set
            {
                if (value < 0)
                    m_Offset = 0;
                else if (value >= RawBuffer.Length)
                    m_Offset = RawBuffer.Length - 1;
                else
                    m_Offset = value;
            }
        }

        public CCompoundBuffer(int bufferLength, int initialOffset = 0)
        {
            RawBuffer = new byte[bufferLength];
            Offset = initialOffset;
        }

        public unsafe short ReadNextShort(int adtOffset = 0)
        {
            fixed (byte* b = RawBuffer)
            {
                return *(short*)&b[(Offset + adtOffset).Clamp(0, RawBuffer.Length)];
            }
        }

        public ushort ReadNextUShort(int adtOffset = 0)
        {
            return (ushort)ReadNextShort(adtOffset);
        }

        public unsafe int ReadNextInt(int adtOffset = 0)
        {
            fixed (byte* b = RawBuffer)
            {
                return *(int*)&b[(Offset + adtOffset).Clamp(0, RawBuffer.Length)];
            }
        }
    }



}