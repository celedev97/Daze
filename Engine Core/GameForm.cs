using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Daze {
    internal partial class GameForm:Form {
        internal bool loaded = false;

        internal bool focus = false;
        private Graphics graph;

        internal GameForm() {
            InitializeComponent();
        }

        private void GameForm_FormClosed(object sender, FormClosedEventArgs e) {
            Engine.stopGameCycle();
            Application.ExitThread();
            Environment.Exit(0);
        }

        private void GameForm_Shown(object sender, EventArgs e) {
            loaded = true;
            graph = CreateGraphics();
            graph.CompositingMode = CompositingMode.SourceCopy;
            graph.InterpolationMode = InterpolationMode.NearestNeighbor;

        }

        private void Lost_Focus(object sender, EventArgs e) {
            focus = false;
            Engine.lostFocus?.Invoke();
        }

        private void Got_Focus(object sender, EventArgs e) {
            focus = true;
            Engine.gotFocus?.Invoke();
        }


        public Bitmap buffer;

        internal void updateImage() {
            if(focus) {
                buffer.Dispose();
                buffer = new Bitmap(Engine.drawBufferWidth, Engine.drawBufferHeight, PixelFormat.Format24bppRgb);
                BitmapData bmpData = buffer.LockBits(new Rectangle(0, 0, buffer.Width, buffer.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                for(int y = 0; y < Engine.drawBufferHeight; y++) {
                    IntPtr startOfLine = bmpData.Scan0 + y * bmpData.Stride;
                    Marshal.Copy(Engine.drawBuffer, y * Engine.drawBufferStride, startOfLine, Engine.drawBufferStride);
                }

                buffer.UnlockBits(bmpData);

                graph.DrawImage(buffer, 0, 0);
            }
        }
    }
}
