using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Components.Controllers
{
    public abstract class GraphicsCard
    {
        protected Memory memory;

        public uint Length { get; private set; }

        public GraphicsCard(Memory memory)
        {
            this.memory = memory;
            Length = memory.Length;
        }

        public void Write(uint address, byte[] value)
        {
            memory.Write(address, value);
        }
    }
}