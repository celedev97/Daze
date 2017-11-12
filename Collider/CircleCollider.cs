using System;
using Daze.Geometry;

namespace Daze {
    public class CircleCollider:Collider {
        internal protected Circle circle;

        public CircleCollider(GameObject gameObject) : base(gameObject) {}

        public override float ray => circle.radius;

        public override bool collide(Collider otherCollider) {
            throw new NotImplementedException();
        }

        public override void recreateCollider() {
            throw new NotImplementedException();
        }

        protected override void moveCollider(GameObject gameObject) {
            throw new NotImplementedException();
        }

        protected override void rotateCollider(GameObject gameObject) {
            throw new NotImplementedException();
        }
    }
}
