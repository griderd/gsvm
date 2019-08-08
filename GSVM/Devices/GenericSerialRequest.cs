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
    public struct GenericSerialRequest : IDataType
    {
        public bool read;
        public byte error;
        public byte[] data;

        uint address;

        public GenericSerialRequest(byte[] binary)
        {
            address = 0;
            read = binary[0] != 0;

            error = binary[1];

            Range<byte> _data = new Range<byte>(2, 4, binary);
            data = _data.ToArray();
        }

        public GenericSerialRequest(bool read, byte error, byte[] data) : this()
        {
            this.read = read;
            this.data = data;
            this.error = error;
        }

        public SmartPointer Pointer { get { return new SmartPointer(address, Length); } }

        public uint Address { get { return address; } set { address = value; } }

        public uint Length { get { return 6; } }

        public GenericSerialRequest FromBinary(byte[] binary)
        {
            throw new NotImplementedException();
        }

        public byte[] ToBinary()
        {
            List<byte> result = new List<byte>();
            result.Add((byte)(read ? 1 : 0));
            result.Add(error);
            result.AddRange(data);

            return result.ToArray();
        }

        void IDataType.FromBinary(byte[] value)
        {
            read = value[0] != 0;

            error = value[1];

            Range<byte> _data = new Range<byte>(2, 4, value);
            data = _data.ToArray();
        }
    }
}
