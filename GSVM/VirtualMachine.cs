using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Components;

namespace GSVM
{
    public class VirtualMachine
    {
        public Memory memory;
        internal CPU cpu;

        public bool Debug
        {
            get
            {
                return cpu.Debug;
            }
            set
            {
                cpu.Debug = value;
            }
        }

        public event EventHandler UpdateDebugger;

        public VirtualMachine(CPU cpu, uint memorySize)
        {
            memory = new Memory(memorySize);
            this.cpu = cpu;
            this.cpu.Parent = this;
        }

        public void Start()
        {
            cpu.Start();
        }

        public void Stop()
        {
            cpu.Stop();
        }

        public byte[] GetRegisters()
        {
            return cpu.GetRegisters();
        }

        public void LoadMemory(byte[] data)
        {
            memory.Write(0, data);
        }

        public byte[] CoreDump()
        {
            return memory.Read(0, memory.Length);
        }

        public void DebugStep()
        {
            cpu.DebugStep();
        }

        public void RaiseUpdateDebugger()
        {
            UpdateDebugger?.Invoke(this, new EventArgs());
        }
    }
}
