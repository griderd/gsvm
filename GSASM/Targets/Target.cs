using System;
using System.Collections.Generic;
using System.Text;

namespace GSVM.Assembler.Targets
{
    public abstract class Target
    {
        protected Assembler asm;

        public abstract int Assemble(int tokenID);

        public Target(Assembler asm)
        {
            this.asm = asm;
        }
    }
}
