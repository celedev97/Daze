using System;
using Daze.Geometry;

namespace Daze {
    public class RectangleCollider:ConvexPolygonCollider {
        public override float ray => polygon.ray;

        public RectangleCollider(GameObject gameObject) : base(gameObject) {}

        protected override void moveCollider(GameObject gameObject) {
            polygon.center = gameObject.position;
        }

        public override void recreateCollider() {
            polygon = new Rectangle(gameObject.spriteSet.size.width, gameObject.spriteSet.size.height, gameObject.rotation);
            polygon.center = gameObject.position;
        }

        public override bool collide(Collider collider2) {
            return base.collide(collider2);
        }

    }
}
