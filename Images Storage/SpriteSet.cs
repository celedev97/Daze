using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;

namespace Daze {
    public class SpriteSet {
        //sprites
        private Sprite[] sprites;

        #region Variables for drawing
        private int hiddenMinX;
        private int hiddenMinY;

        public Size size;
        public int minX { get => hiddenMinX; }
        public int minY { get => hiddenMinY; }

        public Sprite sprite { get => sprites[index]; }
        #endregion

        #region Variables for animations
        private GameObject gameObject;
        private Timer timer;
        private int timerID;

        public int index;

        private bool hiddenRepeat;
        public bool repeat {
            get => hiddenRepeat;
            set {
                if(value != hiddenRepeat) {
                    if(value) {
                        timer = gameObject.createTimer(timerID, hiddenChangeMS, next);
                    } else {
                        timer = null;
                        gameObject.removeTimer(timerID);
                    }
                }
                hiddenRepeat = value;
            }
        }
        #endregion


        private int hiddenChangeMS;
        public int changeMS {
            get => hiddenChangeMS;
            set {
                hiddenChangeMS = value;
                if(hiddenRepeat) { timer.msPerTick = hiddenChangeMS; }
            }
        }

        public SpriteSet(GameObject gameObject, params Sprite[] sprites) {
            int changeMS = 1000/sprites.Length;
            Initialize(gameObject, changeMS, true, sprites);
        }

        public SpriteSet(GameObject gameObject, int msToChangeSprite, bool repeat, params Sprite[] sprites) {
            Initialize(gameObject, msToChangeSprite, repeat, sprites);
        }

        private void Initialize(GameObject gameObject, int msToChangeSprite, bool repeat, params Sprite[] sprites) {
            //inizializzazione parametri GameObject e timer
            this.gameObject = gameObject;
            this.timerID = gameObject.lastTimerIndex;
            gameObject.lastTimerIndex--;
            hiddenChangeMS = msToChangeSprite;
            this.repeat = repeat;//così facendo avvio anche il timer se serve

            //inizializzazione parametri di cambio immagine
            index = 0;

            //inizializzazione sprite e dimensioni
            this.sprites = sprites;

            //trovo le coordinate minime e massime dello sprite, sono necessarie per evitare di disegnare parti superflue (così da alleviare i calcoli)
            int[] spriteBounds = null;
            foreach(Sprite sprite in sprites) {
                if(spriteBounds != null) {
                    int[] newBounds = getBounds(sprite);
                    //xMin 
                    spriteBounds[0] = spriteBounds[0] > newBounds[0] ? newBounds[0] : spriteBounds[0];
                    //xMax
                    spriteBounds[1] = spriteBounds[1] < newBounds[1] ? newBounds[1] : spriteBounds[1];
                    //yMin
                    spriteBounds[2] = spriteBounds[2] > newBounds[2] ? newBounds[2] : spriteBounds[2];
                    //yMax
                    spriteBounds[3] = spriteBounds[3] < newBounds[3] ? newBounds[3] : spriteBounds[3];
                } else {
                    spriteBounds = getBounds(sprite);
                }
            }

            //in base alle coordinate trovate calcolo i dati necessari al metodo Draw per fare un disegno parziale
            hiddenMinX = spriteBounds[0];
            hiddenMinY = spriteBounds[2];

            size.width = (spriteBounds[1] - spriteBounds[0]) + 1;
            size.height = (spriteBounds[3] - spriteBounds[2]) + 1;
        }

        public void next() {
            index++;
            if(index == sprites.Length) index = 0;
        }

        private int[] getBounds(Sprite sprite) {
            int lastX = sprite.width -1;
            int lastY = sprite.height -1;

            int xMin = lastX;
            int xMax = 0;
            //ricerca della prima e dell'ultima colonna contenenti pixel con alpha>0
            for(int y = 0; y < sprite.height; y++) {
                int firstColouredPixel = lastX;
                int lastColouredPixel = 0;
                //faccio un ciclo in avanti e cerco il primo pixel colorato
                for(int x = 0; x < sprite.width; x++) {
                    int alphaByteIndex = y*sprite.stride + x*sprite.bytesPerPixel + 3;
                    //se il pixel è colorato
                    if(sprite.pixelArray[alphaByteIndex] != 0) {
                        //ho trovato il primo pixel colorato della linea, esco
                        firstColouredPixel = x;
                        break;
                    }
                }
                //faccio un ciclo all'indietro e cerco il primo pixel colorato
                for(int x = lastX; x > -1; x--) {
                    int alphaByteIndex = y*sprite.stride + x*sprite.bytesPerPixel + 3;
                    //se il pixel è colorato
                    if(sprite.pixelArray[alphaByteIndex] != 0) {
                        //ho trovato il primo pixel colorato della linea, esco
                        lastColouredPixel = x;
                        break;
                    }
                }

                if(firstColouredPixel < xMin) xMin = firstColouredPixel;
                if(lastColouredPixel > xMax) xMax = lastColouredPixel;
            }

            int yMin = lastY;
            int yMax = 0;
            //ricerca della prima e dell'ultima colonna contenenti pixel con alpha>0
            for(int x = 0; x < sprite.height; x++) {
                int firstColouredPixel = lastY;
                int lastColouredPixel = 0;
                //faccio un ciclo in avanti e cerco il primo pixel colorato
                for(int y = 0; y < sprite.height; y++) {
                    int alphaByteIndex = y*sprite.stride + x*sprite.bytesPerPixel + 3;
                    //se il pixel è colorato
                    if(sprite.pixelArray[alphaByteIndex] != 0) {
                        //ho trovato il primo pixel colorato della linea, esco
                        firstColouredPixel = y;
                        break;
                    }
                }
                //faccio un ciclo all'indietro e cerco il primo pixel colorato
                for(int y = lastY; y > -1; y--) {
                    int alphaByteIndex = y*sprite.stride + x*sprite.bytesPerPixel + 3;
                    //se il pixel è colorato
                    if(sprite.pixelArray[alphaByteIndex] != 0) {
                        //ho trovato il primo pixel colorato della linea, esco
                        lastColouredPixel = y;
                        break;
                    }
                }

                if(firstColouredPixel < yMin) yMin = firstColouredPixel;
                if(lastColouredPixel > yMax) yMax = lastColouredPixel;
            }

            return new int[] { xMin, xMax, yMin, yMax };
        }

    }
}
