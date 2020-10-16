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
using GSVM.Components.Controllers;
using GSVM.Components.Clocks;
using GSVM.Components.Processors;
using GSVM.Components.Mem;
using GSVM.Constructs.DataTypes;
using GSVM.Components.Processors.CPU_1;
using GSVM.Devices;
using GSVM.Devices.DisplayAdapters;
using GSVM.Peripherals.IODevices;

using GSVM.Components.Processors.CPU_1.Assembler;

namespace VMDebugger
{
    public partial class frmRegisters : Form
    {
        Memory cmosMemory;
        Memory disk1Memory;
        DiskDrive cmos;
        DiskDrive disk1;
        Southbridge sb;
        CPU1 cpu;
        Northbridge nb;
        ThreadedClock clock;
        VirtualMachine vm;
        MonochromeDisplayAdapter graphics;
        Keyboard keyboard;

        frmMonitor monitor;
        frmRAM ram;

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

            registers.Append<uint32_t>(Register.SVM);

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

            registers.Append<uint16_t>(Register.ABP);
            registers.Append<uint16_t>(Register.AEI);
            registers.Append<uint16_t>(Register.AEL);
            registers.Append<uint16_t>(Register.AEP);

            registers.BuildMemory();

            //byte[] bin = Assemble();
            byte[] bcmos = System.IO.File.ReadAllBytes("cmos.bin");
            byte[] bdisk1 = System.IO.File.ReadAllBytes("disk1.bin");

            // Build the VM
            cmosMemory = new Memory(bcmos);
            cmos = new DiskDrive(cmosMemory);
            disk1Memory = new Memory(bdisk1);
            disk1 = new DiskDrive(disk1Memory);
            sb = new Southbridge(cmos);
            sb.AddDevice(disk1);
            keyboard = new Keyboard();
            sb.AddDevice(keyboard);
            cpu = new CPU1();
            graphics = new MonochromeDisplayAdapter();
            nb = new Northbridge(cpu, sb, new Memory(64*1024), graphics);
            clock = new ThreadedClock(nb);
            nb.Clock = clock;
            vm = new VirtualMachine(cpu, clock, nb, sb);

            byte[] state = vm.GetRegisters();
            registers.LoadState(state);

            //graphics.Write(0, Encoding.ASCII.GetBytes("Hello World!"));

            UpdateList();
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
            Text = string.Format("Registers ({0} KB allocated)", GC.GetTotalMemory(false) / 1024);

            if (ram != null) ram.RefreshText();
            lstRegisters.Items.Clear();
            lstInfo.Items.Clear();

            //nb.WriteDisplay(0, Encoding.ASCII.GetBytes("Hello World!"));

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

            // Stack
            lstStack.Items.Clear();
            IDataType[] values = cpu.Stack;
            for (int i = 0; i < values.Length; i++)
            {
                try
                {
                    lstStack.Items.Add(values[i].ToString());
                }
                catch
                {
                }
            }
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
            try
            {
                byte[] bvalue = nb.Memory.Read(address, length);
                bvalue.CopyTo(binary, 0);
            }
            catch
            {
            }
            ulong value = BitConverter.ToUInt64(binary, 0);

            return value;
        }

        private void frmRegisters_Shown(object sender, EventArgs e)
        {
            monitor = new frmMonitor(graphics);
            monitor.Show();
            ram = new frmRAM(nb.Memory, this);
            ram.Show();
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

        private void btnStart_Click(object sender, EventArgs e)
        {
            // vm.Debug = false;
            vm.Start();
            btnDebug.Enabled = false;
            btnStart.Enabled = false;
        }

        private void lstInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void btnDebug_Click(object sender, EventArgs e)
        {
            vm.Debug = true;
            tmrUpdate.Enabled = !vm.Debug;

            vm.UpdateDebugger += Vm_UpdateDebugger;

            vm.Start();
            btnDebug.Enabled = false;
            btnStart.Enabled = false;
        }

        public int GetMAR()
        {
            return (int)Lookup(Register.MAR);   
        }

        public int GetMLR()
        {
            return (int)Lookup(Register.MLR);
        }

        private void frmRegisters_FormClosing(object sender, FormClosingEventArgs e)
        {
            vm.Stop();
            monitor.Close();
            ram.Close();
            try
            {
                graphics.Dispose();
            }
            catch
            {
            }
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            tmrUpdate.Enabled = true;

            vm.UpdateDebugger -= Vm_UpdateDebugger;
            vm.Debug = false;
        }
    }
}
