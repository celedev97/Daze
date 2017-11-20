using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Daze {
    /// <summary>
    /// A Sprite in daze is a array of bytes representing a Bitmap.
    /// </summary>
    public class Sprite {
        private BitmapData spriteData;
        #region Variables and properties
        private byte[] _pixelArray;
        /// <summary>
        /// The array of pixels that represent this sprite
        /// </summary>
        public byte[] pixelArray { get => _pixelArray; }
        
        private int _width;
        /// <summary>
        /// The width of the sprite
        /// </summary>
        public int width { get => _width; }

        private int _height;
        /// <summary>
        /// The height of the sprite
        /// </summary>
        public int height { get => _height; }

        private int _stride;
        /// <summary>
        /// The length of a line of pixel of this sprite measured in bytes
        /// </summary>
        public int stride { get => _stride; }
        
        private int _bytesPerPixel;
        /// <summary>
        /// How many bytes a pixel takes
        /// </summary>
        public int bytesPerPixel { get => _bytesPerPixel; }
        #endregion

        /// <summary>
        /// This create a sprite from a Bitmap.
        /// In Daze a sprite is nothing more than an array of bytes representing the bitmap so it can be accessed more fastly.
        /// </summary>
        /// <param name="bitmap">The original Bitmap</param>
        internal Sprite(Bitmap bitmap) {
            //inizializzo le variabili della classe
            _width = bitmap.Width;
            _height = bitmap.Height;
            _bytesPerPixel = (bitmap.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3);

            //blocco il bitmap in memoria (verrà purtoppo sbloccato dal GC, quindi sono costretto a metterlo in un area di memoria fissa)
            spriteData = bitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            _stride = spriteData.Stride;
            IntPtr ptr_startOfSpriteLock = spriteData.Scan0;

            //copio il bitmap in un array di byte
            _pixelArray = new byte[4*_width*_height];
            Marshal.Copy(ptr_startOfSpriteLock, _pixelArray, 0, _pixelArray.Length);

            //rendo l'array di byte fisso in memoria
            GCHandle handle = GCHandle.Alloc(_pixelArray, GCHandleType.Pinned);

            //sblocco il bitmap e forzo il rilascio
            bitmap.UnlockBits(spriteData);
            bitmap.Dispose();
        }

    }
}
