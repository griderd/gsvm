using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Components.Coprocessors
{
    
    public static class FastBitConverter
    {
        public static Int16 ToInt16(byte[] value, int startIndex = 0)
        {
            if (startIndex + value.Length < 2)
                throw new IndexOutOfRangeException();

            Span<byte> sub = new Span<byte>(value, startIndex, 2);

            unsafe
            {
                fixed (byte* arr = &sub[0])
                {
                    Int16* ptr = (Int16*)arr;
                    return *ptr;
                }
            }
        }

        public static UInt16 ToUInt16(byte[] value, int startIndex = 0)
        {
            if (startIndex + value.Length < 2)
                throw new IndexOutOfRangeException();

            Span<byte> sub = new Span<byte>(value, startIndex, 2);

            unsafe
            {
                fixed (byte* arr = &sub[0])
                {
                    UInt16* ptr = (UInt16*)arr;
                    return *ptr;
                }
            }
        }

        public static Int32 ToInt32(byte[] value, int startIndex = 0)
        {
            if (startIndex + value.Length < 4)
                throw new IndexOutOfRangeException();

            Span<byte> sub = new Span<byte>(value, startIndex, 4);

            unsafe
            {
                fixed (byte* arr = &sub[0])
                {
                    Int32* ptr = (Int32*)arr;
                    return *ptr;
                }
            }
        }

        public static UInt32 ToUInt32(byte[] value, int startIndex = 0)
        {
            if (startIndex + value.Length < 4)
                throw new IndexOutOfRangeException();

            Span<byte> sub = new Span<byte>(value, startIndex, 4);

            unsafe
            {
                fixed (byte* arr = &sub[0])
                {
                    UInt32* ptr = (UInt32*)arr;
                    return *ptr;
                }
            }
        }

        public static Int64 ToInt64(byte[] value, int startIndex = 0)
        {
            if (startIndex + value.Length < 8)
                throw new IndexOutOfRangeException();

            Span<byte> sub = new Span<byte>(value, startIndex, 8);

            unsafe
            {
                fixed (byte* arr = &sub[0])
                {
                    Int64* ptr = (Int64*)arr;
                    return *ptr;
                }
            }
        }

        public static UInt64 ToUInt64(byte[] value, int startIndex = 0)
        {
            if (startIndex + value.Length < 8)
                throw new IndexOutOfRangeException();

            Span<byte> sub = new Span<byte>(value, startIndex, 8);

            unsafe
            {
                fixed (byte* arr = &sub[0])
                {
                    UInt64* ptr = (UInt64*)arr;
                    return *ptr;
                }
            }
        }

        
    }
    
}
