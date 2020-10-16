using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSLib.Collections;
using GSVM.Constructs;
using GSVM.Constructs.DataTypes;

namespace GSVM.Devices
{
    /// <summary>
    /// Transfers 4 bytes at a time.
    /// </summary>
    public class GenericSerialRequest : IDataType
    {
        public byte control;
        public uint data;

        uint interioraddress;

        public byte[] BinaryData
        {
            get
            {
                return BitConverter.GetBytes(data);
            }
            set
            {
                if (value == null)
                    throw new NullReferenceException();
                if (value.Length != 4)
                    throw new ArgumentException();
                data = BitConverter.ToUInt32(value, 0);
            }
        }

        /// <summary>
        /// Gets the lower (first) word in the data field.
        /// </summary>
        public ushort LowerWord
        {
            get
            {
                byte[] dw = BinaryData;
                byte[] w = new byte[] { dw[0], dw[1] };
                return BitConverter.ToUInt16(w, 0);
            }
            set
            {
                byte[] dw = BinaryData;
                byte[] w = BitConverter.GetBytes(value);
                dw[0] = w[0];
                dw[1] = w[1];
                BinaryData = dw;
            }
        }

        /// <summary>
        /// Gets the upper (second) word in the data field.
        /// </summary>
        public ushort UpperWord
        {
            get
            {
                byte[] dw = BinaryData;
                byte[] w = new byte[] { dw[2], dw[3] };
                return BitConverter.ToUInt16(w, 0);
            }
            set
            {
                byte[] dw = BinaryData;
                byte[] w = BitConverter.GetBytes(value);
                dw[0] = w[2];
                dw[1] = w[3];
                BinaryData = dw;
            }
        }

        public GenericSerialRequest()
        {
            interioraddress = 0;
            control = 0;
            data = 0;
        }

        public GenericSerialRequest(byte[] binary)
        {
            interioraddress = 0;

            control = binary[0];

            Range<byte> _data = new Range<byte>(1, 4, binary);
            data = BitConverter.ToUInt32(_data.ToArray(), 0);
        }

        public GenericSerialRequest(byte control, uint data)
        {
            this.data = data;
            this.control = control;
        }

        public SmartPointer Pointer { get { return new SmartPointer(interioraddress, Length); } }

        public uint InteriorAddress { get { return interioraddress; } set { interioraddress = value; } }

        public uint Length { get { return 6; } }

        public uint Address { get { return interioraddress; } set { interioraddress = value; } }

        public virtual void FromBinary(byte[] binary)
        {
            control = binary[0];

            Range<byte> _data = new Range<byte>(1, 4, binary);
            data = BitConverter.ToUInt32(_data.ToArray(), 0);
        }

        public virtual byte[] ToBinary()
        {
            List<byte> result = new List<byte>();
            result.Add(control);
            result.AddRange(BinaryData);

            return result.ToArray();
        }

        void IDataType.FromBinary(byte[] value)
        {
            FromBinary(value);
        }
    }
}
