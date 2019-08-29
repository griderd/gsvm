using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Components;
using GSVM.Components.Controllers;

namespace GSVM.Devices.DisplayAdapters
{
    public class MonochromeDisplayAdapter : DisplayAdapter
    {
        private object lockObject = new object();

        string checksum;
        Bitmap charmap;

        int cellWidth;
        int cellHeight;
        int columns;
        int rows;

        public MonochromeDisplayAdapter(int columns = 80, int rows = 25, int cellWidth = 9, int cellHeight = 16)
            : base(new Memory((uint)(columns * rows)), columns * cellWidth, rows * cellHeight)
        {
            charmap = new Bitmap("codepage.bmp");

            this.cellHeight = cellHeight;
            this.cellWidth = cellWidth;
            this.columns = columns;
            this.rows = rows;

            checksum = memory.Checksum;
        }

        public override void Rasterize()
        {
            checksum = memory.Checksum;

            raster.Dispose();
            raster = new Bitmap(cellWidth * columns, cellHeight * rows);
            using (Graphics g = Graphics.FromImage(raster))
            {
                g.Clear(Color.Black);
                for (int row = 0; row < rows; row++)
                {
                    for (int column = 0; column < columns; column++)
                    {
                        byte cnum = memory.Read((uint)(column + (80 * row)));
                        if (cnum == 0)
                            continue;

                        int mapRow = cnum / 32;
                        int mapColumn = cnum % 32;

                        int sourceX = mapColumn * cellWidth;
                        int sourceY = mapRow * cellHeight;
                        int destX = column * cellWidth;
                        int destY = row * cellHeight;

                        //g.DrawImage(charmap, (float)(mapColumn * cellWidth), (float)(mapRow * cellHeight), (float)cellWidth, (float)cellHeight);
                        g.DrawImage(charmap, new Rectangle(destX, destY, cellWidth, cellHeight), new Rectangle(sourceX, sourceY, cellWidth, cellHeight), GraphicsUnit.Pixel);
                    }
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            charmap.Dispose();
        }
    }
}
