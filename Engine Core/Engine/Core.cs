using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;

namespace Daze {
    /// <summary>
    /// This is the core class of Daze, call Engine.Start() to start your game
    /// </summary>
    public static partial class Engine {


        #region Variables
        #region Engine Settings
        private static Camera _camera;
        /// <summary>
        /// This is the camera that is showing on the screen (Daze currently support only one camera)
        /// </summary>
        public static Camera camera { get => _camera; }

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

        #region Debug variables
        private static float beforeBlit = 0, blitMS = 0, beforeCompositing = 0, compositingMS = 0, beforeUpdate, updateMS = 0;
        #endregion

        private static float lastCycleMS = 0;

        private static int targetCycleMS;
        #endregion
        #endregion

        #region Functions
        #region Start/Stop functions
        /// <summary>
        /// This function start the Engine
        /// </summary>
        /// <param name="FPSLimit">The maximum FPS that the Engine should reach, don't specify it if you don't need it</param>
        /// <param name="renderingSize">The internal rendering size</param>
        /// <param name="drawingMethod">The Engine drawing method, see Engine.DrawingMethod to get more info</param>
        /// <param name="drawDestination">The draw destination, set to null to use Daze default window</param>
        public static void Start(int FPSLimit = 60, RenderingSize renderingSize = RenderingSize.SIZE_1280X720, DrawingMethod drawingMethod = DrawingMethod.REDRAW_GAMEOBJECTS, IDrawable drawDestination = null) {
            #region Initial Setup
            //draw setup
            _drawDestination = drawDestination;
            Engine.drawingMethod = drawingMethod;

            //calculating timespan necessary to not overcome FPS limit too much
            targetCycleMS = (targetCycleMS = 1000 / FPSLimit + 1) > 0 ? targetCycleMS : 1;

            //initializing utility for random numbers
            Utility.random = new Random();
            if(_drawDestination == null) {
                _drawDestination = new DrawOnWinform();
            }
            #region Window and buffer
            //creating draw window
            _drawDestination.IntialSetup();

            //starting Daze buffer
            string sizeString = renderingSize.ToString();
            int startNumber = sizeString.IndexOf("_")+1;
            int xPosition = sizeString.IndexOf("X");
            _drawBufferWidth = int.Parse(sizeString.Substring(startNumber, xPosition - startNumber));
            _drawBufferHeight = int.Parse(sizeString.Substring(xPosition + 1));
            _drawBuffer = new byte[_drawBufferWidth * _drawBufferHeight * 3];
            _drawBufferStride = _drawBufferWidth * 3;

            //set the window according to Daze buffer
            _drawDestination.BufferSetup();
            #endregion

            _camera = new Camera();

            #region Events
            mouseClicksList = new List<MouseEvent>();
            mouseDoubleClicksList = new List<MouseEvent>();
            mouseMovesList = new List<MouseEvent>();
            mouseDownsList = new List<MouseEvent>();
            mouseUpsList = new List<MouseEvent>();

            mouseClick += mouseClickedAdd;
            mouseDoubleClick += mouseDoubleClickedAdd;

            mouseMove += mouseMovedAdd;

            mouseDown += mouseButtonDownAdd;
            mouseUp += mouseButtonUpAdd;
            #endregion

            #region Lists initialization
            //initializing preloading lists
            sprites = new Dictionary<string, Sprite>();
            wavPaths = new Dictionary<string, string>();

            //initializing sounds lists
            playingWavs = new List<Wav>();

            //initializing gameobjects and scripts lists
            gameObjects = new List<GameObject>();
            newGameObjects = new List<GameObject>();
            toDeleteGameObjects = new List<GameObject>();

            gameScripts = new List<GameScript>();

            //i search for the gameScripts by using reflection, add them to the list, and start them
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach(Type type in assembly.GetTypes()) {
                    foreach(Type interfaceType in type.GetInterfaces()) {
                        if(interfaceType == typeof(GameScript) && type != typeof(GameScript) && !type.IsSubclassOf(typeof(GameObject)) && type != typeof(GameObject)) {
                            //this class implements GameScript (but it's not GameScript itself) and it's not a GameObject, so this is a GameScript
                            GameScript script = (GameScript)Activator.CreateInstance(type);
                            script.Start();
                            gameScripts.Add(script);
                        }
                    }
                }
            }
            #endregion

            //i start the draw window
            _drawDestination.Start();

            #region Wait till the window is loaded
            while(!_drawDestination.loaded) {
                Thread.Sleep(100);
                Application.DoEvents();
            }
            #endregion

            //starting game cycle
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
            //i create the stopwatch for managing the game cycle
            Stopwatch stopwatch = new Stopwatch();

            //drawing the background
            drawBackground();
            #endregion

