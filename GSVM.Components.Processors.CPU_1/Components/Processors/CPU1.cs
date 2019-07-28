using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GSVM.Components.Mem;
using GSVM.Components.Processors.CPU_1;
using GSVM.Constructs;
using GSVM.Constructs.DataTypes;
using GSVM.Components.Clocks;

namespace GSVM.Components.Processors
{
    public partial class CPU1 : CPU
    {
        ThreadedClock clock;

        Stack<IDataType> stack;

        Dictionary<Opcodes, Delegate> opcodes;

        Registers<Register> registers;

        Opcode opcode;
        Delegate operation;

        Integral<ushort> operandA, operandB;

        bool debug;
        public override bool Debug
        {
            get
            {
                return debug;
            }

            set
            {
                debug = value;
                if (debug)
                {
                    clock.Stop();
                }
                else
                {
                    clock.Start();
                }
            }
        }

        public CPU1()
        {
            clock = new ThreadedClock(this);
            stack = new Stack<IDataType>();
            opcodes = new Dictionary<Opcodes, Delegate>();

            registers = new Registers<Register>();

            registers.Append<uint16_t>(Register.PC);       // Program counter
            registers.Append<uint16_t>(Register.MAR);      // Memory address register
            registers.Append<uint16_t>(Register.MLR);      // Memory length register

            registers.Append<uint64_t>(Register.MDR);      // Memory data register (64-bits)
            registers.Subdivide(Register.MDR, Register.MDR32);    // 32-bit MDR register
            registers.Subdivide(Register.MDR32, Register.MDR16);  // 16-bit MDR register
            registers.Subdivide(Register.MDR16, Register.MDR8);   // 8-bit MDR register

            registers.Append<Opcode>(Register.CIR);      // Current instruction register

            registers.Append<uint16_t>(Register.IDT);   // Interrupt Descriptor Table address

            registers.Append<uint64_t>(Register.FLAGS); // Flags register

            registers.Append<uint32_t>(Register.EAX);
            registers.Subdivide(Register.EAX, Register.AX);
            registers.Subdivide(Register.AX, Register.AL, Register.AH);

            registers.Append<uint32_t>(Register.EBX);
            registers.Subdivide(Register.EBX, Register.BX);
            registers.Subdivide(Register.BX, Register.BL, Register.BH);

            registers.Append<uint32_t>(Register.ECX);
            registers.Subdivide(Register.ECX, Register.CX);
            registers.Subdivide(Register.CX, Register.CL, Register.CH);

            registers.Append<uint32_t>(Register.EDX);
            registers.Subdivide(Register.EDX, Register.DX);
            registers.Subdivide(Register.DX, Register.DL, Register.DH);


            registers.BuildMemory();
            SetupOpCodes();

            registers.Write<uint64_t>(Register.FLAGS, new uint64_t((ulong)CPUFlags.Empty));
        }

        void SetupOpCodes()
        {
            opcodes.Add(Opcodes.nop, new Action(NoOp));
            opcodes.Add(Opcodes.movr, new Action<Register_t, Register_t>(MoveR));
            opcodes.Add(Opcodes.movl, new Action<Register_t, uint16_t>(MoveL));
            opcodes.Add(Opcodes.readr, new Action<Register_t, Register_t>(Read));
            opcodes.Add(Opcodes.readl, new Action<Register_t, uint16_t>(Read));
            opcodes.Add(Opcodes.writer, new Action<Register_t, Register_t>(Write));
            opcodes.Add(Opcodes.writel, new Action<Register_t, uint16_t>(Write));
            opcodes.Add(Opcodes.outl, new Action<uint16_t, Register_t>(Out));
            opcodes.Add(Opcodes.pushr, new Action<Register_t>(PushRegister));
            opcodes.Add(Opcodes.pushl, new Action<uint16_t>(PushLiteral));
            opcodes.Add(Opcodes.pusha, new Action(PushAll));
            opcodes.Add(Opcodes.pop, new Action<Register_t>(Pop));
            opcodes.Add(Opcodes.popa, new Action(PopAll));

            opcodes.Add(Opcodes.addr, new Action<Register_t, Register_t>(Add));
            opcodes.Add(Opcodes.addl, new Action<Register_t, uint16_t>(Add));
            opcodes.Add(Opcodes.subr, new Action<Register_t, Register_t>(Subtract));
            opcodes.Add(Opcodes.subl, new Action<Register_t, uint16_t>(Subtract));
            opcodes.Add(Opcodes.multr, new Action<Register_t, Register_t>(Multiply));
            opcodes.Add(Opcodes.multl, new Action<Register_t, uint16_t>(Multiply));
            opcodes.Add(Opcodes.divr, new Action<Register_t, Register_t>(Divide));
            opcodes.Add(Opcodes.divl, new Action<Register_t, uint16_t>(Divide));
            opcodes.Add(Opcodes.modr, new Action<Register_t, Register_t>(Mod));
            opcodes.Add(Opcodes.modl, new Action<Register_t, uint16_t>(Mod));
            opcodes.Add(Opcodes.andr, new Action<Register_t, Register_t>(And));
            opcodes.Add(Opcodes.andl, new Action<Register_t, uint16_t>(And));
            opcodes.Add(Opcodes.orr, new Action<Register_t, Register_t>(Or));
            opcodes.Add(Opcodes.orl, new Action<Register_t, uint16_t>(Or));
            opcodes.Add(Opcodes.xorr, new Action<Register_t, Register_t>(Xor));
            opcodes.Add(Opcodes.xorl, new Action<Register_t, uint16_t>(Xor));
            opcodes.Add(Opcodes.not, new Action<Register_t>(Not));
            opcodes.Add(Opcodes.neg, new Action<Register_t>(Neg));

            opcodes.Add(Opcodes.hlt, new Action(Halt));

            // intr
            // intl

            opcodes.Add(Opcodes.jmpr, new Action<Register_t>(JumpR));
            opcodes.Add(Opcodes.jmpl, new Action<uint16_t>(JumpL));
            opcodes.Add(Opcodes.je, new Action<uint16_t>(JumpEqual));
            opcodes.Add(Opcodes.jne, new Action<uint16_t>(JumpNotEqual));
            opcodes.Add(Opcodes.jg, new Action<uint16_t>(JumpGreater));
            opcodes.Add(Opcodes.jge, new Action<uint16_t>(JumpGreaterEqual));
            opcodes.Add(Opcodes.jl, new Action<uint16_t>(JumpLess));
            opcodes.Add(Opcodes.jle, new Action<uint16_t>(JumpLessEqual));
            opcodes.Add(Opcodes.ret, new Action(Ret));
        }

