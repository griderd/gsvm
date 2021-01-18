using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Constructs.DataTypes;
using GSVM.Components.Coprocessors;

namespace GSVM.Constructs.FastDataTypes
{
    public class int8_t : Integral<sbyte>
    {
        public int8_t()
            : this(0)
        {

        }

        public int8_t(sbyte value, uint address)
        {
            this.value = value;
            ptr = new SmartPointer(address, 1);
        }

        public int8_t(sbyte value) : this(value, 0)
        {
        }

        public int8_t(byte[] value)
            : this()
        {
            FromBinary(value);
        }

        public override byte[] ToBinary()
        {
            unchecked
            {
                return new byte[] { (byte)value };
            }
        }

        public override void FromBinary(byte[] value)
        {
            if (value.Length > Pointer.Length)
                throw new IndexOutOfRangeException();

            unchecked
            {
                Value = (sbyte)value[0];
            }
        }

        public static implicit operator sbyte(int8_t value)
        {
            return value.Value;
        }

        public static implicit operator int8_t(sbyte value)
        {
            return new int8_t(value);
        }
    }

    public class uint8_t : Integral<byte>
    {
        public uint8_t()
            : this(0)
        {

        }

        public uint8_t(byte value, uint address)
        {
            this.value = value;
            ptr = new SmartPointer(address, 1);
        }

        public uint8_t(byte value) : this(value, 0)
        {
        }

        public uint8_t(byte[] value)
            : this()
        {
            FromBinary(value);
        }

        public override byte[] ToBinary()
        {
            return new byte[] { value };
        }

        public override void FromBinary(byte[] value)
        {
            if (value.Length > Pointer.Length)
                throw new IndexOutOfRangeException();

            this.value = value[0];
        }

        public static implicit operator byte(uint8_t value)
        {
            return value.Value;
        }

        public static implicit operator uint8_t(byte value)
        {
            return new uint8_t(value);
        }
    }

    public class int16_t : Integral<Int16>
    {
        public int16_t()
            : this(0)
        {

        }

        public int16_t(Int16 value, uint address)
        {
            this.value = value;
            ptr = new SmartPointer(address, 2);
        }

        public int16_t(Int16 value) : this(value, 0)
        {
        }

        public int16_t(byte[] value)
            : this()
        {
            FromBinary(value);
        }

        public override byte[] ToBinary()
        {
            return BitConverter.GetBytes(value);
        }

        public override void FromBinary(byte[] value)
        {
            if (value.Length > Pointer.Length)
                throw new IndexOutOfRangeException();

            byte[] val = new byte[Pointer.Length];
            Array.Copy(value, val, value.Length);

            this.value = FastBitConverter.ToInt16(val, 0);
        }

        public static implicit operator Int16(int16_t value)
        {
            return value.Value;
        }

        public static implicit operator int16_t(Int16 value)
        {
            return new int16_t(value);
        }
    }

    public class uint16_t : Integral<UInt16>
    {
        public uint16_t()
            : this(0)
        {

        }

        public uint16_t(UInt16 value, uint address)
        {
            this.value = value;
            ptr = new SmartPointer(address, 2);
        }

        public uint16_t(UInt16 value) : this(value, 0)
        {
        }

        public uint16_t(uint address) : this(0, address)
        {
        }

        public uint16_t(byte[] value)
            : this()
        {
            FromBinary(value);
        }

        public override byte[] ToBinary()
        {
            return BitConverter.GetBytes(value);
        }

        public override void FromBinary(byte[] value)
        {
            if (value.Length > Pointer.Length)
                throw new IndexOutOfRangeException();

            byte[] val = new byte[Pointer.Length];
            Array.Copy(value, val, value.Length);

            this.value = FastBitConverter.ToUInt16(val, 0);
        }

        public static implicit operator UInt16(uint16_t value)
        {
            return value.Value;
        }

        public static implicit operator uint16_t(UInt16 value)
        {
            return new uint16_t(value);
        }
    }

    public class int32_t : Integral<Int32>
    {
        public int32_t()
            : this(0)
        {

        }

