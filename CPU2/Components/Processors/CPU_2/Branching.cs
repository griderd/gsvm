using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GSVM.Constructs.FastDataTypes;
using GSVM.Components.Processors.CPU_2;


namespace GSVM.Components.Processors
{
    public partial class CPU2 : CPU
    {
        void Brk()
        {
            Debug = true;
        }

        void Ret()
        {
            Pop(Register.PC);
        }

        void CallR(Register_t a)
        {
            PushRegister(Register.PC);
            MoveR(Register.PC, a);
        }

        void CallL(uint16_t a)
        {
            PushRegister(Register.PC);
            MoveL(Register.PC, a);
        }

        void JumpR(Register_t a)
        {
            MoveR(Register.PC, a);
        }

        void JumpL(uint16_t a)
        {
            MoveL(Register.PC, a);
        }

        void JumpEqual(uint16_t a)
        {
            if (HasFlag(CPUFlags.Equal))
                JumpL(a);
        }

        void JumpNotEqual(uint16_t a)
        {
            if (!HasFlag(CPUFlags.Equal))
                JumpL(a);
        }

        void JumpGreater(uint16_t a)
        {
            if (HasFlag(CPUFlags.GreaterThan))
                JumpL(a);
        }

        void JumpGreaterEqual(uint16_t a)
        {
            if (HasFlag(CPUFlags.GreaterThan) | HasFlag(CPUFlags.Equal))
                JumpL(a);
        }

        void JumpLess(uint16_t a)
        {
            if (HasFlag(CPUFlags.LessThan))
                JumpL(a);
        }

        void JumpLessEqual(uint16_t a)
        {
            if (HasFlag(CPUFlags.LessThan) | HasFlag(CPUFlags.Equal))
                JumpL(a);
        }
    }
}
