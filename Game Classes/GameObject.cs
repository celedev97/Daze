using Daze.Geometry;
using Daze.Vectors;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Daze {
    /// <summary>
    /// A GameObject is a sprite with a position, it can get mouse events, it can have timers, a collider, and a lot more.
    /// </summary>
    public class GameObject:GameScript {
        #region variables
        #region variables for timers
        internal int lastSpriteTimerIndex = -1;
        
        /// <summary>
        /// The timers of this gameObject excluding the new created ones
        /// NOT RECOMMENDED: You should use the default functions related to timers to edit this
        /// </summary>
        protected internal List<Timer> timers;
        /// <summary>
        /// The timers of this gameObject created in this game cycle
        /// NOT RECOMMENDED: You should use the default functions related to timers to edit this
        /// </summary>
        protected internal List<Timer> newTimers;
        /// <summary>
        /// The timers of this gameObject to be deleted at the end of this game cycle
        /// NOT RECOMMENDED: You should use the default functions related to timers to edit this
        /// </summary>
        protected internal List<Timer> toDeleteTimers;
        #endregion

        #region variables for sprites
        internal Sprite lastSprite;
        private SpriteSet _SpriteSet;
        /// <summary>
        /// The SpriteSet for this gameObject
        /// </summary>
        public virtual SpriteSet spriteSet {
            get => _SpriteSet;
            set {
                value.reset();
                if(_SpriteSet != null) {
                    Engine.clean(this, _SpriteSet);
                }
                _SpriteSet = value;
                collider?.RecreateCollider();
                pushPixelPosition();
            }
        }

        #endregion

        #region variables for position
        /// <summary>
        /// The position of the gameObject
        /// </summary>
        public Point position;

        private float _Rotation = 0;
        /// <summary>
        /// The rotation of the gameObjects
        /// </summary>
        public float rotation {
            get => _Rotation;
            set {
                _Rotation = value - ((int)(value/360) *360);
                spriteSet?.Rotate();
                collider?.Rotate();
            }
        }
        #endregion

        #region variables for drawing
        /// <summary>
        /// The position of the gameObject on the screen in the last Cycle
        /// </summary>
        internal protected IntVector lastPixelPosition;
        /// <summary>
        /// The position of the gameObject on the screen
        /// </summary>
        internal protected IntVector pixelPosition;
        /// <summary>
        /// The position of the GameObject on the Z axis
        /// (note: the Z axis is Only used for drawing's priority)
        /// </summary>
        public int drawLayer;

        /// <summary>
        /// The size of this gameObject on the screen in the last Cycle
        /// </summary>
        internal protected Size lastSize;
        /// <summary>
        /// The minX and minY of the spriteSet before the last Cycle
        /// </summary>
        internal protected IntVector lastMinSpriteCoordinates;

        internal bool invalidated = false;

        /// <summary>
        /// This invalidate the gameObject forcing it to be redrawn, it's used only in some drawing modes
        /// </summary>
        public void invalidate() {
            invalidated = true;
        }

        #endregion


        private Collider _collider;
        /// <summary>
        /// The collider of this GameObject, colliders can have various shapes and they do adapt to the SpriteSet size
        /// </summary>
        public virtual Collider collider {
            get => _collider;
            set {
                _collider = value;
                _collider?.RecreateCollider();
            }
        }

        /// <summary>
        /// Returns the object that collided with this Object in the last movement, can be NULL
        /// </summary>
        public List<GameObject> lastCollision { get { return _lastCollision; } }

        /// <summary>
        /// This contain the GameObject that collided with this one in the last movement (NULL if there was no collision)
        /// </summary>
        protected List<GameObject> _lastCollision;
        /// <summary>
        /// The layers of colliders that this GameObjects should ignore while checking collisions
        /// </summary>
        public List<IgnoreLayer> ignoreLayers;
        #endregion

        #region Event Handlers
        /// <summary>
        /// You can add handler here to manage the click on this gameObject
        /// </summary>
        public MouseEventHandler mouseClick;
        /// <summary>
        /// You can add handler here to manage the double click on this gameObject
        /// </summary>
        public MouseEventHandler mouseDoubleClick;

        /// <summary>
        /// You can add handler here to manage the mouse movement inside this gameObject
        /// </summary>
        public MouseEventHandler mouseMove;

        /// <summary>
        /// You can add handler here to manage the mouse down event on this gameObject
        /// </summary>
        public MouseEventHandler mouseDown;
        /// <summary>
        /// You can add handler here to manage the mouse up event on this gameObject
        /// </summary>
        public MouseEventHandler mouseUp;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a GameObject
        /// </summary>
        /// <param name="x">The position of the GameObject on the X axis</param>
        /// <param name="y">The position of the GameObject on the Y axis</param>
        /// <param name="drawLayer">The priority for drawing this Object</param>
        public GameObject(float x, float y, int drawLayer) {
            timers = new List<Timer>();
            newTimers = new List<Timer>();
            toDeleteTimers = new List<Timer>();

            //i add this GameObject to the gameObjects list
            Engine.AddGameObject(this);

            //setting the position
            position.x = x;
            position.y = y;

            _lastCollision = null;
            ignoreLayers = new List<IgnoreLayer>();
            
            this.drawLayer = drawLayer;
        }

        /// <summary>
        /// Create a GameObject
        /// </summary>
        /// <param name="x">The position of the GameObject on the X axis</param>
        /// <param name="y">The position of the GameObject on the Y axis</param>
        public GameObject(float x, float y) : this(x, y, 0) { }
        #endregion

        #region Methods to use Engine timers
        /// <summary>
        /// This method create a timer that you can check in the Update method
        /// </summary>
        /// <param name="timerID">The ID of the timerw, different gameObject can have the same ID for different timer</param>
        /// <param name="msPerTick">The number of milliseconds necessary for this timer to tick</param>
        /// <param name="tickAction">The Action that will be executed when this Timer ticks</param>
        /// <param name="currentMS">The current number of milliseconds passed from the last tick</param>
        public Timer createTimer(int timerID, int msPerTick, Action tickAction, int currentMS) {
            return createTimer(timerID, msPerTick, tickAction, true, currentMS);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timerID">The ID of the timerw, different gameObject can have the same ID for different timer</param>
        /// <param name="msPerTick">The number of milliseconds necessary for this timer to tick</param>
        /// <param name="tickAction">The Action that will be executed when this Timer ticks</param>
        /// <param name="restartFlag">If this flag is set to false then the Timer will not reset automatically once it ticks</param>
        /// <param name="currentMS">The current number of milliseconds passed from the last tick</param>
        /// <returns>The created timer</returns>
        public Timer createTimer(int timerID, int msPerTick, Action tickAction, bool restartFlag = true, int currentMS = 0) {
            if(msPerTick < 1) throw new Exception("You cannot create a timer that tick every less than one second");
            //i search the timer
            Timer timerFound = null;
            foreach(Timer timer in timers) {
                if(timer.ID == timerID) {
                    timerFound = timer;
                }
            }
            //if it exists
            if(timerFound != null) {
                if(toDeleteTimers.Contains(timerFound)) {
                    //there request is to create a timer that has been deleted in this cycle.
                    //I need to remove it from the deleted list (or it will be deleted at the end of the cycle) and then set it again
                    toDeleteTimers.Remove(timerFound);
                    timerFound.msPerTick = msPerTick;
                    timerFound.tickAction = tickAction;
                    timerFound.restartFlag = restartFlag;
                    timerFound.currentMS = currentMS;
                    timerFound.Restart();
                    return timerFound;
                } else {
                    //the request is to create a timer that already exists, i cannot allow that.
                    //if someone need to set the timer he can just do it, no need to recreate it.
                    throw new Exception("A timer with the ID:" + timerID + " already exists");
                }
            } else {
                //the timer doesn't exist, so i just have to create it
                Timer timer = new Timer(timerID, msPerTick, tickAction, restartFlag, currentMS);
                newTimers.Add(timer);
                return timer;
            }
        }

        /// <summary>
        /// It retrives a Timer in this GameObject from its ID
        /// </summary>
        /// <param name="timerID">The if of the Timer to search for</param>
        /// <returns>The searched Timer, can be NULL if there is no Timer with the specified ID</returns>
        public Timer getTimer(int timerID) {
            foreach(Timer timer in newTimers) {
                if(timer.ID == timerID) {
                    return timer;
                }
            }
            foreach(Timer timer in timers) {
                if(timer.ID == timerID) {
                    return timer;
                }
            }
            return null;
        }

        /// <summary>
        /// It deletes a Timer in this GameObject from its ID
        /// </summary>
        /// <param name="timerID">The if of the Timer to delete</param>
        /// <returns>Returns true if the timer was found and deleted, false otherwise</returns>
        public bool removeTimer(int timerID) {
            int index = -1;
            for(int i = 0; i < timers.Count && index == -1; i++) {
                if(timers[i].ID == timerID) {
                    index = i;
                    toDeleteTimers.Add(timers[i]);
                }
            }
            if(index != -1) {
                return true;
            }
            return false; ;
        }

        #endregion

        #region Methods for to the Draw function
        internal void pushLastPixelPosition() {
            lastPixelPosition.set(pixelPosition);
            if(spriteSet != null) lastSize = spriteSet.size.duplicate();
            lastMinSpriteCoordinates = new IntVector(spriteSet.minX,spriteSet.minY);
        }

        internal virtual void pushPixelPosition() {
            if(spriteSet != null) {
                pixelPosition.set((int)(position.x - spriteSet.size.width / 2) - Engine.camera.pixelPosition.x, (int)(position.y - spriteSet.size.height / 2) - Engine.camera.pixelPosition.y);
            }
        }
        #endregion

        #region Methods related to the movement of the GameObject
        /// <summary>
        /// This function moves the gameObject of a certain offset
        /// </summary>
        /// <param name="offset">The vector representing the movement of the gameObject</param>
        /// <returns></returns>
        public virtual bool Move(Vector offset) {
            return Move(offset.x, offset.y);
        }


        /// <summary>
        /// This function moves the gameObject of a certain offset
        /// </summary>
        /// <param name="xOffset">The offset of the movement of the GameObject on the X axis</param>
        /// <param name="yOffset">The offset of the movement of the GameObject on the Y axis</param>
        /// <returns>This return true if the gameObject moved without collisions, false otherwise</returns>
        public virtual bool Move(float xOffset, float yOffset) {
            _lastCollision = new List<GameObject>();
            bool moved = moveX(xOffset);
            moved = moveY(yOffset) && moved;
            return moved;
        }

        private bool moveX(float xOffset, bool cancelIfCollide = true) {
            //moving the gameObject
            position.x += xOffset;
            //checking if the gameObject is limited to move within the camera
            if(Engine.camera.isFixed) {
                if((position.x + spriteSet?.size.width / 2) > Engine.camera.limits.maxX ||(position.x - spriteSet?.size.width / 2) < Engine.camera.limits.minX) {
                    //the gameObject is going out of the screen, i cancel his movement
                    position.x -= xOffset;
                    _lastCollision = new List<GameObject> { Engine.camera };
                    return false;
                }
            }
            if(collider == null) {
                return true;
            } else {
                collider.Move();
                if((_lastCollision = checkCollisions()).Count != 0) {
                    if(cancelIfCollide) extrapolate(xOffset, 0);

                    //executing this object collision
                    OnCollisionEnter();
                    invalidate();
                    
                    List<GameObject> thisList = new List<GameObject>();
                    thisList.Add(this);

                    //cycling for all the collided gameObjects to fire their collision
                    foreach(GameObject gameObject in _lastCollision) {
                        //if the other gameObject didn't get deleted by this gameObject collision then i fire his collision too
                        if(Engine.gameObjectExists(gameObject)) {
                            gameObject._lastCollision = thisList;
                            invalidate();
                            gameObject.OnCollisionEnter();
                        }
                    }

                    return false;
                }
                return true;
            }
        }
        private bool moveY(float yOffset, bool cancelIfCollide = true) {
            //moving the gameObject
            position.y += yOffset;
            //checking if the gameObject is limited to move within the camera
            if(Engine.camera.isFixed) {
                if((position.y + spriteSet?.size.height/2) > Engine.camera.limits.maxY || (position.y - spriteSet?.size.height / 2) < Engine.camera.limits.minY) {
                    //the gameObject is going out of the screen, i cancel his movement
                    position.y -= yOffset;
                    _lastCollision = new List<GameObject> { Engine.camera };
                    return false;
                }
            }
            if(collider == null) {
                return true;
            } else {
                collider.Move();
                List<GameObject> collision2;
                if((collision2 = checkCollisions()).Count != 0) {
                    if(_lastCollision == null) {
                        _lastCollision = collision2;
                    } else {
                        _lastCollision.AddRange(collision2);
                    }
                    if(cancelIfCollide) extrapolate(0, yOffset);

                    //executing this object collision
                    OnCollisionEnter();
                    invalidate();

                    List<GameObject> thisList = new List<GameObject>();
                    thisList.Add(this);

                    //cycling for all the collided gameObjects to fire their collision
                    foreach(GameObject gameObject in _lastCollision) {
                        //if the other gameObject didn't get deleted by this gameObject collision then i fire his collision too
                        if(Engine.gameObjectExists(gameObject)) {
                            gameObject._lastCollision = thisList;
                            invalidate();
                            gameObject.OnCollisionEnter();
                        }
                    }
                    return false;
                }
                return true;
            }
        }
        #endregion

        #region Methods related to collisions
        private void extrapolate(float xOffset, float yOffset) {
            position -= new Vector(xOffset, yOffset);
        }

        /// <summary>
        /// Check if this object collides with some other objects.
        /// </summary>
        /// <returns>A list of the object colliding with this one.</returns>
        protected List<GameObject> checkCollisions() {
            List<GameObject> collisions = new List<GameObject>();
            foreach(GameObject gameObject2 in Engine.findGameObjects()) {
                if(this.Collide(gameObject2)) {
                    collisions.Add(gameObject2);
                }
            }
            return collisions;
        }

        private bool Collide(GameObject gameObject2) {
            //i don't need to check if this object collide with itself or with a gameObject that doesn't have a collider
            if(gameObject2.collider == null || gameObject2 == this) return false;
            //if i'm here then the other object is different from this one, and it have a collider, so i have to check if it's one of those to be ignored
            foreach(IgnoreLayer ignoreLayer in ignoreLayers) {
                foreach(GameObject gameObjectToIgnore in ignoreLayer.gameObjects) {
                    if(gameObjectToIgnore == gameObject2) {
                        return false;
                    }
                }
                foreach(Type typeToIgnore in ignoreLayer.types) {
                    if(typeToIgnore == gameObject2.GetType()) {
                        return false;
                    }
                }
            }
            //If i'm here then it mean the object is not one of the ignored ones, so i need to check the collision
            return collider.Collide(gameObject2.collider);
        }


        #endregion

        #region Virtual Methods that should be overridden by the Game Programmer in his scripts
        /// <summary>
        /// This method is called every game cycle, here you should check your timers, input flags, process movements and so on.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// This method is called once after the Object is created and before the first Update.
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// This method is called when this object is colliding with another Object, the collision data are in the lastCollision property
        /// </summary>
        public virtual void OnCollisionEnter() { }
        #endregion

        /// <summary>
        /// Deletes this gameObject, is just a call to the Engine.DeleteGameObject function
        /// </summary>
        public void Delete() {
            Engine.DeleteGameObject(this);
        }
    }

}
