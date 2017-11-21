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
        bool loaded { get; }
        bool focus { get; }
        bool hideCursor { get; set; }

        void IntialSetup();

        void BufferSetup();

        void Start();
        void Stop();

        /// <summary>
        /// Implement this method to draw on the screen
        /// </summary>
        void Draw();

    }
}
