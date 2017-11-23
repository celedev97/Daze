using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Daze {
    class DrawOnWinform:IDrawable {
        private static GameForm _window;

        /// <summary>
        /// This is the window that is showing the game, theorically you shouldn't need it, but if you want to do something particular... go for it ;)
        /// </summary>
        public static GameForm window { get => _window; }

        private static bool _hideCursor = false;
        /// <summary>
        /// Setting this to true or false will show of hide the cursor
        /// </summary>
        public bool hideCursor {
            get => _hideCursor;
            set {
                if(value == _hideCursor) {
                    return;
                }
                if(value) {
                    window.cursorHide();
                } else {
                    window.cursorShow();
                }
                _hideCursor = value;
            }
        }

        public bool loaded => _window.loaded;

        public bool focus => _window.focus;

        public void IntialSetup() {
            _window = new GameForm();
        }

        public void BufferSetup() {
            _window.buffer = new Bitmap(Engine.bufferWidth, Engine.bufferHeight);
            using(Graphics g = Graphics.FromImage(_window.buffer)) {
                g.DrawRectangle(new Pen(Color.Blue), 0, 0, Engine.bufferWidth, Engine.bufferHeight);
            }
            _window.setOffsets();
        }

        public void Start() {
            _window.Show();
            _window.Activate();
        }

        public void Stop() {
            _window.Close();
        }

        public void Draw() {
            _window.updateImage();
            Application.DoEvents();
        }

    }
}
