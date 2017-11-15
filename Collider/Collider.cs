using Daze.Geometry;
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
            recreateCollider();
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
        public abstract bool collide(Collider otherCollider);

        /// <summary>
        /// It forces the collider's coordinates recalculation
        /// </summary>
        public abstract void recreateCollider();

        /// <summary>
        /// This move the collider, it will be called automatically when his gameObject is moved
        /// </summary>
        public virtual void moveCollider() => moveCollider(gameObject);
        /// <summary>
        /// This move the collider, it will be called automatically when his gameObject is moved
        /// </summary>
        protected abstract void moveCollider(GameObject gameObject);

        /// <summary>
        /// It forces the collider's coordinates recalculation after a gameObject rotation
        /// </summary>
        public virtual void rotateCollider() => rotateCollider(gameObject);
        /// <summary>
        /// It forces the collider's coordinates recalculation after a gameObject rotation
        /// </summary>
        protected abstract void rotateCollider(GameObject gameObject);
        /// <summary>
        /// This method check if a point is inside the Collider
        /// </summary>
        /// <param name="x">The x of the point to check</param>
        /// <param name="y">The y of the point to check</param>
        /// <returns>True if the point is inside the Collider, false otherwise</returns>
        internal protected abstract bool inCollider(Point point);
        /// <summary>
        /// This method check if a point is inside the Collider
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is inside the Collider, false otherwise</returns>
        internal protected virtual bool inCollider(float x, float y) { return inCollider(new Point(x, y)); }
    }
}
