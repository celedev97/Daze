﻿using Daze.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daze {
    /// <summary>
    /// This class represent a collider
    /// </summary>
    public abstract class Collider {
        /// <summary>
        /// The gameObject of this collider
        /// </summary>
        protected GameObject gameObject;

        /// <summary>
        /// The gameObject of this collider
        /// </summary>
        /// <param name="gameObject"></param>
        public Collider(GameObject gameObject) {
            this.gameObject = gameObject;
            RecreateCollider();
        }

        /// <summary>
        /// The maximum distance from the center and a vertex of this collider
        /// </summary>
        public abstract float ray { get; }

        /// <summary>
        /// A method to check if a collider is colliding with this one
        /// </summary>
        /// <param name="otherCollider">The second collider to check</param>
        /// <returns>True if they collide, false otherwise</returns>
        public abstract bool Collide(Collider otherCollider);

        /// <summary>
        /// It forces the collider's coordinates recalculation
        /// </summary>
        public abstract void RecreateCollider();

        /// <summary>
        /// This move the collider, it will be called automatically when his gameObject is moved
        /// </summary>
        public virtual void Move() => Move(gameObject);
        /// <summary>
        /// This move the collider, it will be called automatically when his gameObject is moved
        /// </summary>
        protected abstract void Move(GameObject gameObject);

        /// <summary>
        /// It forces the collider's coordinates recalculation after a gameObject rotation
        /// </summary>
        public virtual void Rotate() => Rotate(gameObject);
        /// <summary>
        /// It forces the collider's coordinates recalculation after a gameObject rotation
        /// </summary>
        protected abstract void Rotate(GameObject gameObject);
        /// <summary>
        /// This method check if a point is inside the Collider
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is inside the Collider, false otherwise</returns>
        internal protected abstract bool InCollider(Point point);
        /// <summary>
        /// This method check if a point is inside the Collider
        /// </summary>
        /// <param name="x">The x coordinate of the point to check</param>
        /// <param name="y">The y coordinate of the point to check</param>
        /// <returns>True if the point is inside the Collider, false otherwise</returns>
        internal protected virtual bool InCollider(float x, float y) { return InCollider(new Point(x, y)); }
    }
}
