using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Daze {
    public class Sprite {
        private BitmapData spriteData;
        #region Variables and properties
        private byte[] _PixelArray;
        /// <summary>
        /// The array of pixels that represent this sprite
        /// </summary>
        public byte[] pixelArray { get => _PixelArray; }

<<<<<<< HEAD
        private int _Width;
        /// <summary>
        /// The width of the sprite
        /// </summary>
        public int width { get => _Width; }

        private int _Height;
        /// <summary>
        /// The height of the sprite
        /// </summary>
        public int height { get => _Height; }
=======
        private byte[] _PixelArray;

        public byte[] pixelArray { get => _PixelArray; }
>>>>>>> 84a047f1bcbd99d313f202b4c6b43b160f16d8b1

        private int _Stride;
        /// <summary>
        /// The length of a line of pixel of this sprite measured in bytes
        /// </summary>
        public int stride { get => _Stride; }

<<<<<<< HEAD
        private int _BytesPerPixel;
        /// <summary>
        /// How many bytes a pixel takes
        /// </summary>
        public int bytesPerPixel { get => _BytesPerPixel; }
        #endregion
=======
        private int _Width;
        public int width { get => _Width; }

        private int _Height;
        public int height { get => _Height; }

        private int _Stride;
        public int stride { get => _Stride; }

        private int _BytesPerPixel;
        public int bytesPerPixel { get => _BytesPerPixel; }
>>>>>>> 84a047f1bcbd99d313f202b4c6b43b160f16d8b1

        /// <summary>
        /// This create a sprite from a Bitmap.
        /// In Daze a sprite is nothing more than an array of bytes representing the bitmap so it can be accessed more fastly.
        /// </summary>
        /// <param name="bitmap">The original Bitmap</param>
        public Sprite(Bitmap bitmap) {
            //inizializzo le variabili della classe
            _Width = bitmap.Width;
            _Height = bitmap.Height;
            _BytesPerPixel = (bitmap.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3);

            //blocco il bitmap in memoria (verrà purtoppo sbloccato dal GC, quindi sono costretto a metterlo in un area di memoria fissa)
            spriteData = bitmap.LockBits(new Rectangle(0, 0, _Width, _Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            _Stride = spriteData.Stride;
            IntPtr ptr_startOfSpriteLock = spriteData.Scan0;

            //copio il bitmap in un array di byte
            _PixelArray = new byte[4*_Width*_Height];
            Marshal.Copy(ptr_startOfSpriteLock, _PixelArray, 0, _PixelArray.Length);

            //rendo l'array di byte fisso in memoria
            GCHandle handle = GCHandle.Alloc(_PixelArray, GCHandleType.Pinned);

            //sblocco il bitmap e forzo il rilascio
            bitmap.UnlockBits(spriteData);
            bitmap.Dispose();
        }

    }
}
