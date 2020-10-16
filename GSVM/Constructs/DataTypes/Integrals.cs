using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Constructs.DataTypes
{
    public static class Int
    {
        public static uint8_t UInt8 { get { return new uint8_t(); } }
        public static int8_t Int8 { get { return new int8_t(); } }

        public static uint16_t UInt16 { get { return new uint16_t(); } }
        public static int16_t Int16 { get { return new int16_t(); } }

        public static uint32_t UInt32 { get { return new uint32_t(); } }
        public static int32_t Int32 { get { return new int32_t(); } }

        public static uint64_t UInt64 { get { return new uint64_t(); } }
        public static int64_t Int64 { get { return new int64_t(); } }

        public static T Pointer<T>(uint address)
            where T : IDataType
        {
            T value = Activator.CreateInstance<T>();
            value.Address = address;
            return value;
        }

        public static IIntegral BestFit(int size, bool signed = false)
        {
            if (size <= 1)
            {
                if (signed)
                    return Int8;
                else
                    return UInt8;
            }
            else if (size <= 2)
            {
                if (signed)
                    return Int16;
                else
                    return UInt16;
            }
            else if (size <= 4)
            {
                if (signed)
                    return Int32;
                else
                    return UInt32;
            }
            else
            {
                if (signed)
                    return Int64;
                else
                    return UInt64;
            }
        }
    }

    public abstract class Integral<T> : IIntegral<T>
    {
        protected SmartPointer ptr;

        public SmartPointer Pointer
        {
            get { return ptr; }
        }

        protected T value;

        public T Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public uint Address { get { return ptr.Address; } set { ptr.Address = value; } }

        public uint Length { get { return ptr.Length; } }

        object IIntegral.Value { get { return value; } }

        public abstract void FromBinary(byte[] value);

        public abstract byte[] ToBinary();

        public override string ToString()
        {
            return value.ToString();
        }

        public TOut CastTo<TOut>() where TOut : IIntegral
        {
            TOut result = default(TOut);
            byte[] value = new byte[result.Length];
            byte[] bin = ToBinary();

            for (int i = 0; i < result.Length; i++)
            {
                if (i < bin.Length)
                    value[i] = bin[i];
                else
                    value[i] = 0;
            }

            result.FromBinary(value);
            return result;
        }

        public IIntegral CastTo(IIntegral tout)
        {
            IIntegral result = tout;
            byte[] value = new byte[result.Length];
            byte[] bin = ToBinary();

            for (int i = 0; i < result.Length; i++)
            {
                if (i < bin.Length)
                    value[i] = bin[i];
                else
                    value[i] = 0;
            }

            result.FromBinary(value);
            return result;
        }
    }

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

            this.value = BitConverter.ToInt16(val, 0);
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

            this.value = BitConverter.ToUInt16(val, 0);
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

            this.value = BitConverter.ToInt32(val, 0);
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

            this.value = BitConverter.ToUInt32(val, 0);
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

            this.value = BitConverter.ToInt64(val, 0);
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

            this.value = BitConverter.ToUInt64(val, 0);
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
