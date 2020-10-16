using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Constructs.DataTypes;

namespace GSVM.Components
{
    [Flags]
    public enum ALUFlags
    {
        None = 0,
        Equal = 1,
        LessThan = 2,
        GreaterThan = 4,
        Overflow = 8
    }

    public class ALU
    {
        public ALUFlags flags;

        public delegate byte[] ALUOperation(byte[] a, byte[] b, out bool overflow);
        public delegate byte[] ALUOperationUnary(byte[] a);
        public delegate void ALUCompare(byte[] a, byte[] b);

        public ALU()
        {
            flags = ALUFlags.None;
        }

        public byte[] LeftShift(byte[] a, byte[] b, out bool overflow)
        {
            List<byte> bCount = new List<byte>(b);
            if (b.Length < 8)
                bCount.AddRange(new byte[8 - b.Length]);

            ulong count = BitConverter.ToUInt64(bCount.ToArray(), 0);

            byte[] value = a;
            overflow = false;

            for (ulong i = 0; i < count; i++)
            {
                overflow = overflow | ShiftLeft(value);
            }

            return value;
        }

        public byte[] RightShift(byte[] a, byte[] b, out bool overflow)
        {
            List<byte> bCount = new List<byte>(b);
            if (b.Length < 8)
                bCount.AddRange(new byte[8 - b.Length]);

            ulong count = BitConverter.ToUInt64(bCount.ToArray(), 0);

            byte[] value = a;
            overflow = false;

            for (ulong i = 0; i < count; i++)
            {
                overflow = overflow | ShiftRight(value);
            }

            return value;
        }

        public byte[] Add(byte[] a, byte[] b, out bool overflow)
        {
            uint64_t _a = new uint64_t(a);
            uint64_t _b = new uint64_t(b);

            uint64_t result = _a + _b;
            byte[] r = result.ToBinary();
            byte[] values;

            overflow = false;

            if (a.Length >= b.Length)
                values = new byte[a.Length];
            else
                values = new byte[b.Length];

            Array.Copy(r, values, values.Length);
            if (values.Length < 8)
            {
                overflow = (r[values.Length + 1] > 0);
            }
            else
            {
                try
                {
                    UInt64 temp = checked(_a + _b);
                }
                catch (OverflowException)
                {
                    overflow = true;
                    flags = flags | ALUFlags.Overflow;
                }
            }

            flags = ALUFlags.None;
            if (overflow)
                flags = flags | ALUFlags.Overflow;
            return values;
        }

        public byte[] Subtract(byte[] a, byte[] b, out bool overflow)
        {
            uint64_t _a = new uint64_t(a);
            uint64_t _b = new uint64_t(b);

            uint64_t result = _a - _b;
            byte[] r = result.ToBinary();
            byte[] values;

            overflow = false;

            if (a.Length >= b.Length)
                values = new byte[a.Length];
            else
                values = new byte[b.Length];

            Array.Copy(r, values, values.Length);
            if (values.Length < 8)
            {
                overflow = (r[values.Length + 1] > 0);
            }
            else
            {
                try
                {
                    UInt64 temp = checked(_a - _b);
                }
                catch (OverflowException)
                {
                    overflow = true;
                }
            }

            flags = ALUFlags.None;
            if (overflow)
                flags = flags | ALUFlags.Overflow;
            return values;
        }

        public byte[] Multiply(byte[] a, byte[] b, out bool overflow)
        {
            uint64_t _a = new uint64_t(a);
            uint64_t _b = new uint64_t(b);

            uint64_t result = _a * _b;
            byte[] r = result.ToBinary();
            byte[] values;

            overflow = false;

            if (a.Length >= b.Length)
                values = new byte[a.Length];
            else
                values = new byte[b.Length];

            Array.Copy(r, values, values.Length);
            if (values.Length < 8)
            {
                overflow = (r[values.Length] > 0);
            }
            else
            {
                try
                {
                    UInt64 temp = checked(_a * _b);
                }
                catch (OverflowException)
                {
                    overflow = true;
                }
            }

            flags = ALUFlags.None;
            if (overflow)
                flags = flags | ALUFlags.Overflow;
            return values;
        }

