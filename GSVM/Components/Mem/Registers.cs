using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Constructs;
using GSVM.Constructs.DataTypes;

namespace GSVM.Components.Mem
{
    public class Registers<TKey>
    {
        Memory memory;
        MemoryMap<TKey> map;

        public Registers()
        {
            map = new MemoryMap<TKey>();
        }

        public void Map(TKey name, uint address, uint length)
        {
            try
            {
                map.Map(name, address, length);
            }
            catch
            {
                throw;
            }
        }

        public void Map(TKey name, SmartPointer ptr)
        {
            try
            {
                map.Map(name, ptr);
            }
            catch
            {
                throw;
            }
        }

        public void Map<T>(TKey name, uint address)
            where T : IDataType
        {
            try
            {
                T value = Activator.CreateInstance<T>();
                value.Address = address;
                map.Map(name, value);
            }
            catch
            {
                throw;
            }
        }

        public void Map(TKey name, IDataType value)
        {
            try
            {
                map.Map(name, value);
            }
            catch
            {
                throw;
            }
        }

        public void Subdivide(TKey register, TKey lower, TKey upper)
        {
            SmartPointer ptr = map.Lookup(register);
            uint lowLen = ptr.Length / 2;
            uint upLen = ptr.Length - lowLen;
            Map(lower, ptr.Address, lowLen);
            Map(upper, ptr.Address + lowLen, upLen);
        }

        public void Subdivide(TKey register, TKey lower)
        {
            SmartPointer ptr = map.Lookup(register);
            uint lowLen = ptr.Length / 2;
            uint upLen = ptr.Length - lowLen;
            Map(lower, ptr.Address, lowLen);
        }

        public void Append(TKey name, uint length)
        {
            try
            {
                map.Map(name, map.Length, length);
            }
            catch
            {
                throw;
            }
        }

        public void Append(TKey name, SmartPointer ptr)
        {
            try
            {
                Append(name, ptr.Length);
            }
            catch
            {
                throw;
            }
        }

        public void Append<T>(TKey name)
            where T : IDataType
        {
            try
            {
                T value = Activator.CreateInstance<T>();
                Append(name, value.Length);
            }
            catch
            {
                throw;
            }
        }

        public void Append(TKey name, IDataType value)
        {
            try
            {
                Append(name, value.Length);
            }
            catch
            {
                throw;
            }
        }

        public void Clear()
        {
            map.Clear();
        }

        public void BuildMemory()
        {
            memory = new Memory(map.Length);
        }

        public T Read<T>(TKey register)
            where T : IDataType
        {
            return memory.Read<T>(map.Lookup(register));
        }

        public byte[] Read(TKey register)
        {
            return memory.Read(map.Lookup(register));
        }

        public void Write<T>(TKey register, T value)
            where T : IDataType
        {
            memory.Write(map.Lookup(register), value);
        }

        public void Write(TKey register, byte[] value)
        {
            memory.Write(map.Lookup(register).Address, value);
        }

        public void Move(TKey to, TKey from)
        {
            SmartPointer pTo = map.Lookup(to);
            memory.Copy(map.Lookup(from).Address, pTo.Address, pTo.Length);
        }

        public SmartPointer Lookup(TKey name)
        {
            return map.Lookup(name);
        }

        public uint16_t SizeOf(TKey name)
        {
            SmartPointer ptr = Lookup(name);
            return (ushort)ptr.Length;
        }

        public byte[] Dump()
        {
            return memory.Read(0, memory.Length);
        }

        public void LoadState(byte[] state)
        {
            memory.Write(0, state);
        }
    }
}
