using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using GSVM.Devices.DisplayAdapters;
using System.Windows.Forms;


namespace GSVM.Peripherals.Monitors
{
    public class MonitorImageControl : PictureBox
    {
        DisplayAdapter adapter;
        public DisplayAdapter Adapter { get { return adapter; } set { adapter = value; } }

        public MonitorImageControl()
        {
            adapter = null;
            Paint += MonitorControl_Paint;
        }

        private void MonitorControl_Paint(object sender, PaintEventArgs e)
        {
            RefreshScreen();
        }

        public void RefreshScreen()
        {
            if (!DesignMode)
            {
                if (adapter != null)
                {
                    adapter.Rasterize();

                    if (Image != null)
                        Image.Dispose();
                    Image = new Bitmap(adapter.Raster);

                    GC.Collect();
                }
            }
        }
    }
}
