using System;
using Daze.Geometry;

namespace Daze {
    public class SpriteSet {
        #region Variables for drawing
        private Sprite[] sprites;

        private int _minX;
        private int _minY;

        /// <summary>
        /// The size of an image of this spriteSet
        /// </summary>
        public Size size;
        /// <summary>
        /// The starting X of the first coloured pixel, this is used to draw just the coloured part of a sprite and not an alpha part (if present)
        /// </summary>
        public int minX { get => _minX; }
        /// <summary>
        /// The starting Y of the first coloured pixel, this is used to draw just the coloured part of a sprite and not an alpha part (if present)
        /// </summary>
        public int minY { get => _minY; }

        /// <summary>
        /// The sprite currently used from this SpriteSet
        /// </summary>
        public Sprite sprite { get => sprites[_index]; }

        /// <summary>
        /// This reset the SpriteSet, making it go back to the first Sprite and restart the Timer for changing images
        /// </summary>
        public void reset() {
            _index = 0;
            if(repeat) {
                gameObject.getTimer(timerID)?.restart();
            }
        }
        #endregion

        #region Variables for animations
        private GameObject gameObject;

        private Timer timer;
        private  Action endAnimationAction;
        private int _timerID;

        /// <summary>
        /// The ID of the timer used by this SpriteSet, SpriteSets always use negative timer IDs.
        /// </summary>
        public int timerID { get => _timerID; }

        private int _index;
        public int index { get => _index; }
        
        private bool _repeat;
        /// <summary>
        /// Set to true to make this SpriteSet cycle Sprites automatically
        /// </summary>
        public bool repeat {
            get => _repeat;
            set {
                if(value != _repeat) {
                    if(value) {
                        if(gameObject.getTimer(_timerID) == null) {
                            timer = gameObject.createTimer(_timerID, _ChangeMS, next);
                        }
                        timer.restartFlag = true;
                    } else {
                        timer = null;
                        gameObject.removeTimer(_timerID);
                    }
                }
                _repeat = value;
            }
        }

        private int _ChangeMS;
        /// <summary>
        /// The number of milliseconds that the SpriteSet will wait before going to the next Sprite
        /// </summary>
        public int changeMS {
            get => _ChangeMS;
            set {
                _ChangeMS = value;
                if(_repeat) { timer.msPerTick = _ChangeMS; }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This create a SpriteSet, a SpriteSet is a list of sprites, it can be used to create an animation
        /// </summary>
        /// <param name="gameObject">The GameObject that this SpriteSet will be attached to</param>
        /// <param name="sprites">The Sprites that this SpriteSet will have</param>
        public SpriteSet(GameObject gameObject, params Sprite[] sprites) : this(gameObject, null, sprites) { }

        /// <summary>
        /// This create a SpriteSet, a SpriteSet is a list of sprites, it can be used to create an animation
        /// </summary>
        /// <param name="gameObject">The GameObject that this SpriteSet will be attached to</param>
        /// <param name="endAnimationAction">This method will be fired when the SpriteSet finished the Sprite cycle and it's going back to the first Sprite</param>
        /// <param name="sprites">The Sprites that this SpriteSet will have</param>
        public SpriteSet(GameObject gameObject, Action endAnimationAction, params Sprite[] sprites) : this(gameObject, true, endAnimationAction, sprites) { }

        /// <summary>
        /// This create a SpriteSet, a SpriteSet is a list of sprites, it can be used to create an animation
        /// </summary>
        /// <param name="gameObject">The GameObject that this SpriteSet will be attached to</param>
        /// <param name="repeat">If this flat is set to true the Sprites will change automatically, if you don't set the number of milliseconds for changing the Sprites then the SpriteSet cycle will last 1 second</param>
        /// <param name="sprites">The Sprites that this SpriteSet will have</param>
        public SpriteSet(GameObject gameObject, bool repeat, params Sprite[] sprites) : this(gameObject, repeat, null, sprites) { }

        /// <summary>
        /// This create a SpriteSet, a SpriteSet is a list of sprites, it can be used to create an animation
        /// </summary>
        /// <param name="gameObject">The GameObject that this SpriteSet will be attached to</param>
        /// <param name="repeat">If this flat is set to true the Sprites will change automatically, if you don't set the number of milliseconds for changing the Sprites then the SpriteSet cycle will last 1 second</param>
        /// <param name="endAnimationAction">This method will be fired when the SpriteSet finished the Sprite cycle and it's going back to the first Sprite</param>
        /// <param name="sprites">The Sprites that this SpriteSet will have</param>
        public SpriteSet(GameObject gameObject, bool repeat, Action endAnimationAction, params Sprite[] sprites) : this(gameObject, 1000 / sprites.Length, repeat, endAnimationAction, sprites) { }//V

        /// <summary>
        /// This create a SpriteSet, a SpriteSet is a list of sprites, it can be used to create an animation
        /// </summary>
        /// <param name="gameObject">The GameObject that this SpriteSet will be attached to</param>
        /// <param name="msToChangeSprite">The number of milliseconds that the SpriteSet will wait before changing Sprite</param>
        /// <param name="repeat">If this flat is set to true the Sprites will change automatically, if you don't set the number of milliseconds for changing the Sprites then the SpriteSet cycle will last 1 second</param>
        /// <param name="endAnimationAction">This method will be fired when the SpriteSet finished the Sprite cycle and it's going back to the first Sprite</param>
        /// <param name="sprites">The Sprites that this SpriteSet will have</param>
        public SpriteSet(GameObject gameObject, int msToChangeSprite, bool repeat, Action endAnimationAction, params Sprite[] sprites) {
            if(sprites.Length < 2) repeat = false;
            this.endAnimationAction = endAnimationAction;

            //inizializzazione parametri GameObject e timer
            this.gameObject = gameObject;
            this._timerID = gameObject.lastTimerIndex;
            gameObject.lastTimerIndex--;
            _ChangeMS = msToChangeSprite;
            this.repeat = repeat;//così facendo avvio anche il timer se serve

            //inizializzazione parametri di cambio immagine
            _index = 0;

            //inizializzazione sprite e dimensioni
            this.sprites = sprites;

            //trovo le coordinate minime e massime dello sprite, sono necessarie per evitare di disegnare parti superflue (così da alleviare i calcoli)
            int[] spriteBounds = null;
            foreach(Sprite sprite in sprites) {
                if(sprite == null) continue;
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

            //this should only happen if all the sprites are null
            if(spriteBounds == null) throw new ArgumentException("You can't create a SpriteSet with no Sprites, if you want a gameObject to not have a Sprite just set its spriteSet property to null.");

            //in base alle coordinate trovate calcolo i dati necessari al metodo Draw per fare un disegno parziale
            _minX = spriteBounds[0];
            _minY = spriteBounds[2];

            size.width = (spriteBounds[1] - spriteBounds[0]) + 1;
            size.height = (spriteBounds[3] - spriteBounds[2]) + 1;
        }

        #endregion

        /// <summary>
        /// This method forcefully change the SpriteSet's Sprite without waiting till the right time to change it
        /// This can be helpful in case you want to use the Sprite manually without using the default timer.
        /// </summary>
        public void next() {
            _index++;
            if(_index == sprites.Length) {
                _index = 0;
                endAnimationAction?.Invoke();
            }
            if(_repeat) timer.restart();
        }

        /// <summary>
        /// This method update the rotation of this SpriteSet
        /// </summary>
        public void rotate() {
            throw new NotImplementedException();
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
            for(int x = 0; x < sprite.width; x++) {
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
