using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Constructs.DataTypes;

namespace GSVM.Constructs
{
    public class MemoryMap<T>
    {
        Dictionary<T, SmartPointer> map;

        public uint Length
        {
            get
            {
                uint max = 0;

                SmartPointer[] values = map.Values.ToArray();
                for (int i = 0; i < values.Length; i++)
                {
                    uint size = values[i].Address + values[i].Length;
                    if (size > max)
                        max = size;
                }

                return max;
            }
        }

        public MemoryMap()
        {
            map = new Dictionary<T, SmartPointer>();
        }

        public void Clear()
        {
            map.Clear();
        }

        public void Map(T name, SmartPointer ptr)
        {
            if (map.ContainsKey(name))
                throw new ArgumentException("A mapped range with that name already exists.");
            map.Add(name, ptr);
        }

        public void Map(T name, uint address, uint length)
        {
            if (map.ContainsKey(name))
                throw new ArgumentException("A mapped range with that name already exists.");
            map.Add(name, new SmartPointer(address, length));
        }

        public void Map(T name, IDataType type)
        {
            try
            {
                Map(name, type.Pointer);
            }
            catch
            {
                throw;
            }
        }

        public void Remap(T name, SmartPointer ptr)
        {
            if (!map.ContainsKey(name))
                throw new ArgumentException("A mapped range with that name does not exist.");
            map[name] = ptr;
        }

        public void Remap(T name, uint address, uint length)
        {
            if (!map.ContainsKey(name))
                throw new ArgumentException("A mapped range with that name does not exist.");
            map[name] = new SmartPointer(address, length);
        }

        public void Remap(T name, IDataType type)
        {
            try
            {
                Remap(name, type.Pointer);
            }
            catch
            {
                throw;
            }
        }

        public SmartPointer Lookup(T name)
        {
            if (map.ContainsKey(name))
            {
                return map[name];
            }
            else
            {
                throw new KeyNotFoundException("A mapped range with that name does not exist.");
            }
        }
    }
}
