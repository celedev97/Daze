namespace Daze.Geometry {
    public abstract class ConvexPolygon {
        public Line[] lines;
        public abstract Point center { get; set; }
        public abstract float rotation { get; set; }
        public float ray;

        public bool contains(Point point) {
            //creo il segmento che passa per i due centri
            Line centersSegment = new Line(point, center);

            //se il segmento colpisce almeno uno dei bordi allora il punto è fuori dal poligono.
            //quest'approccio rende il metodo più veloce se il punto non è all'interno del poligono.
            //avrei potuto fare il contrario usando una linea più lunga e contando il numero di intersezioni,
            //ma considerando che in un gioco non dovrebbero esserci troppe compenetrazioni questo mi pare un approccio adeguato.
            foreach(Line edge in lines) {
                if(Utility.linesIntersect(centersSegment, edge)) {
                    return false;
                }
            }

            return true;
        }
    }

    public class Rectangle:ConvexPolygon {
        #region Properties and variables
        private float _Width; public float width { get => _Width; }
        private float _Height; public float height { get => _Height; }

        private Point _Center;
        public override Point center {
            get => _Center;
            set {
                A = RA.duplicate();
                B = RB.duplicate();
                C = RC.duplicate();
                D = RD.duplicate();

                A += value;
                B += value;
                C += value;
                D += value;

                _Center = value;

                calculateLines();
            }
        }

        private float _Rotation;
        public override float rotation {
            get => _Rotation;
            set {
                _Rotation = value;
                rotate();
                calculateLines();
            }
        }

        /* A B
           C D */
        private Point A0,B0,C0,D0;//punti senza offset ne rotazione
        private Point RA,RB,RC,RD;//punti senza offset ma rotati
        public Point A,B,C,D;//punti con offset

        #endregion

        public Rectangle(float width, float height, float rotation) : this(width, height) {
            this.rotation = rotation;
        }

        public Rectangle(float width, float height) {
            _Width = width;
            _Height = height;

            float halfWidth = width/2;
            float halfHeight = height/2;

            A0.x = -halfWidth;
            B0.x = halfWidth;
            C0.x = -halfWidth;
            D0.x = halfWidth;

            //queste coordinate funzionano su un sistema cartesiano regolare, non su un sistema con Y inversa come lo schermo
            A0.y = halfHeight;
            B0.y = halfHeight;
            C0.y = -halfHeight;
            D0.y = -halfHeight;

            ray = Utility.distance(Point.O, A0);


            resetRotatedPoints();
            lines = new Line[4];
            calculateLines();
        }

        private void resetRotatedPoints() {
            RA = A0.duplicate();
            RB = B0.duplicate();
            RC = C0.duplicate();
            RD = D0.duplicate();
        }

        private void rotate() {
            resetRotatedPoints();

            RA.rotatePointAroundO(_Rotation);
            RB.rotatePointAroundO(_Rotation);
            RC.rotatePointAroundO(_Rotation);
            RD.rotatePointAroundO(_Rotation);
        }

        private void calculateLines() {
            lines[0] = new Line(A, B);
            lines[1] = new Line(B, D);
            lines[2] = new Line(D, C);
            lines[3] = new Line(C, A);
        }

    }

    public struct Circle {
        public Point center;
        public float radius;

        internal bool contains(Point point) {
            if(Utility.distance(center, point)>radius) {
                return false;
            }
            return true;
        }
    }
}
