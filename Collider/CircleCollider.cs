using System;
using Daze.Geometry;

namespace Daze {
    /// <summary>
    /// A collider that use a circle as it's shape... wow, so unexpected.
    /// </summary>
    public class CircleCollider:Collider {
        /// <summary>
        /// The circle that's used to perform collision checks
        /// </summary>
        internal protected Circle circle;

        /// <summary>
        /// Create a CircleCollider
        /// </summary>
        /// <param name="gameObject">The gameObject that will update the collider coordinates when it's coordinates changes</param>
        public CircleCollider(GameObject gameObject) : base(gameObject) {}

        /// <summary>
        /// The radius of the circle
        /// </summary>
        public override float ray => circle.radius;

        /// <summary>
        /// Check if this collider collides with another one
        /// </summary>
        /// <param name="collider2">The second collider to check</param>
        /// <param name="firstTry">Send false if this method is called after a collider couldn't check the collision</param>
        /// <returns>True if they collide, false otherwise</returns>
        public override bool Collide(Collider collider2, bool firstTry = true) {
            if(collider2.GetType() == typeof(CircleCollider) || collider2.GetType().IsSubclassOf(typeof(CircleCollider))) {
                Circle circle2 = ((CircleCollider) collider2).circle;
                if(Utility.distance(circle.center, circle2.center) > (circle.radius + circle2.radius)) {
                    return false;
                }
                return true;
            } else {
                return handleNotImplementedCollision(collider2, firstTry);
            }
        }

        /// <summary>
        /// This force the coordinates recalculation for this collider
        /// </summary>
        public override void RecreateCollider() {
            circle = new Circle();
            Move();
            circle.radius = gameObject.spriteSet.size.width > gameObject.spriteSet.size.height ? gameObject.spriteSet.size.width/2 : gameObject.spriteSet.size.height/2;
        }

        /// <summary>
        /// This force the coordinates recalculation for this collider when the gameObject is moved
        /// </summary>
        /// <param name="gameObject"></param>
        protected override void Move(GameObject gameObject) {
            circle.center = gameObject.position;
        }

        /// <summary>
        /// This force the coordinates recalculation for this collider when the gameObject is rotated (since this is a circle rotating it is totally pointless, don't use this please :( )
        /// </summary>
        /// <param name="gameObject"></param>
        protected override void Rotate(GameObject gameObject) {}

        /// <summary>
        /// This method check if a point is inside the Collider
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is inside the Collider, false otherwise</returns>
        protected internal override bool InCollider(Point point) {
            return circle.contains(point);
        }
    }
}
