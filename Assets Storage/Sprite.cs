using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Daze {
    /// <summary>
    /// A Sprite in daze is a array of bytes representing a Bitmap.
    /// </summary>
    public class Sprite {
        #region Variables and properties
        internal float scale;

        private byte[] _originalPixelArray;
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
        /// <param name="scale">The scale of the Bitmap</param>
        internal Sprite(Bitmap bitmap,string baseName, float scale) {
            //scaling the image
            bitmap = Engine.Utility.scaleImage(bitmap, scale);

            //initializing class variables
            this.scale = scale;
            _width = bitmap.Width;
            _height = bitmap.Height;
            _bytesPerPixel = (bitmap.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3);

            //locking bitmap in memory (it could be unlocked by GC later, so i'll put it in a byte array)
            BitmapData spriteData = bitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            _stride = spriteData.Stride;
            IntPtr ptr_startOfSpriteLock = spriteData.Scan0;

            //copying the bitmap in a sprite
            //(i used Marshaling to avoid using unsafe code, the performance difference is not terrible,
            //and anyway this method is for preloading, not for being used in the Update)
            _originalPixelArray = new byte[4*_width*_height];
            Marshal.Copy(ptr_startOfSpriteLock, _originalPixelArray, 0, _originalPixelArray.Length);
            Buffer.BlockCopy(_originalPixelArray, 0, _pixelArray, 0, _originalPixelArray.Length);

            //i pin the byte of array in the memory, denying GC the possibility to move it, this should increase performance
            GCHandle handle = GCHandle.Alloc(_originalPixelArray, GCHandleType.Pinned);

            //i unlock the bitmap and force the release of the resource/file
            bitmap.UnlockBits(spriteData);
            bitmap.Dispose();
        }
    }
}
