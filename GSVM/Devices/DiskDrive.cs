using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Components;

namespace GSVM.Devices
{
    public class DiskDrive : GenericDeviceBus<DiskDriveRequest>
    {
        protected Memory memory;
        public bool ReadOnly { get; private set; }

        public DiskDrive(uint size, bool isReadOnly = false)
        {
            memory = new Memory(size);
            ReadOnly = isReadOnly;
        }

        public DiskDrive(Memory memory, bool isReadOnly = false)
        {
            this.memory = memory;
            ReadOnly = isReadOnly;
        }
    
        public override void ClockTick()
        {
            if (ReadyToWrite)
            {
                DiskDriveRequest result = new DiskDriveRequest();

                result.address = WriteData.address;
                result.length = WriteData.length;
                result.error = 0;

                if (WriteData.read)
                {
                    // If a GetLength command is sent
                    if (WriteData.error == 255)
                    {
                        result.data = BitConverter.GetBytes(memory.Length);
                        result.length = 4;
                        result.address = 0;
                    }
                    else
                    {
                        try
                        {
                            result.data = memory.Read(WriteData.address, WriteData.length);
                        }
                        catch
                        {
                            result.error = 1;
                        }
                    }
                }
                else
                {
                    if (ReadOnly)
                    {
                        result.error = -1;
                    }
                    else
                    {
                        try
                        {
                            memory.Write(WriteData.address, WriteData.data);
                        }
                        catch
                        {
                            result.error = 1;
                        }
                    }
                }

                ReadData = result;
                ReadyToRead = true;
                ReadyToWrite = false;
            }
                
        }
    }
}
