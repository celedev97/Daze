using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Linq.Expressions;
using System.Resources;

namespace Daze {
<<<<<<< HEAD
    public partial class Engine {
        #region Variables
        #region Engine Settings
        private static Camera _camera = new Camera();
        public static Camera camera {get => camera; }
=======
    public unsafe class Engine {
        public static Camera camera = new Camera();
>>>>>>> 84a047f1bcbd99d313f202b4c6b43b160f16d8b1

        public static Action lostFocus;
        public static Action gotFocus;

        private static GameForm _window;
        public static GameForm window{get=>_window;}

        private static bool _cursorHide = false;
        public static bool cursorHide {
            get => _cursorHide;
            set {
                _cursorHide = value;
                if(value) {
                    Cursor.Hide();
                } else {
                    Cursor.Show();
                }
            }
        }
        
        public static bool PRINT_FPS = false;
        #endregion
        #region Time related variables
        /// <summary>
        /// The milliseconds that the last game cycle took
        /// This can be used to do physics calculation regardless of FPS.
        /// </summary>
<<<<<<< HEAD
        private static float _deltaTime;
        public static float deltaTime { get { return _deltaTime; } }
        
        private static float lastCycleMS = 0; private static float lastCycleDrawMS = 0;

        private static int timeSpan;
        #endregion
        #region Private Game Lists
        //Game Scripts Lists
        private static List<GameScript> gameScripts;
=======
        public static float deltaTime { get { return _deltaTime; } }



        private static float _deltaTime;
        private static float lastCycleMS = 0; private static float lastCycleDrawMS = 0;
        #endregion

        #region _ engine constants/variables
        internal static GameForm window;

        internal static List<GameScript> gameScripts;
>>>>>>> 84a047f1bcbd99d313f202b4c6b43b160f16d8b1

        private static List<GameObject> gameObjects;
        private static List<GameObject> newGameObjects;
        private static List<GameObject> toDeleteGameObjects;

<<<<<<< HEAD
        //Sprite List
=======
        private static int timeSpan;

>>>>>>> 84a047f1bcbd99d313f202b4c6b43b160f16d8b1
        internal static Dictionary<string,Sprite> sprites;
        #endregion
        #region Drawing variables
        internal static byte[] drawBuffer;

        internal static int _drawBufferHeight;
        public static int drawBufferHeight { get => _drawBufferHeight; }
        internal static int _drawBufferWidth;
        public static int drawBufferWidth { get => _drawBufferWidth; }

        internal static int drawBufferStride;

        #endregion
        #endregion