        public byte[] Divide(byte[] a, byte[] b, out bool overflow)
        {
            uint64_t _a = new uint64_t(a);
            uint64_t _b = new uint64_t(b);

            uint64_t result = _a / _b;
            byte[] r = result.ToBinary();
            byte[] values;

            overflow = false;

            if (a.Length >= b.Length)
                values = new byte[a.Length];
            else
                values = new byte[b.Length];

            Array.Copy(r, values, values.Length);
            if (values.Length < 8)
            {
                overflow = (r[values.Length] > 0);
            }
            else
            {
                try
                {
                    UInt64 temp = checked(_a / _b);
                }
                catch (OverflowException)
                {
                    overflow = true;
                }
            }

            flags = ALUFlags.None;
            if (overflow)
                flags = flags | ALUFlags.Overflow;
            return values;
        }

        public byte[] Mod(byte[] a, byte[] b, out bool overflow)
        {
            uint64_t _a = new uint64_t(a);
            uint64_t _b = new uint64_t(b);

            uint64_t result = _a % _b;
            byte[] r = result.ToBinary();
            byte[] values;

            overflow = false;

            if (a.Length >= b.Length)
                values = new byte[a.Length];
            else
                values = new byte[b.Length];

            Array.Copy(r, values, values.Length);
            if (values.Length < 8)
            {
                overflow = (r[values.Length] > 0);
            }
            else
            {
                try
                {
                    UInt64 temp = checked(_a % _b);
                }
                catch (OverflowException)
                {
                    overflow = true;
                }
            }

            flags = ALUFlags.None;
            if (overflow)
                flags = flags | ALUFlags.Overflow;
            return values;
        }

        public byte[] And(byte[] a, byte[] b, out bool overflow)
        {
            uint64_t _a = new uint64_t(a);
            uint64_t _b = new uint64_t(b);

            uint64_t result = _a & _b;
            byte[] r = result.ToBinary();
            byte[] values;

            overflow = false;

            if (a.Length >= b.Length)
                values = new byte[a.Length];
            else
                values = new byte[b.Length];

            Array.Copy(r, values, values.Length);
            if (values.Length < 8)
            {
                overflow = (r[values.Length] > 0);
            }
            else
            {
                try
                {
                    UInt64 temp = checked(_a & _b);
                }
                catch (OverflowException)
                {
                    overflow = true;
                }
            }

            flags = ALUFlags.None;
            if (overflow)
                flags = flags | ALUFlags.Overflow;
            return values;
        }

        public byte[] Or(byte[] a, byte[] b, out bool overflow)
        {
            uint64_t _a = new uint64_t(a);
            uint64_t _b = new uint64_t(b);

            uint64_t result = _a | _b;
            byte[] r = result.ToBinary();
            byte[] values;

            overflow = false;

            if (a.Length >= b.Length)
                values = new byte[a.Length];
            else
                values = new byte[b.Length];

            Array.Copy(r, values, values.Length);
            if (values.Length < 8)
            {
                overflow = (r[values.Length] > 0);
            }
            else
            {
                try
                {
                    UInt64 temp = checked(_a | _b);
                }
                catch (OverflowException)
                {
                    overflow = true;
                }
            }

            flags = ALUFlags.None;
            if (overflow)
                flags = flags | ALUFlags.Overflow;
            return values;
        }

        public byte[] Xor(byte[] a, byte[] b, out bool overflow)
        {
            uint64_t _a = new uint64_t(a);
            uint64_t _b = new uint64_t(b);

            uint64_t result = _a ^ _b;
            byte[] r = result.ToBinary();
            byte[] values;

            overflow = false;

            if (a.Length >= b.Length)
                values = new byte[a.Length];
            else
                values = new byte[b.Length];

            Array.Copy(r, values, values.Length);
            if (values.Length < 8)
            {
                overflow = (r[values.Length] > 0);
            }
            else
            {
                try
                {
                    UInt64 temp = checked(_a ^ _b);
                }
                catch (OverflowException)
                {
                    overflow = true;
                }
            }

            flags = ALUFlags.None;
            if (overflow)
                flags = flags | ALUFlags.Overflow;
            return values;
        }

