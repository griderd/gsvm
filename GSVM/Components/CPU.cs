using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Components.Mem;
using GSVM.Constructs;
using GSVM.Constructs.DataTypes;
using GSVM.Components.Controllers;

namespace GSVM.Components
{
    public abstract class CPU
    {
        public bool Enabled { get; protected set; }
        public bool Busy { get; protected set; }
        public virtual bool Debug { get; set; }

        public int Ring { get; protected set; }

        public VirtualMachine Parent { get; set; }
        public Northbridge Northbridge { get; set; }

        protected ALU alu;

        public CPU()
        {
            alu = new ALU();
            Ring = 0;
        }

        /// <summary>Represents a tick for the external clock, starting an execution cycle.</summary>
        /// <remarks>
        /// The CPU class does not have an internal clock system. It is expected that you will implement one in your CPU implementation using a loop, or a timer of some sort. You may also use an external clock that is provided.
        /// </remarks>
        public virtual void Tick()
        {
            Busy = true;

            Fetch();
            Decode();
            Execute();

            Busy = false;
        }


        protected abstract void Fetch();
        protected abstract void Decode();
        protected abstract void Execute();

        public abstract void Start();
        public abstract void Stop();

        public void DebugStep()
        {
            
        }

        public abstract byte[] GetRegisters();

        public abstract void Interrupt(int channel);
    }
}
