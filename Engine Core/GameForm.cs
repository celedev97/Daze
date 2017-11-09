using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Daze {
    internal partial class GameForm:Form {
        internal bool loaded = false;
        private PictureBox gameFrame;
        internal bool focus = false;

        private System.ComponentModel.IContainer components = null;

        internal GameForm() {
            //form
            DoubleBuffered = true;
            Text = "GameForm";
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            WindowState = System.Windows.Forms.FormWindowState.Maximized;

            //gameFrame
            this.gameFrame = new System.Windows.Forms.PictureBox();
            //((System.ComponentModel.ISupportInitialize)(this.gameFrame)).BeginInit();
            gameFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Controls.Add(this.gameFrame);
            //((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();

            //events
            FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GameForm_FormClosed);
            Shown += new System.EventHandler(this.GameForm_Shown);
            GotFocus += new System.EventHandler(this.Got_Focus);
            LostFocus += new System.EventHandler(this.Lost_Focus);
        }

        private void GameForm_FormClosed(object sender, FormClosedEventArgs e) {
            Environment.Exit(0);
        }
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void GameForm_Shown(object sender, EventArgs e) {
            loaded = true;
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

                gameFrame.Image = buffer;
            }
        }
    }
}
