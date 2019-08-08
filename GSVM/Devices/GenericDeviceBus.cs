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
        IDataType ReadData { get; }

        /// <summary>
        /// Gets or sets whether the WriteData port is ready be read from.
        /// </summary>
        bool ReadyToWrite { get; set; }

        /// <summary>
        /// Gets or sets the data to write.
        /// </summary>
        IDataType WriteData { get; set; }

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

        IDataType IGenericDeviceBus.ReadData
        {
            get
            {
                return ReadData;
            }
        }

        public bool ReadyToWrite { get; set; }

        public TRequest WriteData { get; protected set; }

        IDataType IGenericDeviceBus.WriteData
        {
            get
            {
                return WriteData;
            }
            set
            {
                WriteData = (TRequest)value;
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
