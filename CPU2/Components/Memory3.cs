using GSVM.Constructs;
using GSVM.Constructs.DataTypes;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace GSVM.Components
{
    public class Memory3 : IMemory
    {
        MemoryStream data;

        protected bool readOnly;
        public bool ReadOnly { get { return readOnly; } }

        public uint Length { get; private set; }

        public string Checksum
        {
            get
            {
                using (MD5 hash = MD5.Create())
                {
                    byte[] checksum = hash.ComputeHash(data);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < checksum.Length; i++)
                    {
                        sb.AppendFormat("{0:X2}", checksum[i]);
                    }

                    return sb.ToString();
                }
            }
        }

        public Memory3(uint size)
        {
            data = new MemoryStream((int)size);
            data.Write(new byte[size], 0, (int)size);
            Length = size;
        }

        public Memory3(byte[] data)
        {
            this.data = new MemoryStream(data);
            Length = (uint)data.LongLength;
        }

        public byte Read(uint address)
        {
            if (address < data.Length)
            {
                data.Position = address;
                return (byte)data.ReadByte();
            }
            else
            {
                // TODO: Change to a memory exception
                throw new IndexOutOfRangeException();
            }
        }

        public byte[] Read(uint address, uint length)
        {
            if ((address < this.data.Length) & (address + length - 1 < this.data.Length))
            {
                byte[] buffer = new byte[length];
                data.Position = (int)address;
                data.Read(buffer, 0, (int)length);
                return buffer;
            }
            else
            {
                // TODO: Change to a memory exception
                throw new IndexOutOfRangeException();
            }
        }

        public byte[] Read(SmartPointer ptr)
        {
            try
            {
                return Read(ptr.Address, ptr.Length);
            }
            catch
            {
                throw;
            }
        }

        public TDataType Read<TDataType>(uint address)
            where TDataType : IDataType
        {
            TDataType value = Activator.CreateInstance<TDataType>();

            value.Address = address;
            byte[] data = Read(value.Pointer);
            value.FromBinary(data);

            return value;
        }

        /// <summary>Reads data from memory and converts it to the provided type. Includes built-in type safety by matching the requested type with the requested length from the SmartPointer.</summary>
        /// <typeparam name="TDataType">Data type requested.</typeparam>
        /// <param name="ptr">A pointer containing the address and length of the requested data.</param>
        /// <exception cref="Exception">Thrown if the length of the pointer is not equal to that of the data type.</exception>
        public TDataType Read<TDataType>(SmartPointer ptr)
            where TDataType : IDataType
        {
            TDataType value = Activator.CreateInstance<TDataType>();

            // TODO: Replace with a type mismatch error
            if (value.Length != ptr.Length)
                throw new Exception("Type mismatch.");

            value.Address = ptr.Address;
            byte[] data = Read(value.Pointer);
            value.FromBinary(data);

            return value;
        }

        public void Write(uint address, byte value)
        {
            if (ReadOnly)
                throw new Exception("Memory is read only.");

            if (address < data.Length)
            {
                data.Position = address;
                data.WriteByte(value);
            }
            else
                throw new IndexOutOfRangeException();
        }

        public void Write(uint address, byte[] value, uint bufferSize = uint.MaxValue)
        {
            if (ReadOnly)
                throw new Exception("Memory is read only.");

            if ((address < this.data.Length) & (address + value.Length - 1 < this.data.Length))
            {
                data.Position = address;
                data.Write(value, 0, value.Length);
            }
            else
            {
                // TODO: Change to a memory exception
                throw new IndexOutOfRangeException();
            }
        }

        public void Write(IDataType value)
        {
            try
            {
                Write(value.Pointer.Address, value.ToBinary());
            }
            catch
            {
                throw;
            }
        }

        public void Write(uint address, IDataType value)
        {
            try
            {
                Write(address, value.ToBinary());
            }
            catch
            {
                throw;
            }
        }

        public void Write(SmartPointer ptr, IDataType value)
        {
            if (value is IIntegral val)
            {
                Write(ptr, val);
                return;
            }
            if (ptr.Length != value.Length)
                throw new Exception("Type mismatch.");

            try
            {
                Write(ptr.Address, value.ToBinary());
            }
            catch
            {
                throw;
            }
        }

        public void Write(SmartPointer ptr, IIntegral value)
        {
            IIntegral val = value;
            if (ptr.Length != value.Length)
            {
                val = value.CastTo(Int.BestFit((int)ptr.Length));
            }

            try
            {
                Write(ptr.Address, val.ToBinary());
            }
            catch
            {
                throw;
            }
        }

        public void Copy(uint fromAddress, uint fromLen, uint toAddress, uint toLen)
        {
            for (int i = 0; i < toLen; i++)
            {
                if (i < fromLen)
                {
                    byte b = Read((uint)(fromAddress + i));
                    Write((uint)(toAddress + i), b);
                }
                else
                {
                    Write((uint)(toAddress + i), (byte)0);
                }
            }
        }
    }
}
