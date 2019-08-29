using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Devices;
using GSVM.Constructs;
using GSVM.Constructs.DataTypes;

namespace GSVM.Components.Controllers
{
    public class Southbridge
    {
        MemoryMap<int> ports;
        List<IGenericDeviceBus> devices;

        bool[] hasDrive;

        public Southbridge(DiskDrive cmos)
        {
            devices = new List<IGenericDeviceBus>();
            ports = new MemoryMap<int>();
            hasDrive = new bool[5];

            InitializePorts();
            AddDrive(cmos);
        }

        /// <summary>
        /// Initializes the ports with a total of 5 disk drive ports, the first is CMOS. Everything else should be GSB.
        /// </summary>
        public virtual void InitializePorts()
        {
            int index = 0;
            
            for (int i = 0; i < 32; i++)
            {
                int length = 6;
                if (i < 5)
                    length = 45;

                ports.Map(i, (uint)index, (uint)length);
                index = index + length;
            }

            devices.AddRange(new DummyDrive[] { new DummyDrive(), new DummyDrive(), new DummyDrive(),
                                                    new DummyDrive(), new DummyDrive() });
        }

        public bool AddDrive(DiskDrive drive)
        {
            for (int i = 0; i < 5; i++)
            {
                if (!hasDrive[i])
                {
                    hasDrive[i] = true;
                    devices[i] = drive;
                    return true;
                }
            }
            return false;
        }

        public void AddDevice(IGenericDeviceBus device)
        {
            devices.Add(device);
        }

        public void RemoveDevice(int port)
        {
            devices.RemoveAt(port);
        }

        public void ClockTick()
        {
            for (int i = 0; i < devices.Count; i++)
            {
                devices[i].ClockTick();
            }
        }

        public void WriteToPort(int port, byte[] data)
        {
            if (port >= devices.Count)
                throw new ArgumentOutOfRangeException();
            else
            {
                devices[port].WriteData = data;
                devices[port].ReadyToWrite = true;
            }
        }

        public void WriteToPort(int port, IDataType data)
        {
            if (port >= devices.Count)
                throw new ArgumentOutOfRangeException();
            else
            {
                devices[port].WriteData = data.ToBinary();
                devices[port].ReadyToWrite = true;
            }
        }

        public byte[] ReadFromPort(int port)
        {
            if (port >= devices.Count)
                throw new ArgumentOutOfRangeException();
            else if (!devices[port].ReadyToRead)
                throw new ArgumentException();
            else
            {
                devices[port].Acknowledge();
                return devices[port].ReadData;
            }
        }

        public T ReadFromPort<T>(int port)
            where T : IDataType
        {
            if (port >= devices.Count)
                throw new ArgumentOutOfRangeException();
            else if (!devices[port].ReadyToRead)
                throw new ArgumentException();
            else
            {
                devices[port].Acknowledge();
                T data = default(T);
                data.FromBinary(devices[port].ReadData);
                return data;
            }
        }
    }
}
