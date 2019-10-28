using System;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using W2Open.Common.GameStructure;

namespace W2Open.Common.Utility
{
    /// <summary>
    /// A wrapper encapsulating some core methods used to marshals raw data buffers into structures.
    /// </summary>
    public static  class W2Marshal
    {

        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr Memset(IntPtr dest, int c, int count);
        /// <summary>
        /// Marshals a raw buffer to a given marshalable struct.
        /// </summary>
        /// 
        






        public static unsafe T GetStructure<T>(byte[] buffer, int offset = 0) where T : struct
        {
            fixed (byte* bufferPin = &buffer[offset])
            {
                return GetStructure<T>(bufferPin);
            }
        }

        public static unsafe T GetStructure<T>(CCompoundBuffer buffer) where T : struct
        {
            return GetStructure<T>(buffer.RawBuffer, buffer.Offset);
        }

        public static unsafe T GetStructure<T>(byte* buffer) where T : struct
        {
            return (T)Marshal.PtrToStructure(new IntPtr(buffer), typeof(T));
        }

        /// <summary>
        /// Marshals a given T instance into a raw buffer.
        /// </summary>
        public static unsafe byte[] GetBytes<T>(T obj) where T : struct
        {
            byte[] rawBuffer = new byte[Marshal.SizeOf(obj)];

            fixed (byte* rawBufferPin = rawBuffer)
            {
                Marshal.StructureToPtr<T>(obj, new IntPtr(rawBufferPin), false);
            }

            return rawBuffer;
        }
        public static bool IsNull<T>(this T source) where T : struct
        {
            return source.Equals(default(T));
        }
        /// <summary>
        /// Crates a read-to-use marshaled instance of T
        /// </summary>
        /// <typeparam name="T">Type to be marshaled as zero-initialized instance.</typeparam>
        /// <returns>A zero-initialized instance of T.</returns>
        public static unsafe T CreateEmpty<T>() where T : struct
        {
            int typeSize = Marshal.SizeOf(typeof(T));

            byte* rawBuffer = stackalloc byte[typeSize];

            for (int i = 0; i < typeSize; i++)
                rawBuffer[i] = 0;
            
            T zeroInited = (T)Marshal.PtrToStructure(new IntPtr(rawBuffer), typeof(T));

            return zeroInited;
        }

        /// <summary>
        /// Creates a empty ready-to-use copy of a given implementation of DBSRVPackets.
        /// </summary>
        public static T CreatePacket<T>(ushort opcode) where T : struct, DBSRVPackets
        {
            MSG_HEADER validHeader = new MSG_HEADER();
            T packet = W2Marshal.CreateEmpty<T>();

            // Set the default values to the packet header.
            validHeader.Size = (ushort)Marshal.SizeOf(packet);
            validHeader.PacketID = opcode;
            validHeader.Key = (byte)W2Random.Instance.Next(127);
            validHeader.TimeStamp = (uint)Environment.TickCount;

            packet.Header = validHeader;

            return packet;
        }
        public static T CreatePacket<T>(ushort opcode,ushort id) where T : struct, DBSRVPackets
        {
            MSG_HEADER validHeader = new MSG_HEADER();
            T packet = W2Marshal.CreateEmpty<T>();

            // Set the default values to the packet header.
            validHeader.Size = (ushort)Marshal.SizeOf(packet);
            validHeader.PacketID = opcode;
            validHeader.ClientId = id;
            validHeader.Key = (byte)W2Random.Instance.Next(127);
            validHeader.TimeStamp = (uint)Environment.TickCount;

            packet.Header = validHeader;
            
            return packet;
        }

        public static D ReinterpretCast<S,D>(S SPacket) where D : struct, DBSRVPackets
        {
            byte[] rawBuffer = new byte[Marshal.SizeOf(SPacket)];
            unsafe
            {
                fixed (byte* rawBufferPin = rawBuffer)
                {
                    Marshal.StructureToPtr<S>(SPacket, new IntPtr(rawBufferPin), false);
                }
            }
            D sm = GetStructure<D>(rawBuffer);
            return sm;
        }




        public static T memset<T>(T Struct) where T : struct
        {
            byte[] rawData = W2Marshal.GetBytes<T>(Struct);
            GCHandle gch = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            Memset(gch.AddrOfPinnedObject(), 0, rawData.Length);
            var pinnedRawData = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            try
            {
                var pinnedRawDataPtr = pinnedRawData.AddrOfPinnedObject();
                return (T)Marshal.PtrToStructure(pinnedRawDataPtr, typeof(T));
            }
            finally
            {
                pinnedRawData.Free();
            }
        }
    }
}