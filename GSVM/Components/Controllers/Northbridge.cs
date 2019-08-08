using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Components.Clocks;
using GSVM.Constructs;

namespace GSVM.Components.Controllers
{
    public class Northbridge
    {
        public CPU CPU { get; private set; }
        public Memory Memory { get; private set; }
        public GraphicsCard Graphics { get; set; }
        public DebugStepping DebugClock { get; private set; }
        public ClockGenerator Clock { get; set; }
        protected Southbridge southbridge;

        public MemoryMap<MemoryAccessData> MemoryMap { get; private set; }

        private Northbridge()
        {

        }

        public Northbridge(CPU cpu, Southbridge southbridge, Memory ram, GraphicsCard graphics) : this()
        {
            CPU = cpu;
            cpu.Northbridge = this;
            this.southbridge = southbridge;

            Memory = ram;
            Graphics = graphics;

            MemoryMap = new MemoryMap<MemoryAccessData>();
        }

        public virtual void ClockTick()
        {
            if ((CPU.Enabled) & (!CPU.Busy)) CPU.Tick();
            Graphics?.ClockTick();
            southbridge.ClockTick();
        }

        public virtual void DebugTick()
        {
            if (CPU.Debug)
            {
                Clocks.DebugStepping.Step(this);
            }
        }

        public byte[] ReadMemory(uint address, uint length)
        {
            return Memory.Read(address, length);
        }

        public void WriteMemory(uint address, byte[] value)
        {
            if (CanWriteMemory(address, (uint)value.Length))
                Memory.Write(address, value);
            else
                throw new MemoryAccessException(new SmartPointer(address, (uint)value.Length), true);
        }

        bool CanWriteMemory(uint address, uint length)
        {
            SmartPointer ptr = new SmartPointer(address, length);

            MemoryAccessData[] mad = MemoryMap.FindEncompassingRanges(ptr);

            for (int i = 0; i < mad.Length; i++)
            {
                if (mad[i].ReadOnly)
                    return false;
                if (mad[i].Ring < CPU.Ring)
                    return false;
            }

            return true;
        }
    }
}
