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
        /// <summary>
        /// Moves a value from one register to another.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        void MoveR(Register_t  to, Register_t  from)
        {
            registers.Move(to, from);
        }

        /// <summary>
        /// Moves a literal into a register.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="literal"></param>
        void MoveL(Register_t to, uint16_t literal)
        {
            registers.Write(to, literal);
        }

        /// <summary>
        /// Reads the value stored in memory at address in regB into regA.
        /// </summary>
        /// <param name="regA">Destination of data</param>
        /// <param name="regB">Memory address</param>
        void Read(Register_t  regA, Register_t  regB)
        {
            MoveL(Register.MLR, registers.SizeOf(regA));
            MoveR(Register.MAR, regB);
            ReadMemory();
            MoveR(regA, Register.MDR);
        }

        /// <summary>
        /// Reads the value stored in memory at address into reg.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="literal"></param>
        void Read(Register_t  reg, uint16_t literal)
        {
            MoveL(Register.MLR, 2);
            MoveL(Register.MAR, literal);
            ReadMemory();
            MoveR(reg, Register.MDR);
        }

        /// <summary>
        /// Writes the value stored in regB to memory at address in regA
        /// </summary>
        /// <param name="regA"></param>
        /// <param name="regB"></param>
        void Write(Register_t  regA, Register_t  regB)
        {
            MoveR(Register.MAR, regA);
            MoveL(Register.MLR, registers.SizeOf(regB));
            MoveR(Register.MDR, regB);
            WriteMemory();
        }

        /// <summary>
        /// Writes the literal to the memory at address in reg
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="literal"></param>
        void Write(Register_t  reg, uint16_t literal)
        {
            MoveR(Register.MAR, reg);
            MoveL(Register.MLR, 2);
            MoveL(Register.MDR, literal);
            WriteMemory();
        }
    }
}
