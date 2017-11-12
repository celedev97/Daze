using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Daze {
    public class GameForm:Form {
        #region Variables
        internal bool loaded = false;
        private PictureBox gameFrame;
<<<<<<< HEAD
        internal Bitmap buffer;
        internal bool focus = false;
        #endregion
=======
        internal bool focus = false;

        private System.ComponentModel.IContainer components = null;
>>>>>>> 84a047f1bcbd99d313f202b4c6b43b160f16d8b1

        #region Methods
        internal GameForm() {
            //form
            DoubleBuffered = true;
            Text = "GameForm";
<<<<<<< HEAD
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            //gameFrame
            this.gameFrame = new PictureBox();
            gameFrame.Dock = DockStyle.Fill;
            this.Controls.Add(this.gameFrame);

            //events
            FormClosed += new FormClosedEventHandler(this.GameForm_FormClosed);
            Shown += new EventHandler(this.GameForm_Shown);
            GotFocus += new EventHandler(this.Got_Focus);
            LostFocus += new EventHandler(this.Lost_Focus);
=======
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
>>>>>>> 84a047f1bcbd99d313f202b4c6b43b160f16d8b1
        }

        #region Event handlers
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
        #endregion

        #region Drawing methods
        internal void updateImage() {
            if(focus) {
                buffer.Dispose();
                buffer = new Bitmap(Engine._drawBufferWidth, Engine._drawBufferHeight, PixelFormat.Format24bppRgb);
                BitmapData bmpData = buffer.LockBits(new Rectangle(0, 0, buffer.Width, buffer.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                for(int y = 0; y < Engine._drawBufferHeight; y++) {
                    IntPtr startOfLine = bmpData.Scan0 + y * bmpData.Stride;
                    Marshal.Copy(Engine.drawBuffer, y * Engine.drawBufferStride, startOfLine, Engine.drawBufferStride);
                }

                buffer.UnlockBits(bmpData);

                gameFrame.Image = buffer;
            }
        }
        #endregion
        #endregion

    }
}
