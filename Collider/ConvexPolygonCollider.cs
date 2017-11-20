using System;
using Daze.Geometry;

namespace Daze {
    /// <summary>
    /// A general collider that can have different shapes as long as it's shape is convex
    /// </summary>
    public abstract class ConvexPolygonCollider : Collider {
        /// <summary>
        /// The polygon used to check collisions
        /// </summary>
        internal protected ConvexPolygon polygon;

        /// <summary>
        /// Create a ConvexPolygonCollider
        /// </summary>
        /// <param name="gameObject">The gameObject that will be used to calculate the polygon position</param>
        protected ConvexPolygonCollider(GameObject gameObject) : base(gameObject) { }

        /// <summary>
        /// This check if this collider is colliding with another one
        /// </summary>
        /// <param name="collider2">The second collider to check</param>
        /// <returns>True if the two collider collide, false otherwise</returns>
        public override bool Collide(Collider collider2) {
            
            Type collider2Type = collider2.GetType();
            if(collider2Type.IsSubclassOf(typeof(ConvexPolygonCollider))) {
                ConvexPolygon poly1 = polygon;
                ConvexPolygon poly2 = ((ConvexPolygonCollider) collider2).polygon;

                float centerDistance = Utility.distance(poly1.center, poly2.center);
                float raySum = (poly1.ray + poly2.ray);

                //se la distanza dei centri è maggiore della somma dei raggi delle circonferenze circoscritte ai poligoni allora non possono collidere e non ho bisogno di ricorrere a nessun controllo
                if(centerDistance > raySum) {
                    return false;
                } else {
                    //se la distanza è più piccola invece è possibile che siano uno dentro l'altro,
                    //quindi invece di iniziare a provare le collisioni con tutti gli edge, sprecare risorse e poi avere come risultato che non si toccano
                    //mi tolgo subito il dubbio di se uno è dentro l'altro
                    if(poly1.contains(poly2.center) || poly2.contains(poly1.center)) {
                        return true;
                    }
                }

                //arrivato a questo punto so che i collider non sono troppo lontani, ma neanche troppo vicini,
                //sono costretto a controllare le intersezioni tra ogni singolo edge dei collider

                //faccio uno swap in modo tale da fare il ciclo sul poligono che ha più lati, il risultato è indifferente
                //ma ho la sensazione che così io abbia più probabilità di trovare collisione in meno cicli.
                //in ogni caso non perdo praticamente niente in performance a fare un semplice swap tra variabili,
                //quindi se anche la mia impressione fosse sbagliata non ci perderei più di tanto.
                if(poly1.lines.Length < poly2.lines.Length) {
                    ConvexPolygon temp = poly1;
                    poly1 = poly2;
                    poly2 = temp;
                }

                //in caso in cui l'algoritmo per lati sia troppo lento considera di passare al SAT
                //questo algoritmo è più veloce se collidono, il SAT funziona al contrario, è più veloce se non collidono
                //considerando che se sono lontani questo pezzo non viene eseguito comunque direi che questo dovrebbe essere meglio del SAT
                // (è più lento solo se uno è completamente dentro l'altro)
                //TODO: hai controllato la correttezza delle linee, sembrerebbe sensata, quindi è il linesIntersect a non funzionare
                foreach(Line line in poly1.lines) {
                    foreach(Line line2 in poly2.lines) {
                        if(Utility.linesIntersect(line, line2))
                            return true;
                    }
                }
                return false;
            } else if(collider2Type == typeof(CircleCollider)){
                Circle circle = ((CircleCollider)collider2).circle;

                //controllo se il rettangolo è nel cerchio o il contrario
                if(circle.contains(polygon.center)) {
                    return true;
                }
                if(polygon.contains(circle.center)) {
                    return true;
                }

                //non sono uno nell'altro, quindi mi tocca controllare se ci sono intersezioni
                foreach(Line line in polygon.lines) {
                    if(Utility.linesIntersect(line, circle)) return true;
                }
            } else {
                throw new NotImplementedException();
            }
            return false;
        }

        /// <summary>
        /// This forces the collider's coordinates recalculation after that the gameObject rotates
        /// </summary>
        /// <param name="gameObject">The gameObject that will be used to check rotation and calculate the new coordinates</param>
        protected override void Rotate(GameObject gameObject) {
            polygon.rotation = gameObject.rotation;
        }

        /// <summary>
        /// This update the collider's coordinates after that the gameObject moves
        /// </summary>
        /// <param name="gameObject">The gameobject that will be used to recalculate the coordinates, it should be this collider's gameobject</param>
        protected override void Move(GameObject gameObject) {
            polygon.center = gameObject.position;
        }

        /// <summary>
        /// This method check if a point is inside the Collider
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is inside the Collider, false otherwise</returns>
        protected internal override bool InCollider(Point point) {
            return polygon.contains(point);
        }

    }
}
