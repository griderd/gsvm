using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GSVM.Components;
using GSVM.Components.Controllers;

namespace GSVM.Devices.DisplayAdapters
{
    public abstract class DisplayAdapter : GraphicsCard, IDisposable
    {
        protected Bitmap raster;
        protected Size pixelResolution;
        protected bool debugMode;

        public Size PixelResolution { get { return pixelResolution; } }

        public Bitmap Raster { get { return raster; } }

        public DisplayAdapter(Memory memory, int width, int height)
            : base(memory)
        {
            this.pixelResolution = new Size(width, height);
            raster = new Bitmap(pixelResolution.Width, pixelResolution.Height);
        }

        /// <summary>
        /// Generates the next frame of video. This should be done by the monitor.
        /// </summary>
        /// <returns></returns>
        public abstract void Rasterize();

        public virtual void Dispose()
        {
            raster.Dispose();
        }
    }
}
