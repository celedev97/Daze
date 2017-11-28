using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daze {
    public static partial class Engine {
        #region Enums
        /// <summary>
        /// The rendering resolutions
        /// </summary>
        public enum RenderingSize {
            /// <summary>
            /// Resolution 160X120
            /// </summary>
            SIZE_160X120,
            /// <summary>
            /// Resolution 160X200
            /// </summary>
            SIZE_160X200,
            /// <summary>
            /// Resolution 240X160
            /// </summary>
            SIZE_240X160,
            /// <summary>
            /// Resolution 320X240
            /// </summary>
            SIZE_320X240,
            /// <summary>
            /// Resolution 480X272
            /// </summary>
            SIZE_480X272,
            /// <summary>
            /// Resolution 480X360
            /// </summary>
            SIZE_480X360,
            /// <summary>
            /// Resolution 640X200
            /// </summary>
            SIZE_640X200,
            /// <summary>
            /// Resolution 640X350
            /// </summary>
            SIZE_640X350,
            /// <summary>
            /// Resolution 640X360
            /// </summary>
            SIZE_640X360,
            /// <summary>
            /// Resolution 640X480
            /// </summary>
            SIZE_640X480,
            /// <summary>
            /// Resolution 720X348
            /// </summary>
            SIZE_720X348,
            /// <summary>
            /// Resolution 720X350
            /// </summary>
            SIZE_720X350,
            /// <summary>
            /// Resolution 720X400
            /// </summary>
            SIZE_720X400,
            /// <summary>
            /// Resolution 720X480
            /// </summary>
            SIZE_720X480,
            /// <summary>
            /// Resolution 720X576
            /// </summary>
            SIZE_720X576,
            /// <summary>
            /// Resolution 800X600
            /// </summary>
            SIZE_800X600,
            /// <summary>
            /// Resolution 1024X768
            /// </summary>
            SIZE_1024X768,
            /// <summary>
            /// Resolution 1152X864
            /// </summary>
            SIZE_1152X864,
            /// <summary>
            /// Resolution 1280X720
            /// </summary>
            SIZE_1280X720,
            /// <summary>
            /// Resolution 1280X800
            /// </summary>
            SIZE_1280X800,
            /// <summary>
            /// Resolution 1280X1024
            /// </summary>
            SIZE_1280X1024,
            /// <summary>
            /// Resolution 1360X768
            /// </summary>
            SIZE_1360X768,
            /// <summary>
            /// Resolution 1366X768
            /// </summary>
            SIZE_1366X768,
            /// <summary>
            /// Resolution 1400X1050
            /// </summary>
            SIZE_1400X1050,
            /// <summary>
            /// Resolution 1440X900
            /// </summary>
            SIZE_1440X900,
            /// <summary>
            /// Resolution 1600X1200
            /// </summary>
            SIZE_1600X1200,
            /// <summary>
            /// Resolution 1680X1050
            /// </summary>
            SIZE_1680X1050,
            /// <summary>
            /// Resolution 1920X1080
            /// </summary>
            SIZE_1920X1080,
            /// <summary>
            /// Resolution 1920X1200
            /// </summary>
            SIZE_1920X1200,
            /// <summary>
            /// Resolution 2048X1080
            /// </summary>
            SIZE_2048X1080,
            /// <summary>
            /// Resolution 2048X1536
            /// </summary>
            SIZE_2048X1536,
            /// <summary>
            /// Resolution 2560X1600
            /// </summary>
            SIZE_2560X1600,
            /// <summary>
            /// Resolution 2560X2048
            /// </summary>
            SIZE_2560X2048,
            /// <summary>
            /// Resolution 3200X2048
            /// </summary>
            SIZE_3200X2048,
            /// <summary>
            /// Resolution 3200X2400
            /// </summary>
            SIZE_3200X2400,
            /// <summary>
            /// Resolution 3840X2160
            /// </summary>
            SIZE_3840X2160,
            /// <summary>
            /// Resolution 3840X2400
            /// </summary>
            SIZE_3840X2400,
            /// <summary>
            /// Resolution 4096X2160
            /// </summary>
            SIZE_4096X2160,
            /// <summary>
            /// Resolution 4096X3072
            /// </summary>
            SIZE_4096X3072,
            /// <summary>
            /// Resolution 5120X3200
            /// </summary>
            SIZE_5120X3200,
            /// <summary>
            /// Resolution 5120X4096
            /// </summary>
            SIZE_5120X4096,
            /// <summary>
            /// Resolution 6400X4096
            /// </summary>
            SIZE_6400X4096,
            /// <summary>
            /// Resolution 6400X4800
            /// </summary>
            SIZE_6400X4800,
            /// <summary>
            /// Resolution 7680X4320
            /// </summary>
            SIZE_7680X4320,
            /// <summary>
            /// Resolution 7680X4800
            /// </summary>
            SIZE_7680X4800,
        }

        /// <summary>
        /// The method that the Engine use to clear and redraw the screen
        /// </summary>
        public enum DrawingMethod {
            /// <summary>
            /// Setting the engine in this way make it so that it redraw only the gameObjects, it is the best setting for action games
            /// </summary>
            REDRAW_GAMEOBJECTS,
            /// <summary>
            /// This settings redraw everything at every frames, it is the slowest method, and not really advised sice REDRAW_GAMEOBJECTS works better anyway.
            /// </summary>
            REDRAW_EVERYTHING,
            /// <summary>
            /// This method make it so that the Engine draw only gameObject that moved, changed sprite, or had collisions, this the faster option, but this can lead to visual glitches if your game is a game in wich there are lots of moving gameObjects that are not physical, think about it carefully before choosing to use this option.
            /// </summary>
            REDRAW_MOVED_GAMEOBJECTS,
        }

        #endregion

        #region Engine Settings
        /// <summary>
        /// The destination in wich the Game Cycle will draw
        /// </summary>
        public static IDrawable drawDestination { get => _drawDestination; }
        private static IDrawable _drawDestination;

        /// <summary>
        /// This changes the way the Engine clean and redraw on the screen, read the Engine.DrawingMethod enum informations for info about the various options.
        /// </summary>
        public static DrawingMethod drawingMethod;
        #endregion

        #region Drawing variables
        internal static byte[] _drawBuffer;

        /// <summary>
        /// The buffer used to draw, use it only if you are creating a new IDrawable
        /// </summary>
        public static byte[] drawBuffer { get => _drawBuffer; }

        internal static int _drawBufferHeight;
        /// <summary>
        /// The height of the screen
        /// </summary>
        public static int bufferHeight { get => _drawBufferHeight; }

        internal static int _drawBufferWidth;
        /// <summary>
        /// The width of the screen
        /// </summary>
        public static int bufferWidth { get => _drawBufferWidth; }

        /// <summary>
        /// The stide of the bitmap used for the buffer
        /// </summary>
        public static int drawBufferStride { get => _drawBufferStride; }

        internal static int _drawBufferStride;

        #endregion


        #region Hidden engine methods for drawing
        private static void drawBackground() {
            if(_camera.background != null) drawSprite(_camera.background, 0, 0);
        }

        private static void cleanGameObjects(bool onlyMoved = false) {
            //cycle for every gameObject and clean it
            foreach(GameObject gameObject in gameObjects) {
                //i skip the current gameObject if it doesn't have a spriteSet
                if(gameObject.spriteSet == null) continue;
                if(onlyMoved) {
                    if(statusChanged(gameObject)) clean(gameObject);
                } else {
                    clean(gameObject);
                }
            }
        }

        private static void drawGameObjects(bool onlyMoved = false) {
            //cycle for every gameObject and clean it
            foreach(GameObject gameObject in gameObjects) {
                //i skip the current gameObject if it doesn't have a sprite to draw
                if(gameObject.spriteSet?.sprite == null) continue;
                if(onlyMoved) {
                    if(statusChanged(gameObject)) drawSprite(gameObject);
                } else {
                    drawSprite(gameObject);
                }
            }
        }

        private static void sortGameObjectByZ() {
            bool nothingReplaced;
            do {
                nothingReplaced = true;
                for(int i = gameObjects.Count - 1; i > 0 && nothingReplaced; i--) {
                    if(gameObjects[i].drawLayer < gameObjects[i - 1].drawLayer) {
                        GameObject temp = gameObjects[i];
                        gameObjects[i] = gameObjects[i - 1];
                        gameObjects[i - 1] = temp;
                        nothingReplaced = false;
                    }
                }
            } while(!nothingReplaced);
        }

        private static void drawSprite(GameObject gameObject) {
            if(getDrawData(gameObject, out int drawXPosition, out int drawWidth, out int spriteXPosition, out int drawYPosition, out int drawHeight, out int spriteYPosition)) {
                drawSpritePortion(gameObject.spriteSet.sprite,
                                  drawXPosition, drawYPosition,
                                  spriteXPosition, spriteYPosition,
                                  drawWidth, drawHeight);
            }
        }

        //this give the data necessary to draw partially the sprite, it return false if the sprite is out of screen
        private static bool getDrawData(GameObject gameObject, out int drawXPosition, out int drawWidth, out int spriteXPosition, out int drawYPosition, out int drawHeight, out int spriteYPosition, bool lastPosition = false, SpriteSet spriteSet = null) {
            //it is possible that this gameObject is completely (or partially) out of the screen, i need to calculate how much of the sprite should i draw, in wich position and with wich offset in the sprite.
            if(spriteSet == null) spriteSet = gameObject.spriteSet;

            if(lastPosition) {
                drawWidth = gameObject.lastSize.width;
                spriteXPosition = gameObject.lastMinSpriteCoordinates.x;

                drawHeight = gameObject.lastSize.height;
                spriteYPosition = gameObject.lastMinSpriteCoordinates.y;

                drawXPosition = gameObject.lastPixelPosition.x;
                drawYPosition = gameObject.lastPixelPosition.y;
            } else {
                drawWidth = spriteSet.size.width;
                spriteXPosition = spriteSet.minX;

                drawHeight = spriteSet.size.height;
                spriteYPosition = spriteSet.minY;

                drawXPosition = gameObject.pixelPosition.x;
                drawYPosition = gameObject.pixelPosition.y;
            }

            int originalXPosition = drawXPosition;
            int originalYPosition = drawYPosition;

            //checking if the gameObject is out of the screen to the left
            if(drawXPosition < 0) {
                drawWidth += originalXPosition;
                if(drawWidth < 0) return false;//The gameobject can't be drawn since it's completely out of the screen
                drawXPosition = 0;
                spriteXPosition += (spriteSet.size.width - drawWidth);
            }
            //checking if the gameObject is out of the screen to the right
            if((drawXPosition + drawWidth) > Engine._drawBufferWidth) {
                drawWidth += Engine._drawBufferWidth - (drawXPosition + drawWidth);
                if(drawWidth < 0) return false;//The gameobject can't be drawn since it's completely out of the screen
            }

            //checking if the gameObject is out of the screen in the upper part
            if(drawYPosition < 0) {
                drawHeight += originalYPosition;
                if(drawHeight < 0) return false;//The gameobject can't be drawn since it's completely out of the screen
                drawYPosition = 0;
                spriteYPosition += (spriteSet.size.height - drawHeight);
            }
            //checking if the gameObject is out of the screen in the lower part
            if((drawYPosition + drawHeight) > Engine._drawBufferHeight) {
                drawHeight += Engine._drawBufferHeight - (drawYPosition + drawHeight);
                if(drawHeight < 0) return false;//The gameobject can't be drawn since it's completely out of the screen
            }
            return true;
        }

        internal static void clean(GameObject gameObject, SpriteSet spriteSet = null) {
            if(_camera.background == null) return;
            if(getDrawData(gameObject, out int drawXPosition, out int drawWidth, out int spriteXPosition, out int drawYPosition, out int drawHeight, out int spriteYPosition, true)) {
                drawSpritePortion(_camera.background,
                                  drawXPosition, drawYPosition,
                                  drawXPosition, drawYPosition,
                                  drawWidth, drawHeight);
            }
        }

        private static bool statusChanged(GameObject gameObject) {
            return gameObject.invalidated || gameObject.lastPixelPosition != gameObject.pixelPosition || gameObject.lastSprite != gameObject.spriteSet?.sprite;
        }

        private static void drawSpritePortion(Sprite sprite, int drawXPosition, int drawYPosition, int spriteXPosition, int spriteYPosition, int width, int height) {
            byte[] spriteArray = sprite.pixelArray;
            int spriteStride = sprite.stride;

            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    int bufferPixelEnd = ((drawYPosition + y) * _drawBufferStride) + ((drawXPosition + x) * 3) + 2;
                    int spritePixelEnd = ((spriteYPosition + y) * spriteStride) + ((spriteXPosition + x) * 4) + 3;

                    byte alpha = sprite.pixelArray[spritePixelEnd];
                    if(alpha != 0) {
                        //(R)
                        spritePixelEnd--;
                        _drawBuffer[bufferPixelEnd] = Utility.alphaBlend(_drawBuffer[bufferPixelEnd], sprite.pixelArray[spritePixelEnd], alpha);
                        //(G)
                        spritePixelEnd--; bufferPixelEnd--;
                        _drawBuffer[bufferPixelEnd] = Utility.alphaBlend(_drawBuffer[bufferPixelEnd], sprite.pixelArray[spritePixelEnd], alpha);
                        //(B)
                        spritePixelEnd--; bufferPixelEnd--;
                        _drawBuffer[bufferPixelEnd] = Utility.alphaBlend(_drawBuffer[bufferPixelEnd], sprite.pixelArray[spritePixelEnd], alpha);
                    }
                }
            }
        }

        internal static void drawSprite(Sprite sprite, int xPosition, int yPosition) {
            drawSpritePortion(sprite, xPosition, yPosition, 0, 0, sprite.width, sprite.height);
        }


        #endregion

    }
}
