using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using GSVM;
using GSVM.Components;
using GSVM.Components.Clocks;
using GSVM.Components.Processors;
using GSVM.Components.Mem;
using GSVM.Constructs.DataTypes;
using GSVM.Components.Processors.CPU_1;

using GSVM.Components.Processors.CPU_1.Assembler;

namespace VMDebugger
{
    public partial class frmRegisters : Form
    {
        VirtualMachine vm = new VirtualMachine(new CPU1(), 16384);
        byte[] sampleProgram = new byte[]
            {
                0x08, 0x00, 0x09, 0x00, 0x0B, 0x00, 0x01, 0x00,         // mov ax, 1
                0x08, 0x00, 0x09, 0x00, 0x0F, 0x00, 0x02, 0x00,         // mov bx, 2
                0x09, 0x00, 0x03, 0x00, 0x0B, 0x00, 0x0F, 0x00,         // add ax, bx
                0x1E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00          // hlt
            };

        string[] sampleCode = new string[]
            {
                "jmp main",
                "short a 1",
                "short b 2",
                "main:",
                "read ax, a",
                "read bx, b",
                "add ax, bx",
                "out b, ax",
                "read cx, b",
                "hlt",
            };

        Registers<Register> registers = new Registers<Register>();

        public frmRegisters()
        {
            InitializeComponent();
        }

        private void frmRegisters_Load(object sender, EventArgs e)
        {
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

            byte[] state = vm.GetRegisters();
            registers.LoadState(state);

            //vm.LoadMemory(sampleProgram);
            Assemble();

            UpdateList();
        }

        void Assemble()
        {
            Assembler asm = new Assembler();
            asm.AddSource(sampleCode);
            asm.Assemble();
            byte[] bin = asm.Binary;

            vm.LoadMemory(bin);
        }

        private void Vm_UpdateDebugger(object sender, EventArgs e)
        {
            UpdateDebugger();
        }

        void UpdateDebugger()
        {
            byte[] state = vm.GetRegisters();
            registers.LoadState(state);
            UpdateList();
        }

        void UpdateList()
        {
            lstRegisters.Items.Clear();
            lstInfo.Items.Clear();

            ListViewItem item;

            string[] names = Enum.GetNames(typeof(Register));
            for (int i = 0; i < names.Length; i++)
            {
                item = new ListViewItem(names[i]);
                item.SubItems.Add(Lookup((Register)i).ToString("X"));
                lstRegisters.Items.Add(item);
            }

            item = new ListViewItem("MAR+MLR");
            uint address = (uint)Lookup(Register.MAR);
            uint len = (uint)Lookup(Register.MLR);
            if (len > 0)
            {
                ulong value = Lookup(address, len);
                item.SubItems.Add(value.ToString("X"));
            }
            else
            {
                item.SubItems.Add("0");
            }
            lstInfo.Items.Add(item);

            Opcode opcode = new Opcode(registers.Read(Register.CIR));
            item = new ListViewItem("Opcode");
            item.SubItems.Add(opcode.Code.ToString());
            lstInfo.Items.Add(item);

            item = new ListViewItem("Flags");
            item.SubItems.Add(opcode.Flags.ToString("X"));
            lstInfo.Items.Add(item);

            item = new ListViewItem("Operand A");
            if (opcode.Flags.HasFlag(OpcodeFlags.Register1))
                item.SubItems.Add(((Register)opcode.OperandA.Value).ToString());
            else
                item.SubItems.Add(opcode.OperandA.Value.ToString("X"));
            lstInfo.Items.Add(item);

            item = new ListViewItem("Operand B");
            if (opcode.Flags.HasFlag(OpcodeFlags.Register2))
                item.SubItems.Add(((Register)opcode.OperandB.Value).ToString());
            else
                item.SubItems.Add(opcode.OperandB.Value.ToString("X"));
            lstInfo.Items.Add(item);
        }

        ulong Lookup(Register i)
        {
            byte[] binary = new byte[8];
            byte[] bvalue = registers.Read((Register)i);
            bvalue.CopyTo(binary, 0);
            ulong value = BitConverter.ToUInt64(binary, 0);

            return value;
        }

        ulong Lookup(uint address, uint length)
        {
            byte[] binary = new byte[8];
            byte[] bvalue = vm.memory.Read(address, length);
            bvalue.CopyTo(binary, 0);
            ulong value = BitConverter.ToUInt64(binary, 0);

            return value;
        }

        private void frmRegisters_Shown(object sender, EventArgs e)
        {
            vm.Debug = true;
            tmrUpdate.Enabled = !vm.Debug;
            vm.Start();

            if (vm.Debug)
            {
                vm.UpdateDebugger += Vm_UpdateDebugger;
            }
        }

        private void frmRegisters_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void frmRegisters_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10)
            {
                vm.DebugStep();
            }
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            vm.DebugStep();
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            UpdateDebugger();
        }
    }
}
