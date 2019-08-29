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
            //byteViewer.SetBytes(dump);
            hexBox.ByteProvider = new DynamicByteProvider(dump);
            hexBox.Select(select, selLength);
            

            //for (int i = 0; i < dump.Length; i++)
            //{

            //    hex.AppendFormat("{0:X2} ", dump[i]);
            //}

            //for (int i = 0; i < memory.Length; i++)
            //{
            //    if (i % 16 == 0)
            //        hex.Append(i.ToString("X3"));
            //    hex.Append("  ");

            //    if (registers.GetMAR() == i)
            //    {
            //        select = hex.Length;
            //        selLength = registers.GetMLR() * 3;
            //    }

            //    if (chkHexDigits.Checked)
            //    {

            //        hex.Append(((int)memory.Data[i]).ToString("X2"));
            //        selLength = 2;
            //    }
            //    else
            //    {
            //        hex.Append(' ');
            //        hex.Append(memory.Data[i]);
            //    }

            //    if ((i + 1) % 16 == 0)
            //        hex.AppendLine();
            //}
            //txtHexView.Text = hex.ToString();
            //txtHexView.Select(select, selLength);
        }

        private void btnDump_Click(object sender, EventArgs e)
        {
            //File.WriteAllBytes("ram.bin", memory.Data);
        }

        private void tmrRefresh_Tick(object sender, EventArgs e)
        {
            //RefreshText();
        }
    }
}
