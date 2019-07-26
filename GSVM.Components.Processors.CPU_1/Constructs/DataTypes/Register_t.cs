using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Components.Processors.CPU_1;

namespace GSVM.Constructs.DataTypes
{
    public class Register_t : Integral<ushort>
    {
        public Register_t()
            : this(Register.PC)
        {

        }

        public Register_t(Register value, uint address)
        {
            this.value = (ushort)value;
            ptr = new SmartPointer(address, 2);
        }

        public Register_t(Register value) : this(value, 0)            
        {
        }

        public Register_t(ushort value) : this((Register)value, 0)
        {
        }

        public Register_t(byte[] value)
            : this()
        {
            FromBinary(value);
        }

        public override byte[] ToBinary()
        {
            return BitConverter.GetBytes((ushort)value);
        }

        public override void FromBinary(byte[] value)
        {
            if (value.Length > Pointer.Length)
                throw new IndexOutOfRangeException();

            byte[] val = new byte[Pointer.Length];
            Array.Copy(value, val, value.Length);

            this.value = BitConverter.ToUInt16(val, 0);
        }

        public static implicit operator UInt16(Register_t value)
        {
            return (ushort)value.Value;
        }

        public static implicit operator Register_t(UInt16 value)
        {
            return new Register_t((Register)value);
        }

        public static implicit operator uint16_t(Register_t value)
        {
            return (ushort)value.Value;
        }

        public static implicit operator Register_t(uint16_t value)
        {
            return new Register_t((Register)value.Value);
        }

        public static implicit operator Register(Register_t value)
        {
            return (Register)value.Value;
        }

        public static implicit operator Register_t(Register value)
        {
            return new Register_t(value);
        }
    }
}
