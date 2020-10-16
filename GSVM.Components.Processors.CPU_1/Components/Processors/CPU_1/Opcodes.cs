using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Constructs;
using GSVM.Constructs.DataTypes;

namespace GSVM.Components.Processors.CPU_1
{
    public class Opcode : IDataType
    {
        SmartPointer ptr;
        public SmartPointer Pointer { get { return ptr; } }

        public uint Address { get { return ptr.Address; } set { ptr.Address = value; } }

        public uint Length { get { return ptr.Length; } }

        Opcodes opcode;
        public Opcodes Code
        {
            get
            {
                return opcode;
            }
        }

        uint16_t operandA, operandB;
        OpcodeFlags flags;

        public uint16_t OperandA { get { return operandA; } }
        public uint16_t OperandB { get { return operandB; } }
        public OpcodeFlags Flags { get { return flags; } }

        public Opcode()
        {
            ptr = new SmartPointer(0, 8);
        }

        public Opcode(Opcodes opcode, OpcodeFlags flags, uint16_t opA, uint16_t opB)
        {
            this.opcode = opcode;

            operandA = opA;
            operandB = opB;
            this.flags = flags;

            ptr = new SmartPointer(0, 8);
        }

        public Opcode(byte[] values)
            : this()
        {
            FromBinary(values);
        }

        public Opcode(uint64_t value, uint16_t address)
            : this()
        {
            this.Address = address;
            FromBinary(value.ToBinary());
        }

        public void FromBinary(byte[] value)
        {
            if (value.Length != 8)
                throw new IndexOutOfRangeException();

            opcode = (Opcodes)BitConverter.ToUInt16(value, 0);
            flags = (OpcodeFlags)BitConverter.ToUInt16(value, 2);
            operandA = new uint16_t(BitConverter.ToUInt16(value, 4));
            operandB = new uint16_t(BitConverter.ToUInt16(value, 6));
        }

        public byte[] ToBinary()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes((UInt16)opcode));
            bytes.AddRange(BitConverter.GetBytes((UInt16)flags));
            bytes.AddRange(operandA.ToBinary());
            bytes.AddRange(operandB.ToBinary());

