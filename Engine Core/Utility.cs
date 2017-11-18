using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq.Expressions;

namespace Daze {
    public static partial class Engine {
        /// <summary>
        /// This is a static class that contains some method that can be used to do various things
        /// </summary>
        public static class Utility {
            internal static Random random;

            /// <summary>
            /// This method generate a random int number
            /// </summary>
            /// <param name="max">The maximum number generated</param>
            /// <returns>A number from 0 to max (both included in the range)</returns>
            public static int RandomInt(int max) {
                return (int)(random.NextDouble() * (max + 1));
            }

            /// <summary>
            /// (Wait... you really need me to explain this... ?)
            /// A simple function that count the occurrence of a string in another string
            /// </summary>
            /// <param name="haystack">The large string</param>
            /// <param name="needle">The small string that must be searched in the larger one</param>
            /// <returns></returns>
            public static int countStringOccurrences(string haystack, string needle) {
                int count = 0;
                int nextIndex = 0;
                while((nextIndex = haystack.IndexOf(needle, nextIndex)) > -1) {
                    count++;
                }
                return count;
            }

            internal static byte alphaBlend(byte colorComponentOld, byte colorComponentNew, byte alpha) {
                switch(alpha) {
                    case 0:
                        return colorComponentOld;
                    case 255:
                        return colorComponentNew;
                }
                return (byte)((colorComponentNew * alpha / 255.0) + (colorComponentOld * (1 - (alpha / 255.0))));
            }

            internal static string getResourceName<T>(Expression<Func<T>> property) {
                return (property.Body as MemberExpression).Member.Name;
            }

            internal static Bitmap scaleImage(Bitmap original, float scale) {
                return scaleImage(original, (int)Math.Round(original.Width * scale), (int)Math.Round(original.Height * scale));
            }

            internal static Bitmap scaleImage(Bitmap original, int width, int height) {
                Bitmap newImage = new Bitmap(width, height) ;
                using(Graphics g = Graphics.FromImage(newImage)) {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(original, 0, 0, newImage.Width, newImage.Height);
                }
                return newImage;
            }
        }
    }

}
