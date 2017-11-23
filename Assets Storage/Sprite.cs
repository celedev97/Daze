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
        #region Base Sprite Data
        private byte[] originalPixelArray;
        private int originalStride;

        private string _baseName;
        internal string baseName { get => _baseName; }

        private float _scale;
        internal float scale { get => _scale; }

        private int originalWidth;
        private int originalHeight;


        private int _bytesPerPixel;
        internal int bytesPerPixel { get => _bytesPerPixel; }
        #endregion

        #region Actual Sprite Data
        private int _rotation;
        /// <summary>
        /// The current real rotation of the sprite (the sprite rotation goes by step, to see the theoretical rotation of this object see gameObject.rotation)
        /// </summary>
        public int rotation { get => _rotation; }

        private byte[] _pixelArray;
        internal byte[] pixelArray { get => _pixelArray; }

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
        internal int stride { get => _stride; }

        #endregion

        internal Sprite(Bitmap bitmap, string baseName, float scale = 1, float rotation = 0) {
            //scaling the image size
            bitmap = Engine.Utility.scaleImage(bitmap, scale);
            //bitmap.Save("F:\\test.png");
            //initializing unrotated(base) sprite variables
            initBase(baseName, scale, bitmap.Width, bitmap.Height, (bitmap.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3));

            //creating the byte array for the image
            originalPixelArray = new byte[_bytesPerPixel * originalWidth * originalHeight];

            #region Copying the bitmap into the byte array
            //locking bitmap in memory (it could be unlocked by GC later, so i'll put it in a byte array)
            
            BitmapData spriteData = bitmap.LockBits(new Rectangle(0, 0, originalWidth, originalHeight), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            originalStride = spriteData.Stride;
            IntPtr ptr_startOfSpriteLock = spriteData.Scan0;

            //copying the bitmap in a sprite
            //(i used Marshaling to avoid using unsafe code, the performance difference is not terrible,
            //and anyway this method is for preloading, not for being used in the Update)
            Marshal.Copy(ptr_startOfSpriteLock, originalPixelArray, 0, originalPixelArray.Length);

            //i pin the byte of array in the memory, denying GC the possibility to move it, this should increase performance
            GCHandle handle = GCHandle.Alloc(_pixelArray, GCHandleType.Pinned);

            //i unlock the bitmap and force the release of the resource/file
            bitmap.UnlockBits(spriteData);
            bitmap.Dispose();
            #endregion

            #region Setting the actual sprite data in case the rotation is 0
            _pixelArray = originalPixelArray;
            _stride = originalStride;
            _width = originalWidth;
            _height = originalHeight;
            #endregion

            //rotating the image
            rotate(stepRotation(rotation));
        }

        private Sprite() {}

        private bool rotate(int steppedRotation) {
            #region 90 degrees multiple rotations
            _rotation = steppedRotation;
            #region Cases 90, 180, or 270
            if(rotation == 0) {
                _pixelArray = originalPixelArray;
                _stride = originalStride;
                _width = originalWidth;
                _height = originalHeight;
                return true;
            }
            if(_rotation == 90 || rotation == 180 || rotation == 270) {
                _pixelArray = new byte[originalPixelArray.Length];

                //i define the variables before the cycle, so they won't be recreated and destroyed many times
                int y;
                int x;
                int i;
                int pixelStart;
                int originalPixelStart;

                if(rotation == 180) {
                    _width = originalWidth;
                    _height = originalHeight;
                    _stride = _width * _bytesPerPixel;
                    for(y = 0; y < _height; y++) {
                        for(x = 0; x < _width; x++) {
                            pixelStart = y * originalStride + x * _bytesPerPixel;
                            originalPixelStart = (height - (y + 1)) * originalStride + (width - (x + 1)) * _bytesPerPixel;
                            for(i = 0; i < _bytesPerPixel; i++) {
                                _pixelArray[pixelStart++] = originalPixelArray[originalPixelStart++];
                            }
                        }
                    }
                    return true;
                } else {
                    _width = originalHeight;
                    _height = originalWidth;
                    _stride = _width * _bytesPerPixel;
                    if(_rotation == 90) {
                        //oldX = newHeight - y
                        //oldY = newX
                        for(y = 0; y < _height; y++) {
                            for(x = 0; x < _width; x++) {
                                pixelStart = y * _stride + x * _bytesPerPixel;
                                originalPixelStart = x * originalStride + y * _bytesPerPixel;
                                for(i = 0; i < _bytesPerPixel; i++) {
                                    _pixelArray[pixelStart++] = originalPixelArray[originalPixelStart++];
                                }
                            }
                        }
                    } else {
                        //rotation 270
                        //oldX = newY
                        //oldY = newX
                        for(y = 0; y < _height; y++) {
                            for(x = 0; x < _width; x++) {
                                pixelStart = y * _stride + x * _bytesPerPixel;
                                originalPixelStart = (_width - (x + 1)) * originalStride + y * _bytesPerPixel;
                                for(i = 0; i < _bytesPerPixel; i++) {
                                    _pixelArray[pixelStart++] = originalPixelArray[originalPixelStart++];
                                }
                            }
                        }

                        
                    }
                    return true;
                }
            }

            #endregion
            #endregion

            return false;


        }

        internal int stepRotation(float rotation) {
            if(rotation == 0) return 0;
            int dimension = originalWidth > originalHeight ? originalWidth : originalHeight;
            int unitRotation;
            if(dimension <= 100) {
                unitRotation = 30;//360/12
            } else if(dimension <= 300) {
                unitRotation = 15;//360/24
            } else {
                unitRotation = 10;//360/36
            }
            //finding the stepped rotation from -infinite,+infinite
            int stepRotation = ((int)(rotation / unitRotation)) * unitRotation;
            //converting it to -360,+360
            stepRotation -= (stepRotation / 360) * 360;
            //converting it to 0,360
            if(stepRotation < 0) stepRotation = 360 + stepRotation;
            return stepRotation;
        }

        internal Sprite cloneBase(int steppedRotation = 0) {
            Sprite output = new Sprite();
            output.originalPixelArray = originalPixelArray;
            output.originalStride = originalStride;
            output.initBase(_baseName, _scale, originalWidth, originalHeight, _bytesPerPixel);
            if(!output.rotate(steppedRotation)) {
                //TODO: soluzione temporanea, togli quando rotate sarà completo.
                output._pixelArray = new byte[_pixelArray.Length];
                Buffer.BlockCopy(_pixelArray, 0, output._pixelArray, 0, _pixelArray.Length);
                output._height = _height;
                output._width = _width;
            }
            return output;
        }

        private void initBase(string baseName, float scale, int width, int height, int bpp) {
            _baseName = baseName;
            _scale = scale;
            originalWidth = width;
            originalHeight = height;
            _bytesPerPixel = bpp;
        }
    }
}
