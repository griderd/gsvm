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
    }

    
}