        #region Functions
        #region Start/Stop functions
        public static void Start(int FPSLimit = 60) {
            #region Initial Setup
            //calcolo il timespan di un Update necessario per non superare il limite di troppo il limite di FPS
            timeSpan = (timeSpan = 1000 / FPSLimit + 1) > 0 ? timeSpan : 1;

            //inizializzo l'utility per la generazione di numeri casuali
            Utility.random = new Random();

            #region Finestra e buffer
            //creo la finestra del gioco
            _window = new GameForm();

            //inizializzo il doppio buffer
            _drawBufferWidth = Screen.PrimaryScreen.Bounds.Width;
            _drawBufferHeight = Screen.PrimaryScreen.Bounds.Height;

            _window.buffer = new Bitmap(_drawBufferWidth, _drawBufferHeight);
            using(Graphics g = Graphics.FromImage(_window.buffer)) {
                g.DrawRectangle(new Pen(Color.Blue), 0, 0, _drawBufferWidth, _drawBufferHeight);
            }

<<<<<<< HEAD
            drawBuffer = new byte[_drawBufferWidth * _drawBufferHeight * 3];
            drawBufferStride = _drawBufferWidth * 3;
=======
            drawBuffer = new byte[drawBufferWidth * drawBufferHeight * 3];
            drawBufferStride = drawBufferWidth * 3;
>>>>>>> 84a047f1bcbd99d313f202b4c6b43b160f16d8b1
            #endregion

            #region Inizializzazione liste
            //Inizializzo la lista degli sprite
            sprites = new Dictionary<string, Sprite>();

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
<<<<<<< HEAD

            //avvio la finestra del gioco (avviarla prima avrebbe bloccato l'esecuzione del codice)
            _window.Show();
            _window.Activate();
=======
            
            //avvio la finestra del gioco (avviarla prima avrebbe bloccato l'esecuzione del codice)
            window.Show();
            window.Activate();
>>>>>>> 84a047f1bcbd99d313f202b4c6b43b160f16d8b1

            //avvio il ciclo di gioco
            GameCycle();
            #endregion
        }

<<<<<<< HEAD
        public static void Stop() {
            _window.Close();
=======
        private static void printFPS() {
            if(window.focus) Console.WriteLine("FPS:" + (int)(1000f / lastCycleMS) + " MS:" + lastCycleMS + " DRAW:" + lastCycleDrawMS);
>>>>>>> 84a047f1bcbd99d313f202b4c6b43b160f16d8b1
        }
        #endregion

        private static void GameCycle() {
            while(!_window.loaded) {
                Thread.Sleep(100);
                Application.DoEvents();
            }
            //creo lo stopwatch per il ciclo di gioco
            Stopwatch stopwatch = new Stopwatch();

            //disegno lo sfondo
            drawSprite(_camera.background, 0, 0);

            while(true) {
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
                    gameObjects.Add(newGameObjects[i]);
                    newGameObjects.RemoveAt(i);
                }
                //cancellazione gameobject appena cancellati dalla lista dei gameObject
                for(int i = toDeleteGameObjects.Count - 1; i >= 0; i--) {
                    clean(toDeleteGameObjects[i], false);
                    gameObjects.Remove(toDeleteGameObjects[i]);
                    toDeleteGameObjects.RemoveAt(i);
                }
                #endregion
                #endregion

<<<<<<< HEAD
                #region Updating new pixel positions
                _camera.pushPixelPosition();
                foreach(GameObject gameObject in gameObjects) {
                    gameObject.pushPixelPosition();
                }
                #endregion

                #region Updating the screen
                //sorting game objects by draw priority
                sortGameObjectByZ();

                #region Draw on buffer operations
                //cycle for every gameObject and clean it
                foreach(GameObject gameObject in gameObjects) {
                    //i skip the current gameObject if it doesn't have a spriteSet
                    if(gameObject.spriteSet == null) continue;
                    clean(gameObject);
                }

                //cycle for every gameObject and draw it again
                foreach(GameObject gameObject in gameObjects) {
                    //i skip the current gameObject if it doesn't have a spriteSet
                    if(gameObject.spriteSet == null) continue;
                    //i don't draw the sprite if there is no sprite
                    if(gameObject.spriteSet.sprite == null) continue;
                    drawSprite(gameObject);
                }
                #endregion

                float now = (float)stopwatch.Elapsed.TotalMilliseconds;
                //aggiorno l'immagine sullo schermo
                _window.updateImage();
                Application.DoEvents();
                #endregion

                #region FPS check
=======
                #region Aggiorno lo schermo
                
                Draw();

                float now = (float)stopwatch.Elapsed.TotalMilliseconds;
                //aggiorno l'immagine sullo schermo
                window.updateImage();
                Application.DoEvents();
                #endregion

                #region FPS CONTROL

>>>>>>> 84a047f1bcbd99d313f202b4c6b43b160f16d8b1
                //aspetto il numero di MS necessari per arrivare al timestep oppure aspetto 0MS se l'esecuzione dell'update ne ha richiesti più di 100
                if(timeSpan > 0) {
                    int temp = timeSpan - (int)stopwatch.Elapsed.TotalMilliseconds;
                    if(temp > 0) Thread.Sleep(temp);
                }

                //ora dopo lo sleep posso calcolare i tempi necessari per questo ciclo
                lastCycleMS = (float)stopwatch.Elapsed.TotalMilliseconds;
                lastCycleDrawMS = lastCycleMS - now;//CANCELLA IN RELEASE
<<<<<<< HEAD

                //imposto il deltaTime per le simulazioni fisiche=
                _deltaTime = lastCycleMS / 1000;
=======

                //imposto il deltaTime per le simulazioni fisiche=
                _deltaTime = lastCycleMS / 1000;

                printFPS();
                #endregion
            }
        }

        #region _ engine methods for drawing
        internal static void Draw() {
            sortGameObjectByZ();
>>>>>>> 84a047f1bcbd99d313f202b4c6b43b160f16d8b1

                printFPS();
                #endregion
            }
        }

        #region Hidden engine methods for drawing
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
        private static bool getDrawData(GameObject gameObject, out int drawXPosition, out int drawWidth, out int spriteXPosition, out int drawYPosition, out int drawHeight, out int spriteYPosition, bool lastPosition = false) {
            //è possibile che il gameObject sia fuori (o parzialmente fuori) dallo schermo, devo capire quanto disegnare del gameObject, e in che posizione

            drawWidth = gameObject.spriteSet.size.width;
            spriteXPosition = gameObject.spriteSet.minX;

            drawHeight = gameObject.spriteSet.size.height;
            spriteYPosition = gameObject.spriteSet.minY;

            if(lastPosition) {
                drawXPosition = gameObject.lastPixelPosition.x;
                drawYPosition = gameObject.lastPixelPosition.y;
            } else {
                drawXPosition = gameObject.pixelPosition.x;
                drawYPosition = gameObject.pixelPosition.y;
            }

            int originalXPosition = drawXPosition;
            int originalYPosition = drawYPosition;

            //controllo se il gameObject è fuori dallo schermo da uno dei due lati orizzontali
            if(drawXPosition < 0) {
                drawWidth += originalXPosition;
                if(drawWidth < 0) return false;// il gameObject non è disegnabile in quanto totalmente fuori dallo schermo
                drawXPosition = 0;
                spriteXPosition += (gameObject.spriteSet.size.width - drawWidth);
            } else if((drawXPosition + drawWidth) > Engine._drawBufferWidth) {
                drawWidth += Engine._drawBufferWidth - (originalXPosition + drawWidth);
                if(drawWidth < 0) return false;// il gameObject non è disegnabile in quanto totalmente fuori dallo schermo
            }

            //controllo se il gameObject è fuori dallo schermo da uno dei due lati verticali
            if(drawYPosition < 0) {
                drawHeight += originalYPosition;
                if(drawHeight < 0) return false;// il gameObject non è disegnabile in quanto totalmente fuori dallo schermo
                drawYPosition = 0;
                spriteYPosition += (gameObject.spriteSet.size.height - drawHeight);
            } else if((drawYPosition + drawHeight) > Engine._drawBufferHeight) {
                drawHeight += Engine._drawBufferHeight - (originalYPosition + drawHeight);
                if(drawHeight < 0) return false;// il gameObject non è disegnabile in quanto totalmente fuori dallo schermo
            }
            return true;
        }

