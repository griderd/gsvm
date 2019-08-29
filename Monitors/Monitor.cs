using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GSVM.Devices.DisplayAdapters;

namespace GSVM.Peripherals.Monitors
{
    /// <summary>
    /// Represents a video monitor
    /// </summary>
    public class Monitor : GameWindow
    {
        MonochromeDisplayAdapter adapter;

        public Monitor(int width, int height, string title, MonochromeDisplayAdapter displayAdapter)
            : base(width, height)
        {
            adapter = displayAdapter;
            Title = title;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(Color.CornflowerBlue);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            int frame = GetFrame();
            DrawFrame(frame);

            SwapBuffers();
            //System.Threading.Thread.Yield();
        }

        int GetFrame()
        {
            adapter.Rasterize();
            Bitmap raster = new Bitmap(adapter.Raster);
            //raster.RotateFlip(RotateFlipType.Rotate180FlipX);
            int tex;

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            BitmapData data = raster.LockBits(new Rectangle(0, 0, raster.Width, raster.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, raster.Width, raster.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            raster.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return tex;
        }

        void DrawFrame(int image)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Ortho(0, Width, 0, Height, -1, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Disable(EnableCap.Lighting);

            GL.Enable(EnableCap.Texture2D);

            //GL.Color4(1, 0, 0, 1);

            GL.BindTexture(TextureTarget.Texture2D, image);

            GL.Begin(PrimitiveType.Quads);
            //GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0, 1);
            GL.Vertex3(0, 0, 0);

            GL.TexCoord2(1, 1);
            GL.Vertex3(Width, 0, 0);

            GL.TexCoord2(1, 0);
            GL.Vertex3(Width, Height, 0);

            GL.TexCoord2(0, 0);
            GL.Vertex3(0, Height, 0);

            GL.End();

            GL.Disable(EnableCap.Texture2D);
            GL.PopMatrix();

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

            GL.MatrixMode(MatrixMode.Modelview);
        }
    }
}
