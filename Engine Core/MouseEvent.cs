using System.Windows.Forms;

namespace Daze {
    internal class MouseEvent {
        internal object sender;
        internal MouseEventArgs e;

        internal MouseEvent(object sender, MouseEventArgs e) {
            this.sender = sender;
            this.e = e;
        }

    }
}