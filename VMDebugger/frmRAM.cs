using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GSVM.Components;
using GSVM.Components.Processors;
using System.ComponentModel.Design;
using Be.Windows.Forms;
using System.IO;

namespace VMDebugger
{
    public partial class frmRAM : Form
    {
        Memory memory;
        frmRegisters registers;
        //ByteViewer byteViewer;
        DynamicByteProvider byteProvider;

        public frmRAM(Memory memory, frmRegisters registers)
        {
            InitializeComponent();
            this.memory = memory;
            this.registers = registers;
            //txtHexView.HideSelection = false;

            //byteViewer = new ByteViewer();
            //byteViewer.Location = new Point(8, 8);
            //byteViewer.Width = Width - 16;
            //byteViewer.Height = Height - 100;
            //Controls.Add(byteViewer);

            byteProvider = new DynamicByteProvider(new byte[0]);
            hexBox.ByteProvider = byteProvider;
        }

        private void frmRAM_Load(object sender, EventArgs e)
        {
            RefreshText();
        }

        public void RefreshText()
        {
            this.Text = "RAM - " + memory.Length.ToString() + " bytes (" + (memory.Length / 1024).ToString() + " KB)";
            int select = registers.GetMAR();
            int selLength = registers.GetMLR();

            StringBuilder hex = new StringBuilder();

            byte[] dump = memory.Read(0, memory.Length);

            byteProvider.DeleteBytes(0, byteProvider.Length);
            byteProvider.InsertBytes(0, dump);
            hexBox.Select(select, selLength);
        }

        private void btnDump_Click(object sender, EventArgs e)
        {
            File.WriteAllBytes("memdump.bin", memory.Read(0, memory.Length));
        }

        private void tmrRefresh_Tick(object sender, EventArgs e)
        {
            //RefreshText();
        }

        private void frmRAM_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
