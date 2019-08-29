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
    /// Allows the transfer of a maximum of 32 bytes at a time.
    /// </summary>
    public struct DiskDriveRequest : IDataType
    {
        public bool read;
        public uint address;
        public uint length;
        public int error;
        public byte[] data;

        uint _interiorAddress;

        public SmartPointer Pointer { get { return new SmartPointer(_interiorAddress, 525); } }

        public uint Address { get { return _interiorAddress; } set { _interiorAddress = value; } }

        public uint Length { get { return 525; } }

        public DiskDriveRequest(byte[] binary)
        {
            _interiorAddress = 0;
            read = binary[0] != 0;

            Range<byte> _address = new Range<byte>(1, sizeof(uint), binary);
            address = BitConverter.ToUInt32(_address.ToArray(), 0);

            Range<byte> _length = new Range<byte>(5, sizeof(uint), binary);
            length = BitConverter.ToUInt32(_length.ToArray(), 0);

            Range<byte> _error = new Range<byte>(9, sizeof(int), binary);
            error = BitConverter.ToInt32(_error.ToArray(), 0);

            Range<byte> _data = new Range<byte>(13, (int)length, binary);
            data = _data.ToArray();
        }

        public DiskDriveRequest(bool read, uint address, byte[] data, int error) : this()
        {
            _interiorAddress = 0;
            this.read = read;
            this.address = address;
            this.length = (uint)data.Length;
            if (length > 512)
                throw new ArgumentException("Data cannot exceed 512 bytes in length.");
            this.data = data;
            this.error = error;
        }

        public DiskDriveRequest(bool read, uint address, uint length) : this()
        {
            _interiorAddress = 0;
            this.read = read;
            this.address = address;
            this.length = length;
            if (length > 512)
                throw new ArgumentException("Data cannot exceed 512 bytes in length.");
            this.data = new byte[0];
            this.error = 0;
        }

        public byte[] ToBinary()
        {
            List<byte> result = new List<byte>();
            result.Add((byte)(read ? 1 : 0));
            result.AddRange(BitConverter.GetBytes(address));
            result.AddRange(BitConverter.GetBytes(length));
            result.AddRange(BitConverter.GetBytes(error));
            if (data != null)
                result.AddRange(data);
            else
                result.AddRange(new byte[0]);

            return result.ToArray();
        }

        public DiskDriveRequest FromBinary(byte[] binary)
        {
            return new DiskDriveRequest(binary);
        }

        void IDataType.FromBinary(byte[] value)
        {
            read = value[0] != 0;

            Range<byte> _address = new Range<byte>(1, sizeof(uint), value);
            address = BitConverter.ToUInt32(_address.ToArray(), 0);

            Range<byte> _length = new Range<byte>(5, sizeof(uint), value);
            length = BitConverter.ToUInt32(_length.ToArray(), 0);

            Range<byte> _error = new Range<byte>(9, sizeof(int), value);
            error = BitConverter.ToInt32(_error.ToArray(), 0);

            if ((length > 0) & (value.Length == length + 13))
            {
                Range<byte> _data = new Range<byte>(13, (int)length, value);
                data = _data.ToArray();
            }
            else
            {
                data = new byte[0];
            }
        }
    }
}
