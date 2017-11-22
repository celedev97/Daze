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
            //i get the line segment that goes from the point to the center of the polygon
            Line centersSegment = new Line(point, center);

            //if the line segment hits at least one of the edges of the polygon then the point is out of the polygon.
            //this approach makes so that this method is faster if the point is outside the polygon.
            //Tipically the usual way to check this is doing a longer line and counting the number of intersections while cycling,
            //but isn't it faster if you just return false when you know there is an intersection ;) ?
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

            //thoose coordinates are based on a normal XY cartesian system, not a system with the Y reversed like the screen
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
