using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daze {
    /// <summary>
    /// Extend this class to create a different way to draw on screen
    /// </summary>
    public interface IDrawable {
        /// <summary>
        /// Set this to true when the window is loaded. The engine will wait till this is true to start the Game.
        /// </summary>
        bool loaded { get; }
        /// <summary>
        /// This should return true when the window got focus, false otherwise. If you can't detect the window status then make it always true.
        /// </summary>
        bool focus { get; }
        /// <summary>
        /// This should hide the cursor, if you can't hide it then just return always false.
        /// </summary>
        bool hideCursor { get; set; }

        /// <summary>
        /// This method is called from the Engine at the beginning, you should create your window here
        /// </summary>
        void IntialSetup();

        /// <summary>
        /// This method is called from the Engine after the buffer is created
        /// </summary>
        void BufferSetup();

        /// <summary>
        /// This is called from the method after all the setups to show the window
        /// </summary>
        void Start();
        /// <summary>
        /// This method is called from the Engine when it has to close the window
        /// </summary>
        void Stop();

        /// <summary>
        /// Implement this method to draw on the screen
        /// </summary>
        void Draw();

    }
}
