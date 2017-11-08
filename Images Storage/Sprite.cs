using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Daze {
    public class Sprite {
        private BitmapData spriteData;

        private byte[] hiddenPixelArray;

        public byte[] pixelArray { get => hiddenPixelArray; }


        private int hiddenWidth;
        public int width { get => hiddenWidth; }

        private int hiddenHeight;
        public int height { get => hiddenHeight; }

        private int hiddenStride;
        public int stride { get => hiddenStride; }

        private int hiddenBytesPerPixel;
        public int bytesPerPixel { get => hiddenBytesPerPixel; }

        public Sprite(Bitmap bitmap) {
            //inizializzo le variabili della classe
            hiddenWidth = bitmap.Width;
            hiddenHeight = bitmap.Height;
            hiddenBytesPerPixel = (bitmap.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3);

            //blocco il bitmap in memoria (verrà purtoppo sbloccato dal GC, quindi sono costretto a metterlo in un area di memoria fissa)
            spriteData = bitmap.LockBits(new Rectangle(0, 0, hiddenWidth, hiddenHeight), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            hiddenStride = spriteData.Stride;
            IntPtr ptr_startOfSpriteLock = spriteData.Scan0;

            //copio il bitmap in un array di byte
            hiddenPixelArray = new byte[4*hiddenWidth*hiddenHeight];
            Marshal.Copy(ptr_startOfSpriteLock, hiddenPixelArray, 0, hiddenPixelArray.Length);

            //rendo l'array di byte fisso in memoria
            GCHandle handle = GCHandle.Alloc(hiddenPixelArray, GCHandleType.Pinned);

            //sblocco il bitmap e forzo il rilascio
            bitmap.UnlockBits(spriteData);
            bitmap.Dispose();
        }

    }
}
