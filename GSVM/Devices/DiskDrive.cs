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
        public const byte LEN = 6;
        public const byte PING = 7;
        public const byte REPLY = 8;

        protected IMemory memory;
        public bool ReadOnly { get; private set; }

        private DiskDrive(bool isReadOnly = false)
        {
            ReadOnly = isReadOnly;
            deviceType = 2;
            GenerateID();
        }

        public DiskDrive(uint size, bool isReadOnly = false) : this(isReadOnly)
        {
            memory = new Memory(size);    
        }

        public DiskDrive(IMemory memory, bool isReadOnly = false) : this(isReadOnly)
        {
            this.memory = memory;
        }
    
        public override void ClockTick()
        {
            if (readData == null)
                readData = new DiskDriveRequest();

            if (ReadyToWrite)
            {
                readData.length = 0;
                readData.diskData = new byte[0];
                readData.data = 0;
                readData.control = 0;

                switch (WriteData.control)
                {
                    // READ
                    case READ:
                        readData.control = ACK;
                        readData.data = writeData.data;
                        readData.length = writeData.length;
                        try
                        {
                            readData.diskData = memory.Read(writeData.data, writeData.length);
                        }
                        catch
                        {
                            // error
                            readData.control = ERROR;
                            readData.data = 0;
                        }
                        break;

                    case WRITE:
                        if (ReadOnly)
                        {
                            readData.control = ERROR;
                            readData.data = 1;   // cannot overwrite read-only memory
                        }
                        else
                        {
                            readData.control = ACK;
                            readData.data = WriteData.data;
                            readData.length = WriteData.length;
                            try
                            {
                                memory.Write(writeData.data, writeData.diskData);
                            }
                            catch
                            {
                                readData.control = ERROR;   // error
                                readData.data = 0;      // Unknown
                            }
                        }
                        break;

                    case LEN:
                        readData.control = ACK;   // ack
                        readData.data = memory.Length;
                        break;

                    case PING:
                        readData.control = REPLY;   // reply
                        break;

                }
            }

            base.ClockTick();
        }

        protected override bool InterruptChannelOk(uint channel)
        {
            return true;
        }
    }
}
