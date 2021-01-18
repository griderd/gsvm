using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Components.Clocks;
using GSVM.Constructs;
using GSVM.Constructs.DataTypes;

namespace GSVM.Components.Controllers
{
    public class Northbridge
    {
        public CPU CPU { get; private set; }
        public IMemory Memory { get; private set; }
        public GraphicsCard Graphics { get; set; }
        public DebugStepping DebugClock { get; private set; }
        public ClockGenerator Clock { get; set; }
        protected Southbridge southbridge;

        public uint videoMemoryAddress = 0;

        public MemoryMap<MemoryAccessData> MemoryMap { get; private set; }

        private Northbridge()
        {

        }

        public Northbridge(CPU cpu, Southbridge southbridge, IMemory ram, GraphicsCard graphics) : this()
        {
            CPU = cpu;
            cpu.Northbridge = this;
            this.southbridge = southbridge;
            this.southbridge.northbridge = this;

            Memory = ram;
            Graphics = graphics;

            MemoryMap = new MemoryMap<MemoryAccessData>();
        }

        public virtual void ClockTick()
        {
            if ((CPU.Enabled) & (!CPU.Busy)) CPU.Tick();
            southbridge.ClockTick();

            if (Graphics != null)
            {
                WriteDisplay(0, ReadMemory(videoMemoryAddress, Graphics.Length));
            }
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
            try
            {
                return Memory.Read(address, length);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Writes the provided value to the provided address. Will limit the write to the provided buffer size.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <param name="bufferSize"></param>
        public void WriteMemory(uint address, byte[] value, uint bufferSize)
        {
            if (CanWriteMemory(address, (uint)value.Length))
                Memory.Write(address, value, bufferSize);
            else
                throw new MemoryAccessException(new SmartPointer(address, (uint)value.Length), true);
        }

        public void WriteDisplay(uint address, byte[] value)
        {
            Graphics.Write(address, value);
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

        // Changed uint32_t to uint in order to support non-standard types on the processor side.
        public void WriteToPort(uint port, uint addr, uint len)
        {
            try
            {
                byte[] value = ReadMemory(addr, len);
                southbridge.WriteToPort((int)port, value);
            }
            catch
            {
            }
        }

        // Changed uint32_t to uint in order to support non-standard types on the processor side.
        public void ReadFromPort(uint port, uint addr, uint bufferSize)
        {
            try
            {
                byte[] value = southbridge.ReadFromPort((int)port);
                WriteMemory(addr, value, bufferSize);
            }
            catch
            {
            }
        }

        public void Interrupt(int channel)
        {
            CPU.Interrupt(channel);
        }
    }
}
