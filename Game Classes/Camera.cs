﻿using Daze.Vectors;
using System;
using System.Drawing;
namespace Daze {
    public class Camera : GameObject {
        internal Camera() : base(0, 0) {
            
        }
        #region Variables
        internal Sprite background;

        #region Public variables
        /// <summary>
        /// The collider of the Camera. (Except for the fact that the camera can't have a collider :P)
        /// </summary>
        public override Collider collider { get => null; }
        /// <summary>
        /// It set the camera to being fixed.
        /// A fixed camera cannot move, and the GameObjects can't go out of his limits
        /// </summary>
        public bool isFixed = false;
        /// <summary>
        /// The bounds of the camera, you can use them to check if an object is out or in the camera.
        /// You shouldn't edit them, they are updated automatically as the Camera moves.
        /// NOTE: if you need to avoid GameObjects going out of camera and your camera doesn't move just set the camera to be fixed;
        /// </summary>
        public Limits limits;
        #endregion
        #endregion

        #region Methods
        internal override void pushPixelPosition() {
            pixelPosition = new IntVector((int)position.x, (int)position.y);
            limits.minX = pixelPosition.x;
            limits.minY = pixelPosition.y;
            limits.maxX = limits.minX + Engine._drawBufferWidth;
            limits.maxY = limits.minY + Engine._drawBufferHeight;
        }

        #region Public Methods
        /// <summary>
        /// This set the background of the Camera.
        /// </summary>
        /// <param name="background"></param>
        public void setBackGround(Bitmap background) {
            this.background = new Sprite(Engine.Utility.scaleImage(background, Engine._drawBufferWidth, Engine._drawBufferHeight));
        }

        public override bool move(float xOffset, float yOffset) {
            if(isFixed) {
                throw new MethodAccessException("You cannot move the camera since you set it to fixed");
            }
            position.x += xOffset;
            position.y += yOffset;
            pushPixelPosition();
            return true;
        }
        #endregion
        #endregion
        
        /// <summary>
        /// The struct for showing camera's limits, you don't have reason to use this.
        /// </summary>
        public struct Limits {
            public float minX;
            public float minY;
            public float maxX;
            public float maxY;
        }

    }
}