        public int32_t(Int32 value, uint address)
        {
            this.value = value;
            ptr = new SmartPointer(address, 4);
        }

        public int32_t(Int32 value) : this(value, 0)
        {
        }

        public int32_t(uint address) : this(0, address)
        {
        }

        public int32_t(byte[] value)
            : this()
        {
            FromBinary(value);
        }

        public override byte[] ToBinary()
        {
            return BitConverter.GetBytes(value);
        }

        public override void FromBinary(byte[] value)
        {
            if (value.Length > Pointer.Length)
                throw new IndexOutOfRangeException();

            byte[] val = new byte[Pointer.Length];
            Array.Copy(value, val, value.Length);

            this.value = FastBitConverter.ToInt32(val, 0);
        }

        public static implicit operator Int32(int32_t value)
        {
            return value.Value;
        }

        public static implicit operator int32_t(Int32 value)
        {
            return new int32_t(value);
        }
    }

    public class uint32_t : Integral<UInt32>
    {
        public uint32_t()
            : this(0)
        {

        }

        public uint32_t(UInt32 value, uint address)
        {
            this.value = value;
            ptr = new SmartPointer(address, 4);
        }

        public uint32_t(UInt32 value) : this(value, 0)
        {
        }

        public uint32_t(byte[] value)
            : this()
        {
            FromBinary(value);
        }

        public override byte[] ToBinary()
        {
            return BitConverter.GetBytes(value);
        }

        public override void FromBinary(byte[] value)
        {
            if (value.Length > Pointer.Length)
                throw new IndexOutOfRangeException();

            byte[] val = new byte[Pointer.Length];
            Array.Copy(value, val, value.Length);

            this.value = FastBitConverter.ToUInt32(val, 0);
        }

        public static implicit operator UInt32(uint32_t value)
        {
            return value.Value;
        }

        public static implicit operator uint32_t(UInt32 value)
        {
            return new uint32_t(value);
        }
    }

    public class int64_t : Integral<Int64>
    {
        public int64_t()
            : this(0)
        {

        }

        public int64_t(Int64 value, uint address)
        {
            this.value = value;
            ptr = new SmartPointer(address, 8);
        }

        public int64_t(Int64 value) : this(value, 0)
        {
        }

        public int64_t(uint address) : this(0, address)
        {
        }

        public int64_t(byte[] value)
            : this()
        {
            FromBinary(value);
        }

        public override byte[] ToBinary()
        {
            return BitConverter.GetBytes(value);
        }

        public override void FromBinary(byte[] value)
        {
            if (value.Length > Pointer.Length)
                throw new IndexOutOfRangeException();

            byte[] val = new byte[Pointer.Length];
            Array.Copy(value, val, value.Length);

            this.value = FastBitConverter.ToInt64(val, 0);
        }

        public static implicit operator Int64(int64_t value)
        {
            return value.Value;
        }

        public static implicit operator int64_t(Int64 value)
        {
            return new int64_t(value);
        }
    }

    public class uint64_t : Integral<UInt64>
    {
        public uint64_t()
            : this(0)
        {

        }

        public uint64_t(UInt64 value, uint address)
        {
            this.value = value;
            ptr = new SmartPointer(address, 8);
        }

        public uint64_t(UInt64 value) : this(value, 0)
        {
        }

        public uint64_t(uint address) : this(0, address)
        {
        }

        public uint64_t(byte[] value)
            : this()
        {
            FromBinary(value);
        }

        public override byte[] ToBinary()
        {
            return BitConverter.GetBytes(value);
        }

        public override void FromBinary(byte[] value)
        {
            if (value.Length > Pointer.Length)
                throw new IndexOutOfRangeException();

            byte[] val = new byte[Pointer.Length];
            Array.Copy(value, val, value.Length);

            this.value = FastBitConverter.ToUInt64(val, 0);
        }

        public static implicit operator UInt64(uint64_t value)
        {
            return value.Value;
        }

        public static implicit operator uint64_t(UInt64 value)
        {
            return new uint64_t(value);
        }
    }
}
