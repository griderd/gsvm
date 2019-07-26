using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Components.Processors.CPU_1;
using GSVM.Constructs.DataTypes;
using GSVM.Components.Clocks;
using GSVM.Constructs;

namespace GSVM.Components.Processors
{
    public partial class CPU1 : CPU
    {
        void PushRegister(Register_t reg)
        {
            int size = registers.SizeOf(reg);
            switch (size)
            {
                case 8:
                    stack.Push(registers.Read<uint64_t>(reg));
                    break;

                case 4:
                    stack.Push(registers.Read<uint32_t>(reg));
                    break;

                case 2:
                    stack.Push(registers.Read<uint16_t>(reg));
                    break;

                case 1:
                    stack.Push(registers.Read<uint8_t>(reg));
                    break;
            }
        }

        void PushLiteral(uint16_t literal)
        {
            stack.Push(literal);
        }

        void PushAll()
        {
            PushRegister(Register.EAX);
            PushRegister(Register.EBX);
            PushRegister(Register.ECX);
            PushRegister(Register.EDX);
        }

        void Pop(Register_t reg)
        {
            ushort size = registers.SizeOf(reg);

            if (stack.Peek().Length > size)
            {
                // Throw a value indicating that the source value is larger than the destination register
                throw new Exception("Source larger than destination.");
            }
            else
            {
                registers.Write(reg, stack.Pop().ToBinary());
            }
        }

        void PopAll()
        {
            Pop(Register.EDX);
            Pop(Register.ECX);
            Pop(Register.EBX);
            Pop(Register.EAX);
        }
    }
}
