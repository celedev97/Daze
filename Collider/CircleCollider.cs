using System;
using Daze.Geometry;

namespace Daze {
    public class CircleCollider:Collider {
        internal protected Circle circle;

        public CircleCollider(GameObject gameObject) : base(gameObject) {}

        /// <summary>
        /// The radius of the circle
        /// </summary>
        public override float ray => circle.radius;

        /// <summary>
        /// Check if this collider collides with another one
        /// </summary>
        /// <param name="otherCollider">The second collider to check</param>
        /// <returns>True if they collide, false otherwise</returns>
        public override bool collide(Collider otherCollider) {
            if(otherCollider.GetType().IsSubclassOf(typeof(CircleCollider))) {
                Circle circle2 = ((CircleCollider) otherCollider).circle;
                if(Utility.distance(circle.center, circle2.center)>(circle.radius + circle2.radius)){
                    return false;
                }
                return true;
            } else if(otherCollider.GetType().IsSubclassOf(typeof(ConvexPolygonCollider))) {
                return otherCollider.collide(this);
            }
            throw new NotImplementedException("This collider has no idea how to check the collision with the other one");
        }

        /// <summary>
        /// This force the coordinates recalculation for this collider
        /// </summary>
        public override void recreateCollider() {
            circle = new Circle();
            moveCollider();
            circle.radius = gameObject.spriteSet.sprite.width > gameObject.spriteSet.sprite.height ? gameObject.spriteSet.sprite.width : gameObject.spriteSet.sprite.height;
        }

        /// <summary>
        /// This force the coordinates recalculation for this collider when the gameObject is moved
        /// </summary>
        /// <param name="gameObject"></param>
        protected override void moveCollider(GameObject gameObject) {
            circle.center = gameObject.position;
        }

        /// <summary>
        /// This force the coordinates recalculation for this collider when the gameObject is rotated
        /// </summary>
        /// <param name="gameObject"></param>
        protected override void rotateCollider(GameObject gameObject) {}

    }
}
