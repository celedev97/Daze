using System.Drawing;
namespace Daze {
    public class Camera : GameObject {
        internal Camera() : base(0, 0, null) {
            
        }
        public override Collider collider { get => null; }

        public void setBackGround(Bitmap background) {
            Camera.background = new Sprite(Engine.Utility.scaleImage(background, Engine.drawBufferWidth, Engine.drawBufferHeight));
        }

        internal static Sprite background;
    }
}
