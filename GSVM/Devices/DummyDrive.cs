using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Components;

namespace GSVM.Devices
{
    public class DummyDrive : DiskDrive
    {
        public DummyDrive()
            : base(0, true)
        {

        }

        public override void ClockTick()
        {
            if (ReadyToWrite)
            {
                DiskDriveRequest result = new DiskDriveRequest();

                result.address = WriteData.address;
                result.length = WriteData.length;
                result.error = 1;
                result.data = new byte[0];

                ReadData = result;
                ReadyToRead = true;
                ReadyToWrite = false;
            }

        }
    }
}
