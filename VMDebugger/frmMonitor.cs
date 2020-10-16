using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GSVM.Devices.DisplayAdapters;

namespace VMDebugger
{
    public partial class frmMonitor : Form
    {
        DisplayAdapter adapter;

        public frmMonitor(DisplayAdapter adapter)
        {
            InitializeComponent();

            this.adapter = adapter;
            monitor.Adapter = adapter;
        }

        private void tmrRefresh_Tick(object sender, EventArgs e)
        {
            monitor.RefreshScreen();
        }

        private void frmMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmrRefresh.Enabled = false;
            adapter.Dispose();
            monitor.Dispose();
        }
    }
}
