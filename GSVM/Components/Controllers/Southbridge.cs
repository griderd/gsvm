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
    public class Southbridge : GenericSerialBus
    {
        internal Northbridge northbridge;
        //MemoryMap<int> ports;
        List<IGenericDeviceBus> devices;
        Dictionary<int, int> interruptChannels;

        public Southbridge(DiskDrive cmos)
        {
            deviceType = 1;
            GenerateID();
            devices = new List<IGenericDeviceBus>();
            //ports = new MemoryMap<int>();
            interruptChannels = new Dictionary<int, int>();

            InitializePorts();
            AddDevice(cmos);
        }

        /// <summary>
        /// Initializes the ports with a total of 5 disk drive ports, the first is CMOS. Everything else should be GSB.
        /// Port 5 is reserved for the Southbridge.
        /// </summary>
        public virtual void InitializePorts()
        {
            int index = 0;
            
            for (int i = 0; i < 32; i++)
            {
                int length = 6;
                if (i < 5)
                    length = 45;

                //ports.Map(i, (uint)index, (uint)length);
                index = index + length;
                interruptChannels.Add(i, 0);
            }

            AddDevice(this);
        }

        public void AddDevice(IGenericDeviceBus device)
        {
            devices.Add(device);
        }

        public void RemoveDevice(int port)
        {
            devices.RemoveAt(port);
        }

        public override void ClockTick()
        {
            if (ReadyToWrite)
            {
                interruptChannels[WriteData.LowerWord] = WriteData.UpperWord;
            }

            for (int i = 0; i < devices.Count; i++)
            {
                if (devices[i] is Southbridge)
                    continue;

                devices[i].ClockTick();
                //if (i != 5)
                //{
                //    if (devices[i].Interrupt)
                //    {
                //        northbridge.Interrupt(interruptChannels[i]);
                //        devices[i].Interrupt = false;
                //    }
                //    devices[i].ClockTick();
                //}
            }

            base.ClockTick();
        }

        public void WriteToPort(int port, byte[] data)
        {
            if (data.Length == 0)
                return;

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
                T data = (T)Activator.CreateInstance<T>();
                ((T)data).FromBinary(devices[port].ReadData);
                return data;
            }
        }

        protected override bool InterruptChannelOk(uint channel)
        {
            return true;
        }
    }
}