        public override byte[] GetRegisters()
        {
            return registers.Dump();
        }

        protected override void Fetch()
        {
            if (!HasFlag(CPUFlags.WaitForInterrupt))
            {
                MoveR(Register.MAR, Register.PC);
                MoveL(Register.MLR, 8);
                ReadMemory();
                MoveR(Register.CIR, Register.MDR);

                Add(Register.PC, Register.MLR);

                Parent.RaiseUpdateDebugger();
            }
        }

        protected override void Decode()
        {
            opcode = registers.Read<Opcode>(Register.CIR);
            operation = opcodes[opcode.Code];

            if (opcode.Flags.HasFlag(OpcodeFlags.Register1))
                operandA = new Register_t(opcode.OperandA);

            if (opcode.Flags.HasFlag(OpcodeFlags.Register2))
                operandB = new Register_t(opcode.OperandB);

            if (opcode.Flags.HasFlag(OpcodeFlags.Literal1))
                operandA = opcode.OperandA;

            if (opcode.Flags.HasFlag(OpcodeFlags.Literal2))
                operandB = opcode.OperandB;

        }

        protected override void Execute()
        {
            int arguments = operation.Method.GetParameters().Length;

            // TODO: Wrap in try-catch
            switch (arguments)
            {
                case 0:
                    operation.DynamicInvoke();
                    break;

                case 1:
                    operation.DynamicInvoke(operandA);
                    break;

                case 2:
                    operation.DynamicInvoke(operandA, operandB);
                    break;
            }

            Parent.RaiseUpdateDebugger();
        }

        public override void Start()
        {
            Enabled = true;
            clock.Start();
        }

        public override void Stop()
        {
            Enabled = false;
        }

        void NoOp()
        {
            // DO NOTHING
        }

        void Halt()
        {
            SetFlag(CPUFlags.WaitForInterrupt);
        }

        void SetFlag(CPUFlags flag)
        {
            CPUFlags state = (CPUFlags)registers.Read<uint64_t>(Register.FLAGS).Value;
            state = state | flag;
            registers.Write<uint64_t>(Register.FLAGS, (ulong)state);
        }

        void UnsetFlag(CPUFlags flag)
        {
            CPUFlags state = (CPUFlags)registers.Read<uint64_t>(Register.FLAGS).Value;
            state = state & ~flag;
            registers.Write<uint64_t>(Register.FLAGS, (ulong)state);
        }

        bool HasFlag(CPUFlags flag)
        {
            CPUFlags state = (CPUFlags)registers.Read<uint64_t>(Register.FLAGS).Value;
            return (state & flag) == flag;
        }

        protected void ReadMemory()
        {
            if (Parent == null)
                throw new Exception("Not connected to VM. Parent is null.");

            uint16_t address = registers.Read<uint16_t>(Register.MAR);
            uint16_t length = registers.Read<uint16_t>(Register.MLR);
            byte[] value = Parent.memory.Read(address.Value, length.Value);
            registers.Write(Register.MDR, value);
        }

        protected void WriteMemory()
        {
            if (Parent == null)
                throw new Exception("Not connected to VM. Parent is null.");

            uint16_t address = registers.Read<uint16_t>(Register.MAR);
            uint16_t length = registers.Read<uint16_t>(Register.MLR);

            byte[] value = new byte[0];

            switch (length.Value)
            {
                case 8:
                    value = registers.Read(Register.MDR);
                    break;

                case 4:
                    value = registers.Read(Register.MDR32);
                    break;

                case 2:
                    value = registers.Read(Register.MDR16);
                    break;

                case 1:
                    value = registers.Read(Register.MDR8);
                    break;
            }

            Parent.memory.Write(address, value);
        }
    }
}
