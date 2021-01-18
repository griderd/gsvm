using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Constructs;
using GSVM.Constructs.DataTypes;

namespace GSVM.Components
{
    public interface IMemory
    {
        public bool ReadOnly { get; }
        public uint Length { get; }
        public string Checksum { get; }

        public byte Read(uint address);
        public byte[] Read(uint address, uint length);
        public byte[] Read(SmartPointer ptr);
        public TDataType Read<TDataType>(uint address)
            where TDataType : IDataType;

        /// <summary>Reads data from memory and converts it to the provided type. Includes built-in type safety by matching the requested type with the requested length from the SmartPointer.</summary>
        /// <typeparam name="TDataType">Data type requested.</typeparam>
        /// <param name="ptr">A pointer containing the address and length of the requested data.</param>
        /// <exception cref="Exception">Thrown if the length of the pointer is not equal to that of the data type.</exception>
        public TDataType Read<TDataType>(SmartPointer ptr)
            where TDataType : IDataType;
        public void Write(uint address, byte value);
        public void Write(uint address, byte[] value, uint bufferSize = uint.MaxValue);
        public void Write(IDataType value);
        public void Write(uint address, IDataType value);
        public void Write(SmartPointer ptr, IDataType value);
        public void Write(SmartPointer ptr, IIntegral value);
        public void Copy(uint fromAddress, uint fromLen, uint toAddress, uint toLen);
    }
}
