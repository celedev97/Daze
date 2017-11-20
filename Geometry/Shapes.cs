namespace Daze.Geometry {
    /// <summary>
    /// An abstract class that is the base for creating convex polygons for colliders.
    /// </summary>
    public abstract class ConvexPolygon {
        /// <summary>
        /// The edges of this polygon
        /// </summary>
        public Line[] lines;
        /// <summary>
        /// The center of this polygon
        /// </summary>
        public abstract Point center { get; set; }
        /// <summary>
        /// The rotation of this polygon
        /// </summary>
        public abstract float rotation { get; set; }
        /// <summary>
        /// The ray of the polygon is the distance from the center and a vertex
        /// </summary>
        public float ray;

        /// <summary>
        /// Check if this polygon contains the specified point
        /// </summary>
        /// <param name="point">the point to check</param>
        /// <returns>return true if the point is inside the polygon, false otherwise</returns>
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

    /// <summary>
    /// A rectangle. Yes, it's just a rectangle.
    /// </summary>
    public class Rectangle:ConvexPolygon {
        #region Properties and variables
        private float _Width;
        /// <summary>
        /// The width of the rectangle
        /// </summary>
        public float width { get => _Width; }
        private float _Height;
        /// <summary>
        /// The height of the rectangle
        /// </summary>
        public float height { get => _Height; }

        private Point _Center;
        /// <summary>
        /// The center of the rectangle
        /// </summary>
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
        /// <summary>
        /// The rotation of the rectangle
        /// </summary>
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
        /// <summary>
        /// Vertex of the rectangle
        /// </summary>
        public Point A,B,C,D;//punti con offset

        #endregion

        /// <summary>
        /// Create a rectangle
        /// </summary>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        /// <param name="rotation">The rotation of the rectangle</param>
        public Rectangle(float width, float height, float rotation) : this(width, height) {
            this.rotation = rotation;
        }

        /// <summary>
        /// Create a rectangle
        /// </summary>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
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

    /// <summary>
    /// It's a circle... yes, just a circle.
    /// </summary>
    public struct Circle {
        /// <summary>
        /// The center of the circle
        /// </summary>
        public Point center;
        /// <summary>
        /// The radius of the circle
        /// </summary>
        public float radius;

        internal bool contains(Point point) {
            if(Utility.distance(center, point)>radius) {
                return false;
            }
            return true;
        }
    }
}
