using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Components.Processors.CPU_1;
using GSVM.Constructs.DataTypes;
using GSVM.Components.Clocks;


namespace GSVM.Components.Processors
{
    public partial class CPU1 : CPU
    {
        void ALUCopyFlags()
        {
            if (alu.flags.HasFlag(ALUFlags.Overflow))
                SetFlag(CPUFlags.ArithmeticOverflow);
            else
                UnsetFlag(CPUFlags.ArithmeticOverflow);
        }

        void ALUCopyCompareFlags()
        {
            if ((alu.flags & ALUFlags.Equal) == ALUFlags.Equal)
                SetFlag(CPUFlags.Equal);
            else
                UnsetFlag(CPUFlags.Equal);

            if ((alu.flags & ALUFlags.GreaterThan) == ALUFlags.GreaterThan)
                SetFlag(CPUFlags.GreaterThan);
            else
                UnsetFlag(CPUFlags.GreaterThan);

            if ((alu.flags & ALUFlags.LessThan) == ALUFlags.LessThan)
                SetFlag(CPUFlags.LessThan);
            else
                UnsetFlag(CPUFlags.LessThan);
    }

        void ALUOperation(Register_t a, Register_t  b, ALU.ALUOperation op)
        {
            byte[] opA = registers.Read(a);
            byte[] opB = registers.Read(b);
            bool overflow;

            byte[] result = op(opA, opB, out overflow);
            registers.Write(a, result);
            ALUCopyFlags();
        }

        void ALUOperation(Register_t  a, ALU.ALUOperationUnary op)
        {
            byte[] opA = registers.Read(a);

            byte[] result = op(opA);
            registers.Write(a, result);
            ALUCopyFlags();
        }

        void ALUOperation(Register_t  a, uint16_t b, ALU.ALUOperation op)
        {
            byte[] opA = registers.Read(a);
            byte[] opB = b.ToBinary();
            bool overflow;

            byte[] result = op(opA, opB, out overflow);
            registers.Write(a, result);
            ALUCopyFlags();
        }

        void Add(Register_t  a, Register_t  b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.Add));
        }

        void Add(Register_t  a, uint16_t b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.Add));
        }

        void Subtract(Register_t  a, Register_t  b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.Subtract));
        }

        void Subtract(Register_t  a, uint16_t b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.Subtract));
        }

        void Multiply(Register_t  a, Register_t  b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.Multiply));
        }

        void Multiply(Register_t  a, uint16_t b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.Multiply));
        }

        void Divide(Register_t  a, Register_t  b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.Divide));
        }

        void Divide(Register_t  a, uint16_t b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.Divide));
        }

        void Mod(Register_t  a, Register_t  b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.Mod));
        }

        void Mod(Register_t  a, uint16_t b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.Mod));
        }

        void And(Register_t  a, Register_t  b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.And));
        }

        void And(Register_t  a, uint16_t b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.And));
        }

        void Or(Register_t  a, Register_t  b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.Or));
        }

        void Or(Register_t  a, uint16_t b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.Or));
        }

        void Xor(Register_t  a, Register_t  b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.Xor));
        }

        void Xor(Register_t  a, uint16_t b)
        {
            ALUOperation(a, b, new ALU.ALUOperation(alu.Xor));
        }

        void Not(Register_t  a)
        {
            ALUOperation(a, new ALU.ALUOperationUnary(alu.Not));
        }

        void Neg(Register_t  a)
        {
            ALUOperation(a, new ALU.ALUOperationUnary(alu.Negate));
        }

        void Compare(Register_t a, Register_t b)
        {
            byte[] opA = registers.Read(a);
            byte[] opB = registers.Read(b);

            alu.Compare(opA, opB);

            ALUCopyCompareFlags();
        }

        void Compare(Register_t a, uint16_t b)
        {
            byte[] opA = registers.Read(a);
            byte[] opB = b.ToBinary();

            alu.Compare(opA, opB);

            ALUCopyCompareFlags();
        }
    }
}
