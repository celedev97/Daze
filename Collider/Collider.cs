using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daze {
    public abstract class Collider {
        protected GameObject gameObject;

        public Collider(GameObject gameObject) {
            this.gameObject = gameObject;
            recreateCollider();
        }

        public abstract float ray { get; }

        public abstract bool collide(Collider otherCollider);

        public abstract void recreateCollider();

        public virtual void moveCollider() => moveCollider(gameObject);
        protected abstract void moveCollider(GameObject gameObject);

        public virtual void rotateCollider() => rotateCollider(gameObject);
        protected abstract void rotateCollider(GameObject gameObject);
    }
}
