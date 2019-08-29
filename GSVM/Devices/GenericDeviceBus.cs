using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Constructs.DataTypes;

namespace GSVM.Devices
{
    public interface IGenericDeviceBus
    {
        /// <summary>
        /// Gets whether the ReadData port is ready to be read from.
        /// </summary>
        bool ReadyToRead { get; }
        /// <summary>
        /// Data to read from the device.
        /// </summary>
        byte[] ReadData { get; }

        /// <summary>
        /// Gets or sets whether the WriteData port is ready be read from.
        /// </summary>
        bool ReadyToWrite { get; set; }

        /// <summary>
        /// Gets or sets the data to write.
        /// </summary>
        byte[] WriteData { get; set; }

        void ClockTick();

        int RequestSize { get; }

        void Acknowledge();
    }

    public abstract class GenericDeviceBus<TRequest> : IGenericDeviceBus
        where TRequest : IDataType
    {
        public int RequestSize
        {
            get
            {
                TRequest request = default(TRequest);
                return (int)request.Length;
            }
        }

        public bool ReadyToRead { get; protected set; }

        public TRequest ReadData { get; protected set; }

        byte[] IGenericDeviceBus.ReadData
        {
            get
            {
                return ReadData.ToBinary();
            }
        }

        public bool ReadyToWrite { get; set; }

        public TRequest WriteData { get; protected set; }

        byte[] IGenericDeviceBus.WriteData
        {
            get
            {
                return WriteData.ToBinary();
            }
            set
            {
                TRequest data = default(TRequest);
                data.FromBinary(value);
                WriteData = data;
            }
        }

        public virtual void ClockTick()
        {
        }

        public void Acknowledge()
        {
            ReadyToRead = false;
        }
    }
}
