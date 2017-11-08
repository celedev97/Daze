using System;
using Daze.Geometry;

namespace Daze {
    public abstract class ConvexPolygonCollider : Collider {
        internal protected ConvexPolygon polygon;

        protected ConvexPolygonCollider(GameObject gameObject) : base(gameObject) { }

        public override bool collide(Collider collider2) {
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

        protected override void rotateCollider(GameObject gameObject) {
            polygon.rotation = gameObject.rotation;
        }

        protected override void moveCollider(GameObject gameObject) {
            polygon.center = gameObject.position;
        }

    }
}
