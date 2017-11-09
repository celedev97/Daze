using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Daze {
    public class Sprite {
        private BitmapData spriteData;

        private byte[] _PixelArray;

        public byte[] pixelArray { get => _PixelArray; }


        private int _Width;
        public int width { get => _Width; }

        private int _Height;
        public int height { get => _Height; }

        private int _Stride;
        public int stride { get => _Stride; }

        private int _BytesPerPixel;
        public int bytesPerPixel { get => _BytesPerPixel; }

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
