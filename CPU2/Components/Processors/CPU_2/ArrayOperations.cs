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
        /// <summary>
        /// Set Array. Sets up the array registers for reading or writing to 
        /// </summary>
        /// <param name="baseAddress">Address of the array</param>
        /// <param name="elementSize">Size of the elements in bytes</param>
        void sall(uint16_t baseAddress, uint16_t elementSize)
        {
            registers.Write<uint16_t>(Register.ABP, baseAddress);
            registers.Write<uint16_t>(Register.AEI, 0);
            registers.Write<uint16_t>(Register.AEL, elementSize);
            registers.Write<uint16_t>(Register.AEP, baseAddress);
        }

        void sarl(Register_t baseAddress, uint16_t elementSize)
        {
            sall(registers.Read<uint32_t>(baseAddress).CastTo<uint16_t>(), elementSize);
        }

        void salr(uint16_t baseAddress, Register_t elementSize)
        {
            sall(baseAddress, registers.Read<uint32_t>(elementSize).CastTo<uint16_t>());
        }

        void sarr(Register_t baseAddress, Register_t elementSize)
        {
            sall(registers.Read<uint32_t>(baseAddress).CastTo<uint16_t>(), registers.Read<uint32_t>(elementSize).CastTo<uint16_t>());
        }

        /// <summary>
        /// Increments the array pointer
        /// </summary>
        void inca()
        {
            Add(Register.AEI, new uint16_t(1));
            sair(Register.AEI);
        }

        /// <summary>
        /// Decrements the array pointer
        /// </summary>
        void deca()
        {
            Subtract(Register.AEI, new uint16_t(1));
            sair(Register.AEI);
        }

        void sail(uint16_t index)
        {
            registers.Write<uint16_t>(Register.AEI, index);
            registers.Write<uint16_t>(Register.AEP, index);
            Multiply(Register.AEP, Register.AEL);
            Add(Register.AEP, Register.ABP);
        }

        void sair(Register_t index)
        {
            MoveR(Register.AEI, index);
            MoveR(Register.AEP, index);
            Multiply(Register.AEP, Register.AEL);
            Add(Register.AEP, Register.ABP);
        }
    }
}
