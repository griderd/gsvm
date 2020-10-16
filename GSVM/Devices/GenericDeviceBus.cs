using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Constructs.DataTypes;
using Medallion;

namespace GSVM.Devices
{
    public interface IGenericDeviceBus : IDisposable
    {
        bool Interrupt { get; set; }

        /// <summary>
        /// Gets whether the ReadData port is ready to be read from.
        /// </summary>
        bool ReadyToRead { get; }
        /// <summary>
        /// Data to read from the device to the host.
        /// </summary>
        byte[] ReadData { get; set; }

        /// <summary>
        /// Gets or sets whether the WriteData port is ready be read from.
        /// </summary>
        bool ReadyToWrite { get; set; }

        /// <summary>
        /// Gets or sets the data to write from the host to the device.
        /// </summary>
        byte[] WriteData { get; set; }

        void ClockTick();

        int RequestSize { get; }

        void Acknowledge();
    }

    public abstract class GenericDeviceBus<TRequest> : IGenericDeviceBus
        where TRequest : GenericSerialRequest 
    {
        public const byte READ = 0;
        public const byte WRITE = 1;
        public const byte ACK = 2;
        public const byte HS = 3;
        public const byte HSACK = 4;
        public const byte ERROR = 5;

        public ushort DeviceType { get { return deviceType; } }
        protected ushort deviceType;

        /// <summary>
        /// A (hopefully) unique device ID, randomly generated at start.
        /// </summary>
        public ushort DeviceID { get { return deviceID; } }
        protected ushort deviceID;

        public int RequestSize
        {
            get
            {
                TRequest request = default;
                return (int)request.Length;
            }
        }

        /// <summary>
        /// Gets whether the ReadData port is ready to be read from.
        /// </summary>
        public bool ReadyToRead { get; protected set; }

        /// <summary>
        /// Data to read from the device to the host.
        /// </summary>
        public TRequest ReadData { get { return readData; } protected set { readData = value; } }
        protected TRequest readData;

        byte[] IGenericDeviceBus.ReadData
        {
            get
            {
                return readData.ToBinary();
            }
            set
            {
                TRequest data = (TRequest)Activator.CreateInstance<TRequest>();
                ((TRequest)data).FromBinary(value);
                readData = data;
            }
        }

        /// <summary>
        /// Gets or sets whether the WriteData port is ready be read from.
        /// </summary>
        public bool ReadyToWrite { get; set; }

        /// <summary>
        /// Gets or sets the data to write from the host to the device.
        /// </summary>
        public TRequest WriteData { get { return writeData; } set { writeData = value; } }
        protected TRequest writeData;

        byte[] IGenericDeviceBus.WriteData
        {
            get
            {
                return writeData.ToBinary();
            }
            set
            {
                TRequest data = (TRequest)Activator.CreateInstance<TRequest>();
                ((TRequest)data).FromBinary(value);
                writeData = data;
            }
        }

        public bool Interrupt { get; set; }

        public GenericDeviceBus()
        {
            
        }

        protected void GenerateID()
        {
            deviceID = (ushort)Rand.Next(0, ushort.MaxValue);
        }

        protected abstract bool InterruptChannelOk(uint channel);

        /// <summary>
        /// Performs a hardware handshake
        /// </summary>
        /// <remarks>
        /// To extract the data, read CX for the Device Type, then perform a 16-bit right shift on ECX, then read CX for Device ID.
        /// </remarks>
        protected void Handshake()
        {
            if (InterruptChannelOk(WriteData.data))
            {
                readData.control = HSACK;   // HSACK
                List<byte> _data = new List<byte>();
                _data.AddRange(BitConverter.GetBytes(DeviceType));     
                _data.AddRange(BitConverter.GetBytes(DeviceID));       
                readData.BinaryData = _data.ToArray();
            }
            else
            {
                readData.control = ERROR;   // ERROR
            }
        }

        public virtual void ClockTick()
        {
            if (ReadyToWrite)
            {
                if (readData == null)
                    readData = (TRequest)Activator.CreateInstance<TRequest>();

                switch (writeData.control)
                {
                    // ACK
                    case ACK:
                        Acknowledge();
                        break;

                    // HS - Handshake
                    case HS:
                        Handshake();
                        break;
                }

                ReadyToRead = true;
                ReadyToWrite = false;
            }
        }

        public void Acknowledge()
        {
            ReadyToRead = false;
            Interrupt = false;
        }

        public virtual void Dispose()
        {
        }
    }
}
