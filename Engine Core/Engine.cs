using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.IO;

namespace Daze {
    /// <summary>
    /// This is the core class of Daze, call Engine.Start() to start your game
    /// </summary>
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
            /// This settings redraw everything at every frames, it is the slowest method, and not really advised sice REDRAW_GAMEOBJECTS works better.
            /// </summary>
            REDRAW_EVERYTHING,
            /// <summary>
            /// This method make it so that the Engine draw only gameObject that moved, changed sprite, or had collisions, this the faster option, but this can lead to visual glitches if your game is a game in wich there are lots of moving gameObjects that are not physical, think about it carefully before choosing to use this option.
            /// </summary>
            //TODO: crea un flag invalidate per i gameObject così da poter ridisegnare qualsiasi gameObject ha avuto una collisione, dovrebbe tamponare un po' il problema del cleaning
            REDRAW_MOVED_GAMEOBJECTS,
        }

        #endregion

        #region Variables
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

        private static Camera _camera = new Camera();
        /// <summary>
        /// This is the camera that is showing on the screen (Daze currently support only one camera)
        /// </summary>
        public static Camera camera {get => _camera; }

        /// <summary>
        /// You can hook up an action to the engine to know when the game is not active and make it stop
        /// </summary>
        public static Action lostFocus;
        /// <summary>
        /// You can hook up an action to the engine to know when the game is active again and make it resume
        /// </summary>
        public static Action gotFocus;
        
        /// <summary>
        /// Setting this flag to true will show the FPS count in the console and the difference between the game cycle and the draw time, you should use it only if you are experiencing heavy FPS drop and you have no idea what's going on
        /// </summary>
        public static bool printFpsFlag = false;
        internal static bool stopCycle = false;
        #endregion
        #region Time related variables
        /// <summary>
        /// The milliseconds that the last game cycle took
        /// This can be used to do physics calculation regardless of FPS.
        /// </summary>
        private static float _deltaTime;
        /// <summary>
        /// Use this function to make your game frame rate independent, an example: by multiplying 2 in the Update for this you are basically saying 2 per second
        /// </summary>
        public static float deltaTime { get { return _deltaTime; } }
        
        private static float lastCycleMS = 0; private static float lastCycleDrawMS = 0;

        private static int targetCycleMS;
        #endregion
        #region Private Game Lists
        //Game Scripts Lists
        private static List<GameScript> gameScripts;

        private static List<GameObject> gameObjects;
        private static List<GameObject> newGameObjects;
        private static List<GameObject> toDeleteGameObjects;

        //Preloading lists
        internal static Dictionary<string, Sprite> sprites;
        internal static Dictionary<string, string> wavPaths;

        //sounds list
        internal static List<Wav> playingWavs;
        #endregion
        #region Drawing variables

        internal static byte[] _drawBuffer;

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

        public static int drawBufferStride { get => _drawBufferStride; }

        internal static int _drawBufferStride;

        #endregion
        #endregion

        #region Event handlers
        public static MouseEventHandler mouseClick;
        public static MouseEventHandler mouseDoubleClick;

        public static MouseEventHandler mouseMove;

        public static MouseEventHandler mouseDown;
        public static MouseEventHandler mouseUp;

        private static void mouseClicked(object sender, MouseEventArgs e) {
            foreach(GameObject gameObject in gameObjects) {
                if(toDeleteGameObjects.Contains(gameObject)) continue;
                if(gameObject.spriteSet != null && gameObject.spriteSet.sprite != null) {
                    if(Geometry.Utility.between(e.X, gameObject.pixelPosition.x, gameObject.pixelPosition.x + gameObject.spriteSet.sprite.width) &&
                       Geometry.Utility.between(e.Y, gameObject.pixelPosition.y, gameObject.pixelPosition.y + gameObject.spriteSet.sprite.height)) {
                        gameObject.mouseClick?.Invoke(gameObject, e);
                    }
                }
            }
        }

        private static void mouseDoubleClicked(object sender, MouseEventArgs e) {
            foreach(GameObject gameObject in gameObjects) {
                if(toDeleteGameObjects.Contains(gameObject)) continue;
                if(gameObject.spriteSet != null && gameObject.spriteSet.sprite != null) {
                    if(Geometry.Utility.between(e.X, gameObject.pixelPosition.x, gameObject.pixelPosition.x + gameObject.spriteSet.sprite.width) &&
                       Geometry.Utility.between(e.Y, gameObject.pixelPosition.y, gameObject.pixelPosition.y + gameObject.spriteSet.sprite.height)) {
                        gameObject.mouseDoubleClick?.Invoke(gameObject, e);
                    }
                }
            }
        }

        private static void mouseMoved(object sender, MouseEventArgs e) {
            foreach(GameObject gameObject in gameObjects) {
                if(toDeleteGameObjects.Contains(gameObject)) continue;
                if(gameObject.spriteSet != null && gameObject.spriteSet.sprite != null) {
                    if(Geometry.Utility.between(e.X, gameObject.pixelPosition.x, gameObject.pixelPosition.x + gameObject.spriteSet.sprite.width) &&
                       Geometry.Utility.between(e.Y, gameObject.pixelPosition.y, gameObject.pixelPosition.y + gameObject.spriteSet.sprite.height)) {
                        gameObject.mouseMove?.Invoke(gameObject, e);
                    }
                }
            }
        }

        private static void mouseButtonDown(object sender, MouseEventArgs e) {
            foreach(GameObject gameObject in gameObjects) {
                if(toDeleteGameObjects.Contains(gameObject)) continue;
                if(gameObject.spriteSet != null && gameObject.spriteSet.sprite != null) {
                    if(Geometry.Utility.between(e.X, gameObject.pixelPosition.x, gameObject.pixelPosition.x + gameObject.spriteSet.sprite.width) &&
                       Geometry.Utility.between(e.Y, gameObject.pixelPosition.y, gameObject.pixelPosition.y + gameObject.spriteSet.sprite.height)) {
                        gameObject.mouseDown?.Invoke(gameObject, e);
                    }
                }
            }
        }

        private static void mouseButtonUp(object sender, MouseEventArgs e) {
            foreach(GameObject gameObject in gameObjects) {
                if(toDeleteGameObjects.Contains(gameObject)) continue;
                if(gameObject.spriteSet != null && gameObject.spriteSet.sprite != null) {
                    if(Geometry.Utility.between(e.X, gameObject.pixelPosition.x, gameObject.pixelPosition.x + gameObject.spriteSet.sprite.width) &&
                       Geometry.Utility.between(e.Y, gameObject.pixelPosition.y, gameObject.pixelPosition.y + gameObject.spriteSet.sprite.height)) {
                        gameObject.mouseUp?.Invoke(gameObject, e);
                    }
                }
            }
        }
        #endregion

        #region Functions
        #region Start/Stop functions
        /// <summary>
        /// This function start the Engine
        /// </summary>
        /// <param name="FPSLimit">The maximum FPS that the Engine should reach, don't specify it if you don't need it</param>
        /// <param name="renderingSize">The internal rendering size</param>
        /// <param name="drawingMethod">The Engine drawing method, see Engine.DrawingMethod to get more info</param>
        public static void Start(int FPSLimit = 60, RenderingSize renderingSize = RenderingSize.SIZE_1280X720, DrawingMethod drawingMethod = DrawingMethod.REDRAW_GAMEOBJECTS, IDrawable drawDestination = null) {
            #region Initial Setup
            //draw setup
            _drawDestination = drawDestination;
            Engine.drawingMethod = drawingMethod;
            
            //calcolo il timeSpan di un Update necessario per non superare il limite di troppo il limite di FPS
            targetCycleMS = (targetCycleMS = 1000 / FPSLimit + 1) > 0 ? targetCycleMS : 1;

            //inizializzo l'utility per la generazione di numeri casuali
            Utility.random = new Random();
            if(_drawDestination == null) {
                _drawDestination = new DrawOnWinform();
            }
            #region Finestra e buffer
            //creo la finestra del gioco
            _drawDestination.IntialSetup();

            //inizializzo il buffer di Daze
            string sizeString = renderingSize.ToString();
            int startNumber = sizeString.IndexOf("_")+1;
            int xPosition = sizeString.IndexOf("X");
            _drawBufferWidth = int.Parse(sizeString.Substring(startNumber, xPosition - startNumber));
            _drawBufferHeight = int.Parse(sizeString.Substring(xPosition+1));
            _drawBuffer = new byte[_drawBufferWidth * _drawBufferHeight * 3];
            _drawBufferStride = _drawBufferWidth * 3;

            //inizializzo il buffer della finestra
            _drawDestination.BufferSetup();
            #endregion

            #region Eventi
            mouseClick += mouseClicked;
            mouseDoubleClick += mouseDoubleClicked;

            mouseMove += mouseMoved;

            mouseDown += mouseButtonDown;
            mouseUp += mouseButtonUp;
            #endregion

            #region Inizializzazione liste
            //Inizializzo le liste di preloading
            sprites = new Dictionary<string, Sprite>();
            wavPaths = new Dictionary<string, string>();

            //inizializzo la lista dei suoni
            playingWavs = new List<Wav>();

            //inizializzo lista gameobjects e degli script
            gameObjects = new List<GameObject>();
            newGameObjects = new List<GameObject>();
            toDeleteGameObjects = new List<GameObject>();

            gameScripts = new List<GameScript>();

            //trovo gli script di gioco, li inizializzo, li avvio e me li salvo in una lista
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach(Type type in assembly.GetTypes()) {
                    foreach(Type interfaceType in type.GetInterfaces()) {
                        if(interfaceType == typeof(GameScript) && type != typeof(GameScript) && !type.IsSubclassOf(typeof(GameObject)) && type != typeof(GameObject)) {
                            //questa classe implementa GameScript, pertanto è uno script di gioco
                            GameScript script = (GameScript)Activator.CreateInstance(type);
                            script.Start();
                            gameScripts.Add(script);
                        }
                    }
                }
            }
            #endregion

            //avvio la finestra del gioco
            _drawDestination.Start();

            #region Wait till the window is loaded
            while(!_drawDestination.loaded) {
                Thread.Sleep(100);
                Application.DoEvents();
            }
            #endregion

            //avvio il ciclo di gioco
            GameCycle();
            #endregion
        }

        /// <summary>
        /// This stop the Engine,
        /// Use this to close the game
        /// </summary>
        public static void Stop() {
            _drawDestination.Stop();
        }
        #endregion

        private static void GameCycle() {
            #region Initial gameCycle setup
            //creo lo stopwatch per il ciclo di gioco
            Stopwatch stopwatch = new Stopwatch();

            //disegno lo sfondo
            drawBackground();
            #endregion

            while(!stopCycle) {
                //reinizializzo lo stopwatch per misurare questo ciclo
                stopwatch.Restart();

                #region Saving last draw status before all the scripts
                foreach(GameObject gameObject in gameObjects) {
                    //mi segno la posizione corrente dell'oggetto come posizione preUpdate così da poterla poi pulire se necessario
                    gameObject.pushLastPixelPosition();
                    gameObject.lastSprite = gameObject.spriteSet?.sprite;
                }
                #endregion

                #region Executing scripts
                #region GameScripts
                foreach(GameScript gameScript in gameScripts) {
                    gameScript.Update();
                }
                #endregion

                #region GameObjects scripts
                //esecuzione degli script collegati a un gameObject
                foreach(GameObject gameObject in gameObjects) {
                    #region Timers
                    //aggiorno i millisecondi dei timer dell'oggetto prima di eseguire l'Update
                    foreach(Timer timer in gameObject.timers) {
                        //non aggiorno il timer se è un timer di uno spriteSet e non è attivo
                        if(timer.ID < 0 && timer.ID != gameObject.spriteSet?.timerID) continue;
                        if(timer.currentMS < timer.msPerTick) timer.currentMS += lastCycleMS;
                        if(timer.ticked()) {
                            timer.tickAction?.Invoke();
                        }
                    }
                    //aggiunta timer appena creati alla lista dei gameObject
                    for(int i = gameObject.newTimers.Count - 1; i >= 0; i--) {
                        gameObject.timers.Add(gameObject.newTimers[i]);
                        gameObject.newTimers.RemoveAt(i);
                    }
                    //cancellazione timer appena cancellati dalla lista dei gameObject
                    for(int i = gameObject.toDeleteTimers.Count - 1; i >= 0; i--) {
                        gameObject.timers.Remove(gameObject.toDeleteTimers[i]);
                        gameObject.toDeleteTimers.RemoveAt(i);
                    }
                    #endregion
                    gameObject.Update();
                }

                #endregion

                #region GameObject lists editing
                //aggiunta gameobject appena creati alla lista dei gameObject
                for(int i = newGameObjects.Count - 1; i >= 0; i--) {
                    newGameObjects[i].Start();
                    gameObjects.Add(newGameObjects[i]);
                    newGameObjects.RemoveAt(i);
                }
                //cancellazione gameobject appena cancellati dalla lista dei gameObject
                for(int i = toDeleteGameObjects.Count - 1; i >= 0; i--) {
                    if(toDeleteGameObjects[i]?.spriteSet != null) clean(toDeleteGameObjects[i]);
                    gameObjects.Remove(toDeleteGameObjects[i]);
                    toDeleteGameObjects.RemoveAt(i);
                }
                #endregion
                #endregion

                #region Updating new pixel positions
                _camera.pushPixelPosition();
                foreach(GameObject gameObject in gameObjects) {
                    gameObject.pushPixelPosition();
                }
                #endregion

                #region Draw to buffer
                //sorting game objects by draw priority
                sortGameObjectByZ();

                #region Draw on buffer operations
                switch(drawingMethod) {
                    case DrawingMethod.REDRAW_EVERYTHING:
                        drawBackground();
                        drawGameObjects();
                        break;
                    case DrawingMethod.REDRAW_GAMEOBJECTS:
                        cleanGameObjects();
                        drawGameObjects();
                        break;
                    case DrawingMethod.REDRAW_MOVED_GAMEOBJECTS:
                        cleanGameObjects(true);
                        drawGameObjects(true);
                        break;
                }
                #endregion

                float now = (float)stopwatch.Elapsed.TotalMilliseconds;
                //aggiorno l'immagine sullo schermo
                _drawDestination.Draw();
                Application.DoEvents();
                #endregion

                #region FPS check
                //aspetto il numero di MS necessari per arrivare al timestep oppure aspetto 0MS se l'esecuzione dell'update ne ha richiesti più di 100
                if(targetCycleMS > 0) {
                    int temp = targetCycleMS - (int)stopwatch.Elapsed.TotalMilliseconds;
                    if(temp > 0) Thread.Sleep(temp);
                }

                //ora dopo lo sleep posso calcolare i tempi necessari per questo ciclo
                lastCycleMS = (float)stopwatch.Elapsed.TotalMilliseconds;
                if(printFpsFlag) lastCycleDrawMS = lastCycleMS - now;

                //imposto il deltaTime per le simulazioni fisiche=
                _deltaTime = lastCycleMS / 1000;

                if(printFpsFlag) printFPS();
                #endregion
            }
        }

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

        //da in output i dati per disegnare parzialmente lo sprite, restituisce false se lo sprite non è disegnabile
        private static bool getDrawData(GameObject gameObject, out int drawXPosition, out int drawWidth, out int spriteXPosition, out int drawYPosition, out int drawHeight, out int spriteYPosition, bool lastPosition = false, SpriteSet spriteSet = null) {
            //è possibile che il gameObject sia fuori (o parzialmente fuori) dallo schermo, devo capire quanto disegnare del gameObject, e in che posizione
            if(spriteSet == null) spriteSet = gameObject.spriteSet;

            drawWidth = spriteSet.size.width;
            spriteXPosition = spriteSet.minX;

            drawHeight = spriteSet.size.height;
            spriteYPosition = spriteSet.minY;

            if(lastPosition) {
                drawXPosition = gameObject.lastPixelPosition.x;
                drawYPosition = gameObject.lastPixelPosition.y;
            } else {
                drawXPosition = gameObject.pixelPosition.x;
                drawYPosition = gameObject.pixelPosition.y;
            }

            int originalXPosition = drawXPosition;
            int originalYPosition = drawYPosition;

            //controllo se il gameObject è fuori a sinistra
            if(drawXPosition < 0) {
                drawWidth += originalXPosition;
                if(drawWidth < 0) return false;// il gameObject non è disegnabile in quanto totalmente fuori dallo schermo
                drawXPosition = 0;
                spriteXPosition += (spriteSet.size.width - drawWidth);
            }
            //controllo se il gameObject è fuori a destra
            if((drawXPosition + drawWidth) > Engine._drawBufferWidth) {
                drawWidth += Engine._drawBufferWidth - (drawXPosition + drawWidth);
                if(drawWidth < 0) return false;// il gameObject non è disegnabile in quanto totalmente fuori dallo schermo
            }

            //controllo se il gameObject è fuori in alto
            if(drawYPosition < 0) {
                drawHeight += originalYPosition;
                if(drawHeight < 0) return false;// il gameObject non è disegnabile in quanto totalmente fuori dallo schermo
                drawYPosition = 0;
                spriteYPosition += (spriteSet.size.height - drawHeight);
            }
            //controllo se il gameObject è fuori in basso
            if((drawYPosition + drawHeight) > Engine._drawBufferHeight) {
                drawHeight += Engine._drawBufferHeight - (drawYPosition + drawHeight);
                if(drawHeight < 0) return false;// il gameObject non è disegnabile in quanto totalmente fuori dallo schermo
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
            return gameObject.lastPixelPosition != gameObject.pixelPosition || gameObject.lastSprite != gameObject.spriteSet?.sprite;
        }

        private static void drawSpritePortion(Sprite sprite, int drawXPosition, int drawYPosition, int spriteXPosition, int spriteYPosition, int width, int height) {
            byte[] spriteArray = sprite.pixelArray;
            int spriteStride = sprite.stride;

            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    int bufferPixelEnd = ((drawYPosition+y) * _drawBufferStride) + ((drawXPosition + x) * 3) + 2;
                    int spritePixelEnd = ((spriteYPosition+y) * spriteStride) + ((spriteXPosition + x) * 4) + 3;

                    byte alpha =  sprite.pixelArray[spritePixelEnd];
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

        #region Methods for GameObject operations
        #region Methods for getting informations
        /// <summary>
        /// Get all the gameObject currently in the Game
        /// </summary>
        /// <returns>An array that contains all the gameObject that are not deleted</returns>
        public static GameObject[] findGameObjects() {
            GameObject[] output = new GameObject[newGameObjects.Count+gameObjects.Count];
            for(int i = 0; i < gameObjects.Count; i++) {
                output[i] = gameObjects[i];
            }
            for(int i = gameObjects.Count; i < (gameObjects.Count + newGameObjects.Count); i++) {
                output[i] = newGameObjects[i - gameObjects.Count];
            }
            return gameObjects.ToArray();
        }
        /// <summary>
        /// Get all the gameObjects of a given GameObject subClass
        /// </summary>
        /// <typeparam name="GameObjectClass">The type of the gameObjects to find</typeparam>
        public static List<GameObjectClass> findGameObjects<GameObjectClass>() {
            List <GameObjectClass> output = new List<GameObjectClass>();
            foreach(GameObject gameObject in gameObjects) {
                if(typeof(GameObjectClass).IsInstanceOfType(gameObject)) {
                    output.Add((GameObjectClass)Convert.ChangeType(gameObject, typeof(GameObjectClass)));
                }
            }
            foreach(GameObject gameObject in newGameObjects) {
                if(typeof(GameObjectClass).IsInstanceOfType(gameObject)) {
                    output.Add((GameObjectClass)Convert.ChangeType(gameObject, typeof(GameObjectClass)));
                }
            }
            return output;
        }

        /// <summary>
        /// Check if a GameObject exists currently
        /// </summary>
        /// <param name="gameObject">The GameObject to search</param>
        public static bool gameObjectExists(GameObject gameObject) {
            return gameObjects.Contains(gameObject) || newGameObjects.Contains(gameObject);
        }
        /// <summary>
        /// Check if a GameObject is destroyed
        /// </summary>
        /// <param name="gameObject">The GameObject to search</param>
        /// <returns>true if the gameObject doesn't exists</returns>
        public static bool isGameObjectDestroyed(GameObject gameObject) {
            return ((!gameObjects.Contains(gameObject)) && !newGameObjects.Contains(gameObject)) || toDeleteGameObjects.Contains(gameObject);
        }
        #endregion

        #region Methods for modifying the GameObject list
        /// <summary>
        /// NOT RECOMMENDED: This method add forcefully a gameObject in the gameObjects list, when a gameObject is created it is automaticaly added to the list, don't use this method if you are not sure what you are doing
        /// </summary>
        /// <param name="gameObject">The GameObject to add</param>
        public static void AddGameObject(GameObject gameObject) {
            newGameObjects?.Add(gameObject);
        }

        /// <summary>
        /// Delete a gameObject from the gameObject list
        /// </summary>
        /// <param name="gameObject">The gameObject to remove</param>
        public static void DeleteGameObject(GameObject gameObject) {
            toDeleteGameObjects.Add(gameObject);
        }

        #endregion
        #endregion

        #region Methods for preloading
        /// <summary>
        /// This method load a Sprite from a resource.
        /// After that you loaded a sprite in this way the next load for the same sprite will not happen and you will just get a reference to the Sprite, so feel free to call it for every GameObject you need, you won't slow down the game due to I/O operations.
        /// </summary>
        /// <param name="resource_Name"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Sprite loadSprite(string resource_Name, float scale = 1) {
            string spriteName = resource_Name+"x"+scale;
            //cerco se lo sprite esiste già
            foreach(KeyValuePair<string, Sprite> keyVal in sprites) {
                if(keyVal.Key == spriteName) {
                    return keyVal.Value;
                }
            }
            //se sono qui allora lo sprite non è mai stato caricato
            //ottengo il namespace del metodo che ha chiamato questo metodo
            Assembly callerAssembly = new StackTrace().GetFrame(1).GetMethod().ReflectedType.Assembly;
            Bitmap bitmap = null;
            //ottengo i file di risorse 
            foreach(string rsxName in callerAssembly.GetManifestResourceNames()) {
                try {
                    ResourceManager rm = new ResourceManager(rsxName.Replace(".Resources.resources",".Resources"),callerAssembly);
                    bitmap = (Bitmap)rm.GetObject(resource_Name);
                    break;
                } catch { }
            }
            if(bitmap == null) throw new Exception("Can't find the sprite " + resource_Name + ": the name must be the same as the Resource's name");
            Sprite newSprite = new Sprite(Utility.scaleImage(bitmap,scale));
            sprites.Add(spriteName, newSprite);
            return newSprite;
        }

        /// <summary>
        /// This method load a Wav from a resource embedded in your project
        /// </summary>
        /// <param name="resource_Name">The name of the resource representing the wav</param>
        /// <param name="volume">The volume of the Wav (from 0 to 100)</param>
        /// <param name="loop">True if you want the sound to loop till you pause or stop it</param>
        /// <param name="callerAssembly">The assembly in wich the resources are placed, if you don't set this it will be the assembly that call this function</param>
        /// <returns>The Wav loaded</returns>
        public static Wav loadWavFromResources(string resource_Name,int volume = 100, bool loop = false, Assembly callerAssembly = null) {
            //cerco se il wav è già stato estratto
            foreach(KeyValuePair<string, string> keyVal in wavPaths) {
                if(keyVal.Key == resource_Name) {
                    return new Wav(keyVal.Value, volume, loop);
                }
            }
            //se sono qui allora il wav non è mai stato caricato
            //ottengo il namespace del metodo che ha chiamato questo metodo
            if(callerAssembly == null) callerAssembly = new StackTrace().GetFrame(1).GetMethod().ReflectedType.Assembly;
            //ottengo i file di risorse 
            string tempFile = null;
            foreach(string rsxName in callerAssembly.GetManifestResourceNames()) {
                try {
                    ResourceManager rm = new ResourceManager(rsxName.Replace(".Resources.resources", ".Resources"), callerAssembly);
                    tempFile = writeWavStreamToTempLocation(rm.GetStream(resource_Name));
                    break;
                } catch(Exception ex) {
                    Console.WriteLine(ex);
                }
            }
            if(tempFile == null) throw new Exception("Can't find the wav " + resource_Name + ": the name must be the same as the Resource's name");
            return new Wav(tempFile, volume, loop);
        }
        /// <summary>
        /// This method load a wav from a file
        /// </summary>
        /// <param name="filePath">The path of the file to load</param>
        /// <param name="volume">The volume of the Wav (from 0 to 100)</param>
        /// <param name="loop">True if you want this Wav to restart at the end of the file</param>
        /// <returns></returns>
        public static Wav loadWavFromFile(string filePath, int volume = 100, bool loop = false) {
            return new Wav(filePath, volume, loop);
        }

        private static string writeWavStreamToTempLocation(UnmanagedMemoryStream inputStream) {
            string filePath = String.Format(System.IO.Path.GetTempPath() + Guid.NewGuid().ToString("N") + ".wav");
            using(FileStream outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
                int readByte;
                byte[] copyBuffer = new byte[524288]; //0,5MB alla volta (512*1024 byte)
                while((readByte = inputStream.Read(copyBuffer, 0, copyBuffer.Length)) > 0) {
                    outputStream.Write(copyBuffer, 0, readByte);
                }
                outputStream.Flush();
            }
            inputStream.Dispose();
            return filePath;
        }
        #endregion

        #region Method for playing sounds for people so lazy that they don't want to create a variable for the Wav
        /// <summary>
        /// This method play a Wav from the resources.
        /// Using this method you don't need to manage the Wav initialization and disposition when the sound ends
        /// </summary>
        /// <param name="resource_Name"></param>
        /// <param name="volume">The volume of the Wav (from 0 to 100)</param>
        public static void playWavFromResources(string resource_Name, int volume = 100) {
            Wav newWav = loadWavFromResources(resource_Name, volume, false, new StackTrace().GetFrame(1).GetMethod().ReflectedType.Assembly);
            newWav.disposeAtEnd = true;
            playingWavs.Add(newWav);
            newWav.Play();
        }
        /// <summary>
        /// This method play a Wav from a file.
        /// Using this method you don't need to manage the Wav initialization and disposition when the sound ends
        /// </summary>
        /// <param name="filePath">The path of the Wav file</param>
        public static void playWavFromFile(string filePath) {
            Wav newWav = loadWavFromFile(filePath);
            newWav.disposeAtEnd = true;
            playingWavs.Add(newWav);
            newWav.Play();
        }
        #endregion

        #region Diagnostic Methods
        private static void printFPS() {
            if(_drawDestination.focus) Console.WriteLine("FPS:" + (int)(1000f / lastCycleMS) + " MS:" + lastCycleMS + " DRAW:" + lastCycleDrawMS);
        }
        #endregion
        #endregion
    }
}
