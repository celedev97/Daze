﻿using System;
using Daze.Geometry;

namespace Daze {
    /// <summary>
    /// A collider with the shape of a rectangle.
    /// </summary>
    public class RectangleCollider:ConvexPolygonCollider {
        /// <summary>
        /// The distance from the center to a vertex of the rectangle
        /// (Pratically it's half of a diagonal)
        /// </summary>
        public override float ray => polygon.ray;

        /// <summary>
        /// Create a RectangleCollider
        /// </summary>
        /// <param name="gameObject">The gameObject that will be used to calculate the rectangle position</param>
        public RectangleCollider(GameObject gameObject) : base(gameObject) {}

        /// <summary>
        /// This force the recalculation of the coordinates after the gameObject moves
        /// </summary>
        /// <param name="gameObject">The gameObject that moved(it should be this collider's gameObject)</param>
        protected override void Move(GameObject gameObject) {
            polygon.center = gameObject.position;
        }

        /// <summary>
        /// This recalculate every coordinate of the collider
        /// </summary>
        public override void RecreateCollider() {
            polygon = new Rectangle(gameObject.spriteSet.size.width, gameObject.spriteSet.size.height, gameObject.rotation);
            polygon.center = gameObject.position;
        }

        /// <summary>
        /// This function check if a collider is colliding with this one
        /// </summary>
        /// <param name="collider2">The other collider to check</param>
        /// <param name="firstTry">Send false if this method is called after a collider couldn't check the collision</param>
        /// <returns>Return true when they collide, false otherwise</returns>
        public override bool Collide(Collider collider2, bool firstTry = true) {
            return base.Collide(collider2, firstTry);
        }

    }
}