            return bytes.ToArray();
        }
    }

    [Flags]
    public enum OpcodeFlags : ushort
    {
        NoOperands = 0,
        Register1 = 1,
        Register2 = 2,
        Literal1 = 4,
        Literal2 = 8
    }

    public enum Opcodes : UInt16
    {
        /// <summary>
        /// No operation
        /// </summary>
        nop,
        /// <summary>Read from memory, address stored in register.</summary>
        readr,
        /// <summary>Read from memory, address stored in literal.</summary>
        readl,
        /// <summary>
        /// Write to memory, address stored in register, value stored as a register
        /// </summary>
        writerr,
        /// <summary>
        /// Write to memory, address stored as a register, value stored as a literal
        /// </summary>
        writerl,
        /// <summary>
        /// Write to memory, address stored as a literal, value stored as a register
        /// </summary>
        writelr,
        /// <summary>
        /// Write to memory, address stored as a literal, value stored as a literal
        /// </summary>
        writell,
        /// <summary>
        /// Push a register onto the stack
        /// </summary>
        pushr,
        /// <summary>
        /// Push a literal onto the stack
        /// </summary>
        pushl,
        /// <summary>
        /// Move the content of a register to another register
        /// </summary>
        movr,
        /// <summary>
        /// Move the content of a literal to another register
        /// </summary>
        movl,
        /// <summary>
        /// Add two registers together and store the result in the first register
        /// </summary>
        addr,
        /// <summary>
        /// Add a register to a literal and store the result in the register
        /// </summary>
        addl,
        /// <summary>
        /// Subtract two registers and store the result in the first register
        /// </summary>
        subr,
        /// <summary>
        /// Subtract a literal from a register and store the result in the register
        /// </summary>
        subl,
        /// <summary>
        /// Multiply two registers and store the result in the first register
        /// </summary>
        multr,
        /// <summary>
        /// Multiply a register and a literal and store the result in the register
        /// </summary>
        multl,
        /// <summary>
        /// Divide a register divided by another and store the result in the first
        /// </summary>
        divr,
        /// <summary>
        /// Divide a register divided by a literal and store the result in the register
        /// </summary>
        divl,
        /// <summary>
        /// Modulus a register divided by another and store the result in the first
        /// </summary>
        modr,
        /// <summary>
        /// Modulus a register divided by a literal and store the result in the register
        /// </summary>
        modl,
        /// <summary>
        /// Bitwise-AND two registers and store the result in the first
        /// </summary>
        andr,
        /// <summary>
        /// Bitwise-AND a register and a literal and store the result in the register
        /// </summary>
        andl,
        /// <summary>
        /// Bitwise-OR two registers and store the result in the first
        /// </summary>
        orr,
        /// <summary>
        /// Bitwise-OR a register and a literal and store the result in the register
        /// </summary>
        orl,
        /// <summary>
        /// Bitwise-XOR two registers and store the result in the first
        /// </summary>
        xorr,
        /// <summary>
        /// Bitwise-XOR a register and a literal and store the result in the register
        /// </summary>
        xorl,
        /// <summary>
        /// Invert a register
        /// </summary>
        not,
        /// <summary>
        /// Negate a register
        /// </summary>
        neg,
        /// <summary>
        /// Left shift
        /// </summary>
        lsl,
        /// <summary>
        /// Left shift
        /// </summary>
        lsr,
        /// <summary>
        /// Right shift
        /// </summary>
        rsl,
        /// <summary>
        /// Right shift
        /// </summary>
        rsr,
        /// <summary>
        /// Pop a value from the stack and store it in a register
        /// </summary>
        pop,
        /// <summary>
        /// Push all registers onto the stack
        /// </summary>
        pusha,
        /// <summary>
        /// Pop all registers from the stack
        /// </summary>
        popa,
        /// <summary>
        /// Halt processing
        /// </summary>
        hlt,
        /// <summary>
        /// Interrupt
        /// </summary>
        intr,
        /// <summary>
        /// Interrupt
        /// </summary>
        intl,
        /// <summary>
        /// Perform an unconditional jump
        /// </summary>
        jmpr,
        /// <summary>
        /// Perform an unconditional jump
        /// </summary>
        jmpl,
        /// <summary>
        /// Jump if equal flag is set
        /// </summary>
        je,
        /// <summary>
        /// Jump if equal flag is not set
        /// </summary>
        jne,
        /// <summary>
        /// Jump if greater flag is set
        /// </summary>
        jg,
        /// <summary>
        /// Jump if greater flag or equal flag is set
        /// </summary>
        jge,
        /// <summary>
        /// Jump if less than flag is set
        /// </summary>
        jl,
        /// <summary>
        /// Jump if less than flag or equal flag is set
        /// </summary>
        jle,
        /// <summary>
        /// Compare two registers
        /// </summary>
        cmpr,
        /// <summary>
        /// Compare a register to a literal
        /// </summary>
        cmpl,
        /// <summary>
        /// Performs a call
        /// </summary>
        callr,
        /// <summary>
        /// Performs a call
        /// </summary>
        calll,
        /// <summary>
        /// Return from a call
        /// </summary>
        ret,
        /// <summary>
        /// Dereferences the pointer in the second register and stores the result in the first.
        /// </summary>
        derefr,
        /// <summary>
        /// Dereferences the pointer in the literal and stores the result in the register
        /// </summary>
        derefl,
        /// <summary>
        /// Writes to the port in EAX the value at EBX of length ECX.
        /// </summary>
        _out,
        /// <summary>
        /// Reads from the port in EAX to the value at EBX of length ECX.
        /// </summary>
        /// <remarks>ECX is maximum buffer size. If the incoming data exceeds this length, the excess is not written. This is to prevent buffer overruns.</remarks>
        _in,
        /// <summary>
        /// Inserts a programmatic breakpoint, forcing the CPU into debug mode
        /// </summary>
        brk,
        /// <summary>
        /// Returns cpu information
        /// </summary>
        /// <remarks>
        /// Returns the following information based on the value in EAX:
        /// 0 - Returns the CPU name as a 12-character ASCII string in EBX, ECX, and EDX
        /// 1 - Returns the CPU speed as a 12-character ASCII string in EBX, ECX, and EDX
        /// </remarks>
        cpuid,
        /// <summary>
        /// Set Array (Literal, Literal). Sets the array base pointer and element length. Sets the index to 0.
        /// </summary>
        sall,
        /// <summary>
        /// Set Array (Literal, Register)
        /// </summary>
        salr,
        /// <summary>
        /// Set Array (Register, Literal)
        /// </summary>
        sarl,
        /// <summary>
        /// Set Array (Literal, Literal)
        /// </summary>
        sarr,
        /// <summary>
        /// Increment array pointer to the next element
        /// </summary>
        inca,
        /// <summary>
        /// Decrement array pointer to the previous element
        /// </summary>
        deca,
        /// <summary>
        /// Set Array Index (Literal), sets the Array Element Index (AEI) and updates the Array Element Pointer (AEP) at the same time.
        /// </summary>
        sail,
        /// <summary>
        /// Set Array Index (Register)
        /// </summary>
        sair
    }

    public enum Register : UInt16
    {
        /// <summary>
        /// Program counter
        /// </summary>
        PC,
        /// <summary>
        /// Memory address register
        /// </summary>
        MAR,
        /// <summary>
        /// Memory length register
        /// </summary>
        MLR,
        /// <summary>
        /// Memory data register (64-bit)
        /// </summary>
        MDR,
        /// <summary>
        /// Memory data register (32-bit)
        /// </summary>
        MDR32,
        /// <summary>
        /// Memory data register (16-bit)
        /// </summary>
        MDR16,
        /// <summary>
        /// Memory data register (8-bit)
        /// </summary>
        MDR8,
        /// <summary>
        /// Current instruction register
        /// </summary>
        CIR,
        /// <summary>
        /// Interrupt descriptor table
        /// </summary>
        IDT,
        /// <summary>
        /// Flags register
        /// </summary>
        FLAGS,
        /// <summary>
        /// Shared video memory address
        /// </summary>
        SVM,
        /// <summary>
        /// 32-bit register A
        /// </summary>
        EAX,
        /// <summary>
        /// 16-bit register A
        /// </summary>
        AX,
        /// <summary>
        /// Lower 8-bit register A
        /// </summary>
        AL,
        /// <summary>
        /// Upper 8-bit register A
        /// </summary>
        AH,
        /// <summary>
        /// 32-bit register B
        /// </summary>
        EBX,
        /// <summary>
        /// 16-bit register B
        /// </summary>
        BX, 
        /// <summary>
        /// Lower 8-bit register B
        /// </summary>
        BL,
        /// <summary>
        /// Upper 8-bit register B
        /// </summary>
        BH,
        /// <summary>
        /// 32-bit register C
        /// </summary>
        ECX,
        /// <summary>
        /// 16-bit register C
        /// </summary>
        CX,
        /// <summary>
        /// Lower 8-bit register C
        /// </summary>
        CL,
        /// <summary>
        /// Upper 8-bit register C
        /// </summary>
        CH,
        /// <summary>
        /// 32-bit register D
        /// </summary>
        EDX,
        /// <summary>
        /// 16-bit register D
        /// </summary>
        DX,
        /// <summary>
        /// Lower 8-bit register D
        /// </summary>
        DL,
        /// <summary>
        /// Upper 8-bit register D
        /// </summary>
        DH,
        /// <summary>
        /// Array base pointer
        /// </summary>
        ABP,
        /// <summary>
        /// Array element index
        /// </summary>
        AEI,
        /// <summary>
        /// Array element length
        /// </summary>
        AEL,
        /// <summary>
        /// Array element pointer
        /// </summary>
        AEP
    }
}
