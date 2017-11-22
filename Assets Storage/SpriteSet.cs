using System;
using Daze.Geometry;

namespace Daze {
    /// <summary>
    /// A spriteSet is a list of sprites with a timer, it can be used to create an animation
    /// </summary>
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
        /// The number of the sprites in this spriteSet
        /// </summary>
        public int spriteCount { get => sprites.Length; }

        /// <summary>
        /// This reset the SpriteSet, making it go back to the first Sprite and restart the Timer for changing images
        /// </summary>
        public void reset() {
            _index = 0;
            if(repeat) {
                gameObject.getTimer(timerID)?.Restart();
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
        /// <summary>
        /// The index of the spriteSet in the sprites array
        /// </summary>
        public int index { get => _index;
            set {
                if(value<0 || value >= sprites.Length) {
                    throw new IndexOutOfRangeException();
                }
                _index = value;
            }
        }
        
        private bool _repeat;
        /// <summary>
        /// Set to true to make this SpriteSet cycle Sprites automatically
        /// </summary>
        public bool repeat {
            get => _repeat;
            set {
                if(value != _repeat) {
                    if(value) {
                        if(gameObject.getTimer(_timerID) == null || timer == null) {
                            gameObject.removeTimer(_timerID);
                            timer = gameObject.createTimer(_timerID, _ChangeMS, callNext);
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
            this._timerID = gameObject.lastSpriteTimerIndex;
            gameObject.lastSpriteTimerIndex--;
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
        
        private void callNext() { Next(); }
        /// <summary>
        /// This method forcefully change the SpriteSet's Sprite without waiting till the right time to change it
        /// This can be helpful in case you want to use the Sprite manually without using the default timer.
        /// <returns>True if there wasn't a next sprite and the animation restarted from the start</returns>
        /// </summary>
        public bool Next() {
            bool didCycle = false;
            _index++;
            if(_index == sprites.Length) {
                _index = 0;
                endAnimationAction?.Invoke();
                didCycle = true;
            }
            if(_repeat) timer.Restart();
            return didCycle;
        }

        /// <summary>
        /// This method forcefully change the SpriteSet's Sprite to the previous one without waiting till the right time to change it
        /// This can be helpful in case you want to use the Sprite manually without using the default timer.
        /// <returns>True if there wasn't a next sprite and the animation restarted from the start</returns>
        /// </summary>
        public bool Prev() {
            bool didCycle = false;
            _index--;
            if(_index == -1) {
                _index = sprites.Length -1;
                didCycle = true;
            }
            if(_repeat) timer.Restart();
            return didCycle;
        }

        /// <summary>
        /// This method update the rotation of this SpriteSet
        /// </summary>
        public void Rotate() {
            for(int i = 0; i< sprites.Length; i++) {
                //i change the sprite with 
                sprites[i] = Engine.loadSprite(sprite.baseName, sprite.scale, gameObject.rotation);
            }
            throw new NotImplementedException();
        }

        private int[] getBounds(Sprite sprite) {
            int lastX = sprite.width -1;
            int lastY = sprite.height -1;

            int xMin = lastX;
            int xMax = 0;
            //searching the first and the last X with a pixel that have alpha != 0
            for(int y = 0; y < sprite.height; y++) {
                int firstColouredPixel = lastX;
                int lastColouredPixel = 0;
                //i cycle for all the x in this row
                for(int x = 0; x < sprite.width; x++) {
                    int alphaByteIndex = y*sprite.stride + x*sprite.bytesPerPixel + 3;
                    //if the pixel is coloured
                    if(sprite.pixelArray[alphaByteIndex] != 0) {
                        //i've found the first coloured pixel of this row, i stop the cycle
                        firstColouredPixel = x;
                        break;
                    }
                }
                //i cycle backward for all the x in this row
                for(int x = lastX; x > -1; x--) {
                    int alphaByteIndex = y*sprite.stride + x*sprite.bytesPerPixel + 3;
                    //if the pixel is coloured
                    if(sprite.pixelArray[alphaByteIndex] != 0) {
                        //i've found the last coloured pixel of this row, i stop the cycle
                        lastColouredPixel = x;
                        break;
                    }
                }

                //i check if the pixel found this row are the minimum and the maximum till now
                if(firstColouredPixel < xMin) xMin = firstColouredPixel;
                if(lastColouredPixel > xMax) xMax = lastColouredPixel;
            }

            int yMin = lastY;
            int yMax = 0;
            //searching the first and the last Y with a pixel that have alpha != 0
            for(int x = 0; x < sprite.width; x++) {
                int firstColouredPixel = lastY;
                int lastColouredPixel = 0;
                //i cycle for all the y in this column
                for(int y = 0; y < sprite.height; y++) {
                    int alphaByteIndex = y*sprite.stride + x*sprite.bytesPerPixel + 3;
                    //if the pixel is coloured
                    if(sprite.pixelArray[alphaByteIndex] != 0) {
                        //i've found the first coloured pixel of this column, i stop the cycle
                        firstColouredPixel = y;
                        break;
                    }
                }
                //i cycle backward for all the y in this column
                for(int y = lastY; y > -1; y--) {
                    int alphaByteIndex = y*sprite.stride + x*sprite.bytesPerPixel + 3;
                    //if the pixel is coloured
                    if(sprite.pixelArray[alphaByteIndex] != 0) {
                        //i've found the last coloured pixel of this column, i stop the cycle
                        lastColouredPixel = y;
                        break;
                    }
                }

                //i check if the pixel found this column are the minimum and the maximum till now
                if(firstColouredPixel < yMin) yMin = firstColouredPixel;
                if(lastColouredPixel > yMax) yMax = lastColouredPixel;
            }

            return new int[] { xMin, xMax, yMin, yMax };
        }

    }
}
