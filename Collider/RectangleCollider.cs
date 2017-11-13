using System;
using Daze.Geometry;

namespace Daze {
    public class RectangleCollider:ConvexPolygonCollider {
        /// <summary>
        /// The distance from the center to a vertex of the rectangle
        /// (Pratically it's half of a diagonal)
        /// </summary>
        public override float ray => polygon.ray;

        public RectangleCollider(GameObject gameObject) : base(gameObject) {}

        /// <summary>
        /// This force the recalculation of the coordinates after the gameObject moves
        /// </summary>
        /// <param name="gameObject">The gameObject that moved(it should be this collider's gameObject)</param>
        protected override void moveCollider(GameObject gameObject) {
            polygon.center = gameObject.position;
        }

        /// <summary>
        /// This recalculate every coordinate of the collider
        /// </summary>
        public override void recreateCollider() {
            polygon = new Rectangle(gameObject.spriteSet.size.width, gameObject.spriteSet.size.height, gameObject.rotation);
            polygon.center = gameObject.position;
        }

        /// <summary>
        /// This function check if a collider is colliding with this one
        /// </summary>
        /// <param name="collider2">The other collider to check</param>
        /// <returns>Return true when they collide, false otherwise</returns>
        public override bool collide(Collider collider2) {
            return base.collide(collider2);
        }

    }
}