        public byte[] Not(byte[] a)
        {
            byte[] result = new byte[a.Length];

            for (int i = 0; i < a.Length; i++)
            {
                result[i] = (byte)(~a[i]);
            }

            flags = ALUFlags.None;
            return result;
        }

        public byte[] Negate(byte[] a)
        {
            int64_t _a = new int64_t(a);

            int64_t result = -_a;
            byte[] r = result.ToBinary();
            byte[] values;

            values = new byte[a.Length];

            Array.Copy(r, values, values.Length);

            flags = ALUFlags.None;
            return values;
        }

        public void Compare(byte[] a, byte[] b)
        {
            uint64_t _a = new uint64_t(a);
            uint64_t _b = new uint64_t(b);

            flags = ALUFlags.None;
            if (_a.Value == _b.Value)
                flags = flags | ALUFlags.Equal;
            if (_a.Value > _b.Value)
                flags = flags | ALUFlags.GreaterThan;
            if (_a.Value < _b.Value)
                flags = flags | ALUFlags.LessThan;
        }

        #region Shift and Rotate

        // THE FOLLOWING FUNCTIONS ARE COURTESY OF JamieSee ON STACK OVERFLOW
        // User: https://stackoverflow.com/users/1015164/jamiesee
        // Question: https://stackoverflow.com/questions/8440938/c-sharp-left-shift-an-entire-byte-array

        /// <summary>
        /// Rotates the bits in an array of bytes to the left.
        /// </summary>
        /// <param name="bytes">The byte array to rotate.</param>
        public static void RotateLeft(byte[] bytes)
        {
            bool carryFlag = ShiftLeft(bytes);

            if (carryFlag == true)
            {
                bytes[bytes.Length - 1] = (byte)(bytes[bytes.Length - 1] | 0x01);
            }
        }

        /// <summary>
        /// Rotates the bits in an array of bytes to the right.
        /// </summary>
        /// <param name="bytes">The byte array to rotate.</param>
        public static void RotateRight(byte[] bytes)
        {
            bool carryFlag = ShiftRight(bytes);

            if (carryFlag == true)
            {
                bytes[0] = (byte)(bytes[0] | 0x80);
            }
        }

        /// <summary>
        /// Shifts the bits in an array of bytes to the left.
        /// </summary>
        /// <param name="bytes">The byte array to shift.</param>
        public static bool ShiftLeft(byte[] bytes)
        {
            bool leftMostCarryFlag = false;

            // Iterate through the elements of the array from left to right.
            for (int index = 0; index < bytes.Length; index++)
            {
                // If the leftmost bit of the current byte is 1 then we have a carry.
                bool carryFlag = (bytes[index] & 0x80) > 0;

                if (index > 0)
                {
                    if (carryFlag == true)
                    {
                        // Apply the carry to the rightmost bit of the current bytes neighbor to the left.
                        bytes[index - 1] = (byte)(bytes[index - 1] | 0x01);
                    }
                }
                else
                {
                    leftMostCarryFlag = carryFlag;
                }

                bytes[index] = (byte)(bytes[index] << 1);
            }

            return leftMostCarryFlag;
        }

        /// <summary>
        /// Shifts the bits in an array of bytes to the right.
        /// </summary>
        /// <param name="bytes">The byte array to shift.</param>
        public static bool ShiftRight(byte[] bytes)
        {
            bool rightMostCarryFlag = false;
            int rightEnd = bytes.Length - 1;

            // Iterate through the elements of the array right to left.
            for (int index = rightEnd; index >= 0; index--)
            {
                // If the rightmost bit of the current byte is 1 then we have a carry.
                bool carryFlag = (bytes[index] & 0x01) > 0;

                if (index < rightEnd)
                {
                    if (carryFlag == true)
                    {
                        // Apply the carry to the leftmost bit of the current bytes neighbor to the right.
                        bytes[index + 1] = (byte)(bytes[index + 1] | 0x80);
                    }
                }
                else
                {
                    rightMostCarryFlag = carryFlag;
                }

                bytes[index] = (byte)(bytes[index] >> 1);
            }

            return rightMostCarryFlag;

            #endregion 
        }
    }

    
}