        internal static void clean(GameObject gameObject, bool checkStatusChange = true) {
            if(getDrawData(gameObject, out int drawXPosition, out int drawWidth, out int spriteXPosition, out int drawYPosition, out int drawHeight, out int spriteYPosition, true)) {
                drawSpritePortion(_camera.background,
                                  drawXPosition, drawYPosition,
                                  drawXPosition, drawYPosition,
                                  drawWidth, drawHeight);
            }
        }

        private static bool statusChanged(GameObject gameObject) {
            return (gameObject.pixelPosition.x == gameObject.lastPixelPosition.x && gameObject.pixelPosition.y == gameObject.lastPixelPosition.y && gameObject.spriteSet?.sprite == gameObject.lastSprite);
        }

        private static void drawSpritePortion(Sprite sprite, int drawXPosition, int drawYPosition, int spriteXPosition, int spriteYPosition, int width, int height) {
            byte[] spriteArray = sprite.pixelArray;
            int spriteStride = sprite.stride;

            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    int bufferPixelEnd = ((drawYPosition+y) * drawBufferStride) + ((drawXPosition + x) * 3) + 2;
                    int spritePixelEnd = ((spriteYPosition+y) * spriteStride) + ((spriteXPosition + x) * 4) + 3;

                    byte alpha =  sprite.pixelArray[spritePixelEnd];
                    if(alpha != 0) {
                        //(R)
                        spritePixelEnd--;
                        drawBuffer[bufferPixelEnd] = Utility.alphaBlend(drawBuffer[bufferPixelEnd], sprite.pixelArray[spritePixelEnd], alpha);
                        //(G)
                        spritePixelEnd--; bufferPixelEnd--;
                        drawBuffer[bufferPixelEnd] = Utility.alphaBlend(drawBuffer[bufferPixelEnd], sprite.pixelArray[spritePixelEnd], alpha);
                        //(B)
                        spritePixelEnd--; bufferPixelEnd--;
                        drawBuffer[bufferPixelEnd] = Utility.alphaBlend(drawBuffer[bufferPixelEnd], sprite.pixelArray[spritePixelEnd], alpha);
                    }
                }
            }
        }

        internal static void drawSprite(Sprite sprite, int xPosition, int yPosition) {
            drawSpritePortion(sprite, xPosition, yPosition, 0, 0, sprite.width, sprite.height);
        }


        #endregion

<<<<<<< HEAD
=======
        public static Sprite loadSprite(string resource_Name, int scale = 1) {
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
            foreach(string resourceName in callerAssembly.GetManifestResourceNames()) {
                try {
                    ResourceManager rm = new ResourceManager(resourceName.Replace(".Resources.resources",".Resources"),callerAssembly);
                    bitmap = (Bitmap)rm.GetObject(resource_Name);
                    break;
                } catch { }
            }
            if(bitmap == null) throw new Exception("Can't find the sprite: " + resource_Name + " the name must be the same as the Resource's name");
            Sprite newSprite = new Sprite(Utility.scaleImage(bitmap,scale));
            sprites.Add(spriteName, newSprite);
            return newSprite;
        }

>>>>>>> 84a047f1bcbd99d313f202b4c6b43b160f16d8b1
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
        public static Sprite loadSprite(string resource_Name, int scale = 1) {
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
            foreach(string resourceName in callerAssembly.GetManifestResourceNames()) {
                try {
                    ResourceManager rm = new ResourceManager(resourceName.Replace(".Resources.resources",".Resources"),callerAssembly);
                    bitmap = (Bitmap)rm.GetObject(resource_Name);
                    break;
                } catch { }
            }
            if(bitmap == null) throw new Exception("Can't find the sprite: " + resource_Name + " the name must be the same as the Resource's name");
            Sprite newSprite = new Sprite(Utility.scaleImage(bitmap,scale));
            sprites.Add(spriteName, newSprite);
            return newSprite;
        }

        #endregion

        #region Diagnostic Methods
        private static void printFPS() {
            if(_window.focus) Console.WriteLine("FPS:" + (int)(1000f / lastCycleMS) + " MS:" + lastCycleMS + " DRAW:" + lastCycleDrawMS);
        }
        #endregion
        #endregion
    }
}
