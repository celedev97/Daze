using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;


namespace Daze {
    public class Font {

        private Dictionary<char, Sprite> fontSprites;

        private System.Drawing.Font realFont;
        private FontFamily fontFamily;

        private Color _color;
        public Color color {
            get => _color;
            set {
                _color = value;
                size = size;//this forces the recreation of the sprites
            }
        }
        private float _size;

        public float size{
            get => _size;
            set {
                if(size != value) realFont = this;
                _size = value;

                fontSprites = new Dictionary<char, Sprite>();

                //caratteri a-~, 127 è DEL
                createSprites(32, 127);

                //caratteri Ç-■, 255 è nbsp
                createSprites(128, 255);

            }
        }

        private void createSprites(int minChar, int maxCharPlus1) {
            for(int i = minChar; i < maxCharPlus1; i++) {
                string spriteName = "FNCHAR_" + (char)i + "_dazeS"+_size+"_dazeC"+_color;
                if(Engine.sprites.ContainsKey(spriteName)) {
                    return;//se questo sprite esiste esistono anche gli altri, è inutile continuare
                } else {
                    //creating the bitmap
                    Size size = TextRenderer.MeasureText(""+(char)i, realFont);
                    Bitmap characterBMP = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);


                    //setting graphics
                    Graphics g = Graphics.FromImage(characterBMP);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                    //drawing the text
                    TextRenderer.DrawText(g, "" + (char)i, realFont, new Point(0, 0), _color);

                    //creating the Sprite and adding it to the Engine
                    Sprite sprite = new Sprite(characterBMP, spriteName);
                    Engine.sprites.Add(spriteName, sprite);
                    fontSprites.Add((char)i, sprite);
                }
            }
        }

        //costruttore privato, usato solo per le conversioni
        private Font() {}
        public Font(string filePath, float emSize, Color? color = null) {
            PrivateFontCollection fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile(filePath);
            fontFamily = fontCollection.Families[0];

            if(color == null){
                _color = Color.Black;
            }else {
                _color = (Color)color;
            }

            size = emSize;//così inizializzo anche il font e gli sprite
        }

        #region Conversioni tra font C# e Font Daze
        public static implicit operator System.Drawing.Font(Daze.Font dazeFont) {
            return new System.Drawing.Font(dazeFont.fontFamily, dazeFont.size);
        }

        public static implicit operator Daze.Font(System.Drawing.Font systemFont) {
            Daze.Font dazeFont = new Daze.Font();
            dazeFont.fontFamily = systemFont.FontFamily;
            dazeFont.size = systemFont.Size;//così inizializzo anche il realFont e gli sprite
            return dazeFont;
        }
        #endregion


    }
}