            while(!stopCycle) {
                //starting the stopwatch for managing the game cycle
                stopwatch.Restart();

                if(printFpsFlag) beforeUpdate = (float)stopwatch.Elapsed.TotalMilliseconds;
                #region Update
                #region Saving last draw status before all the scripts
                foreach(GameObject gameObject in gameObjects) {
                    //i save the position of the object on the screen, so that if it's needed i can later clean it
                    gameObject.pushLastPixelPosition();
                    gameObject.lastSprite = gameObject.spriteSet?.sprite;
                }
                #endregion

                #region Executing events
                for(int i = mouseClicksList.Count - 1; i >= 0; i--) {
                    mouseClickExecute(mouseClicksList[i].sender, mouseClicksList[i].e);
                    mouseClicksList.RemoveAt(i);
                }

                for(int i = mouseDoubleClicksList.Count - 1; i >= 0; i--) {
                    mouseDoubleClickExecute(mouseDoubleClicksList[i].sender, mouseDoubleClicksList[i].e);
                    mouseDoubleClicksList.RemoveAt(i);
                }

                for(int i = mouseMovesList.Count - 1; i >= 0; i--) {
                    mouseMoveExecute(mouseMovesList[i].sender, mouseMovesList[i].e);
                    mouseMovesList.RemoveAt(i);
                }

                for(int i = mouseDownsList.Count - 1; i >= 0; i--) {
                    mouseDownExecute(mouseDownsList[i].sender, mouseDownsList[i].e);
                    mouseDownsList.RemoveAt(i);
                }

                for(int i = mouseClicksList.Count - 1; i >= 0; i--) {
                    mouseUpExecute(mouseUpsList[i].sender, mouseUpsList[i].e);
                    mouseUpsList.RemoveAt(i);
                }
                #endregion

                #region Executing scripts
                #region GameScripts
                foreach(GameScript gameScript in gameScripts) {
                    gameScript.Update();
                }
                #endregion

                #region GameObjects scripts
                //executing scripts of gameObjects
                foreach(GameObject gameObject in gameObjects) {
                    #region Timers
                    //before executing the Update i update the timers
                    foreach(Timer timer in gameObject.timers) {
                        //i skip this timer if it was removed
                        if(gameObject.toDeleteTimers.Contains(timer)) continue;
                        //i skip this timer if it's a spriteset timer and it's not active
                        if(timer.ID < 0 && timer.ID != gameObject.spriteSet?.timerID) continue;
                        if(timer.currentMS < timer.msPerTick) timer.currentMS += lastCycleMS;
                        if(timer.ticked()) {
                            timer.tickAction?.Invoke();
                        }
                    }
                    #endregion
                    gameObject.Update();
                }

                #endregion
                #endregion

                #region Updating lists
                #region GameObject lists editing
                //adding gameObjects created right now to the list of gameObjects
                for(int i = newGameObjects.Count - 1; i >= 0; i--) {
                    newGameObjects[i].Start();
                    gameObjects.Add(newGameObjects[i]);
                    newGameObjects.RemoveAt(i);
                }
                //removing gameObjects deleted right now from the list of gameObjects
                for(int i = toDeleteGameObjects.Count - 1; i >= 0; i--) {
                    if(toDeleteGameObjects[i]?.spriteSet != null) clean(toDeleteGameObjects[i]);
                    gameObjects.Remove(toDeleteGameObjects[i]);
                    toDeleteGameObjects.RemoveAt(i);
                }
                #endregion

                #region Timer lists editing
                foreach(GameObject gameObject in gameObjects) {
                    //adding timers created right now to the list of gameObject timers
                    for(int i = gameObject.newTimers.Count - 1; i >= 0; i--) {
                        gameObject.timers.Add(gameObject.newTimers[i]);
                        gameObject.newTimers.RemoveAt(i);
                    }
                    //removing timers deleted right now from the list of gameObject timers 
                    for(int i = gameObject.toDeleteTimers.Count - 1; i >= 0; i--) {
                        gameObject.timers.Remove(gameObject.toDeleteTimers[i]);
                        gameObject.toDeleteTimers.RemoveAt(i);
                    }
                }
                #endregion
                #endregion

                #region Updating new pixel positions
                _camera.pushPixelPosition();
                foreach(GameObject gameObject in gameObjects) {
                    gameObject.pushPixelPosition();
                }
                #endregion
                #endregion
                if(printFpsFlag) updateMS = (float)stopwatch.Elapsed.TotalMilliseconds - beforeUpdate;

                if(printFpsFlag) beforeCompositing = (float)stopwatch.Elapsed.TotalMilliseconds;
                #region Draw to buffer
                //sorting game objects by draw priority
                sortGameObjectByZ();
                
                //updating the buffer
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
                        foreach(GameObject gameObject in gameObjects) {
                            gameObject.invalidated = false;
                        }
                        break;
                }
                
                #endregion
                if(printFpsFlag) compositingMS = (float)stopwatch.Elapsed.TotalMilliseconds - beforeCompositing;

                #region Bit blit
                if(printFpsFlag) beforeBlit = (float)stopwatch.Elapsed.TotalMilliseconds;
                //updating the screen
                _drawDestination.Draw();
                if(printFpsFlag) blitMS = (float)stopwatch.Elapsed.TotalMilliseconds - beforeBlit;
                #endregion

                #region Time check
                //i wait for the number of MS necessary to reach the timestep, or i don't wait if the time already exceed the timestep
                if(targetCycleMS > 0) {
                    int temp = targetCycleMS - (int)stopwatch.Elapsed.TotalMilliseconds;
                    if(temp > 0) Thread.Sleep(temp);
                }

                //now after the sleep i can calculate this cycle MS to calculate deltaTime and FPS
                lastCycleMS = (float)stopwatch.Elapsed.TotalMilliseconds;
                _deltaTime = lastCycleMS / 1000;
                if(printFpsFlag) printFPS();
                #endregion
            }
        }

        #region Diagnostic Methods
        private static void printFPS() {
            if(_drawDestination.focus) Console.WriteLine("FPS:" + (int)(1000f / lastCycleMS) + " MS:" + lastCycleMS + " U:" + updateMS + " C:" + compositingMS + " B:" + blitMS);
        }
        #endregion
        #endregion
    }
}
