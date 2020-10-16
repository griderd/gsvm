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
    public class DiskDriveRequest : GenericSerialRequest
    {
        // byte control;
        // uint data;
        public uint length;
        public byte[] diskData;

        uint _interiorAddress;

        public new uint Length { get { return 521; } }

        public DiskDriveRequest()
            : base()
        {
            length = 0;
            diskData = new byte[0];
        }

        public DiskDriveRequest(byte[] binary)
            : base(binary)
        {
            Range<byte> _length = new Range<byte>(5, sizeof(uint), binary);
            length = BitConverter.ToUInt32(_length.ToArray(), 0);

            Range<byte> _data = new Range<byte>(13, (int)length, binary);
            diskData = _data.ToArray();
        }

        public DiskDriveRequest(byte control, uint address, byte[] data)
            : base(control, address)
        {
            this.length = (uint)data.Length;
            if (length > 512)
                throw new ArgumentException("Data cannot exceed 512 bytes in length.");
            this.diskData = data;
        }

        public DiskDriveRequest(byte control, uint address, uint length)
            : base(control, address)
        {
            this.length = length;
            if (length > 512)
                throw new ArgumentException("Data cannot exceed 512 bytes in length.");
            this.diskData = new byte[0];
        }

        public override byte[] ToBinary()
        {
            List<byte> result = new List<byte>();
            result.AddRange(base.ToBinary());
            result.AddRange(BitConverter.GetBytes(length));
            if (diskData != null)
                result.AddRange(diskData);
            else
                result.AddRange(new byte[0]);

            return result.ToArray();
        }

        public override void FromBinary(byte[] binary)
        {
            control = binary[0];

            Range<byte> _data = new Range<byte>(1, 4, binary);
            data = BitConverter.ToUInt32(_data.ToArray(), 0);

            try
            {
                Range<byte> _length = new Range<byte>(5, sizeof(uint), binary);
                length = BitConverter.ToUInt32(_length.ToArray(), 0);
            }
            catch (BoundaryOutOfRangeException)
            {
                length = 0;
            }

            if ((length > 0) & (length + 9 == binary.Length))
            {
                Range<byte> _diskdata = new Range<byte>(9, (int)length, binary);
                diskData = _diskdata.ToArray();
            }
            else
            {
                diskData = new byte[0];
            }
        }   
    }
}
