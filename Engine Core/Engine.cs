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
    public unsafe class Engine {

        public static class Camera{
            public static readonly GameObject gameObject = new GameObject(0,0);
            public static void setBackGround(Bitmap background) {
                Camera.background = new Sprite(Engine.Utility.scaleImage(background, Engine.drawBufferWidth, Engine.drawBufferHeight));
            }
            internal static Sprite background;
        }

        public static Action lostFocus;
        public static Action gotFocus;

        //TODO: SETTA IL FLAG A FALSE SE GLI FPS SONO STABILI (A GIOCO COMPLETATO)
        public static bool PRINT_FPS = true;

        #region Engine external settings/data
        /// <summary>
        /// The milliseconds that the last game cycle took
        /// This can be used to do dynamical physics calculation.
        /// Thinking about it....
        /// Who the hell would implement physics in a Game Engine that use characters to draw?
        /// </summary>
        public static float deltaTime { get { return hiddenDeltaTime; } }



        private static float hiddenDeltaTime;
        private static float lastCycleMS = 0;
        #endregion

        #region Hidden engine constants/variables
        internal static GameForm window;

        internal static List<GameScript> gameScripts;

        private static List<GameObject> gameObjects;
        private static List<GameObject> newGameObjects;
        private static List<GameObject> toDeleteGameObjects;

        private static int timeSpan;

        internal static Thread gameCycleThread;
        internal static bool runGameCycle;

        internal static Dictionary<string,Sprite> sprites;
        #endregion

        #region Drawing buffer and game frame
        internal static byte[] drawBuffer;

        internal static int drawBufferHeight;
        internal static int drawBufferWidth;

        internal static int drawBufferStride;

        #endregion

        public static void Start(int FPSLimit = 60) {
            #region Initial Setup
            //calcolo il timespan di un Update necessario per non superare il limite di troppo il limite di FPS
            FPSLimit++;
            timeSpan = (timeSpan = 1000 / FPSLimit) > 0 ? timeSpan : 1;

            //inizializzo l'utility per la generazione di numeri casuali
            Utility.random = new Random();

            #region Finestra e buffer
            //creo la finestra del gioco
            window = new GameForm();

            //inizializzo il doppio buffer
            drawBufferWidth = Screen.PrimaryScreen.Bounds.Width;
            drawBufferHeight = Screen.PrimaryScreen.Bounds.Height;

            window.buffer = new Bitmap(drawBufferWidth, drawBufferHeight);
            using(Graphics g = Graphics.FromImage(window.buffer)) {
                g.DrawRectangle(new Pen(Color.Blue), 0, 0, drawBufferWidth, drawBufferHeight);
            }
            
            drawBuffer = new byte[drawBufferWidth * drawBufferHeight * 3];
            drawBufferStride = drawBufferWidth * 3;
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

            //avvio del ciclo di gioco
            runGameCycle = true;
            gameCycleThread = new Thread(new ThreadStart(GameCycle));
            gameCycleThread.SetApartmentState(ApartmentState.STA);
            gameCycleThread.Start();
            
            if(PRINT_FPS) {
                Thread t = new Thread(new ThreadStart(printFPS));
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }

            //avvio la finestra del gioco (avviarla prima avrebbe bloccato l'esecuzione del codice)
            Application.Run(window);
            #endregion
        }

        private static void printFPS() {
            while(true) {
                if(window.focus) {
                    Console.WriteLine("FPS:"+(int)(1000f / lastCycleMS) + " MS:" + lastCycleMS);
                }
                Thread.Sleep(1000);
            }
        }

        private static void GameCycle() {
            while(!window.loaded) {
                Thread.Sleep(100);
            }
            //creo lo stopwatch per il ciclo di gioco
            Stopwatch stopwatch = new Stopwatch();

            //disegno lo sfondo
            drawSprite(Camera.background, 0, 0);

            while(runGameCycle) {
                //reinizializzo lo stopwatch per misurare questo ciclo
                stopwatch.Restart();

                #region Aggiorno i componenti di gioco
                #region GameScripts
                foreach(GameScript gameScript in gameScripts) {
                    gameScript.Update();
                }
                #endregion

                #region GameObjects scripts
                //esecuzione degli script collegati a un gameObject
                foreach(GameObject gameObject in gameObjects) {
                    //mi segno la posizione corrente dell'oggetto come posizione preUpdate così da poterla poi pulire se necessario
                    gameObject.pushLastPixelPosition();

                    //aggiorno i millisecondi dei timer dell'oggetto prima di eseguire l'Update
                    foreach(Timer timer in gameObject.timers) {
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
                        clean(gameObject, false);
                        gameObject.timers.Remove(gameObject.toDeleteTimers[i]);
                        gameObject.toDeleteTimers.RemoveAt(i);
                    }
                    gameObject.Update();

                    //aggiorno la posizione corrente sullo schermo
                    gameObject.pushPixelPosition();
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
                    gameObjects.Remove(toDeleteGameObjects[i]);
                    toDeleteGameObjects.RemoveAt(i);
                }
                #endregion
                #endregion

                #region Aggiorno lo schermo
                //ridisegno i componenti modificati dal ciclo di gioco dei vari script
                //CICLO DI DRAW PRECEDENTE: //DEBUGDATA: 120MS in media, 150 massimo + //DEBUGDATA: <1MS CON UN SOLO GAMEOBJECT DI PICCOLE DIMENSIONI
                Draw();

                //aggiorno l'immagine sullo schermo
                window.updateImage();//DEBUGDATA: 30ms media
                #endregion

                //aspetto il numero di MS necessari per arrivare al timestep oppure aspetto 0MS se l'esecuzione dell'update ne ha richiesti più di 100
                if(timeSpan > 0) {
                    int temp = timeSpan - (int)stopwatch.ElapsedMilliseconds;
                    if(temp > 0) Thread.Sleep(temp);
                }
                //imposto il deltaTime per le simulazioni fisiche
                lastCycleMS = (float)stopwatch.Elapsed.TotalMilliseconds;
                hiddenDeltaTime = lastCycleMS / 1000;
            }
        }

        #region Hidden engine methods for drawing
        internal static void Draw() {
            sortGameObjectByZ();

            foreach(GameObject gameObject in gameObjects) {
                if(gameObject.spriteSet == null) continue;
                clean(gameObject);
                drawSprite(gameObject);
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
        //questo metodo disegna solo la parte con alfa != 0
        private static void drawSprite(GameObject gameObject) {
            //AGGIUNGI CALCOLO PER CONTROLLARE SE È FUORI DALLA CAMERA
            drawSpritePortion(gameObject.spriteSet.sprite,
                gameObject.pixelPosition.x, gameObject.pixelPosition.y,
                gameObject.spriteSet.minX, gameObject.spriteSet.minY,
                gameObject.spriteSet.size.width, gameObject.spriteSet.size.height);
        }


        internal static void clean(GameObject gameObject, bool checkPosition = true) {
            if(checkPosition) {
                if(gameObject.pixelPosition.x == gameObject.lastPixelPosition.x && gameObject.pixelPosition.y == gameObject.lastPixelPosition.y) return;
            }
            drawSpritePortion(Camera.background, gameObject.lastPixelPosition.x, gameObject.lastPixelPosition.y, gameObject.lastPixelPosition.x, gameObject.lastPixelPosition.y, gameObject.spriteSet.sprite.width, gameObject.spriteSet.sprite.height);
        }

        internal static void drawSpritePortion(Sprite sprite, int drawXPosition, int drawYPosition, int spriteXPosition, int spriteYPosition, int width, int height) {
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

        public static Sprite loadSprite(string resource_Name, int scale = 1) {
            string spriteName = resource_Name+"x"+scale;
            //cerco se lo sprite esiste già
            foreach(KeyValuePair<string, Sprite> keyVal in sprites){
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
            if(bitmap == null) throw new Exception("Can't find the sprite: "+ resource_Name + " the name must be the same as the Resource's name");
            Sprite newSprite = new Sprite(Utility.scaleImage(bitmap,scale));
            sprites.Add(spriteName, newSprite);
            return newSprite;
        }

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
            newGameObjects.Add(gameObject);
        }

        /// <summary>
        /// Delete a gameObject from the gameObject list
        /// </summary>
        /// <param name="gameObject">The gameObject to remove</param>
        public static void DeleteGameObject(GameObject gameObject) {
            toDeleteGameObjects.Add(gameObject);
        }

        internal static void stopGameCycle() {
            runGameCycle = false;
        }

        #endregion
        #endregion
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
            /// Wait... you really need me to explain this... ?
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

            internal static string[] StringToDesignArray(string designString) {
                return designString.Replace("\r\n", "\n").Split('\n'); ;
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

            internal static Bitmap scaleImage(Bitmap original, int scale) {
                if(scale < 2) return original;
                return scaleImage(original, original.Width * scale, original.Height * scale);
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
