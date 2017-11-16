using Daze.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daze.Geometry {
    #region Geometric structures
    /// <summary>
    /// A point, it has a X cordinate and a Y coordinate... that's pretty much all.
    /// </summary>
    public struct Point {
        /// <summary>
        /// The position of the point on the X axis
        /// </summary>
        public float x;
        /// <summary>
        /// The position of the point on the Y axis
        /// </summary>
        public float y;

        /// <summary>
        /// The origin point (0,0)
        /// </summary>
        public static readonly Point O = new Point(0,0);

        /// <summary>
        /// Create a point by it's coordinates
        /// </summary>
        /// <param name="x">The position of the point on the X axis</param>
        /// <param name="y">The position of the point on the Y axis</param>
        public Point(float x, float y) {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// This method rotate the point counterclockwise around the origin point
        /// </summary>
        /// <param name="angle">the angle of the rotation</param>
        public void rotatePointAroundO(float angle) {
            double sinA = Math.Sin(angle);
            double cosA = Math.Cos(angle);
            x = (float)(x * cosA - y * sinA);
            y = (float)(x * sinA + y * cosA);
        }

        /// <summary>
        /// return a copy of this Point
        /// </summary>
        /// <returns></returns>
        public Point duplicate() {
            return new Point(x, y);
        }

        /// <summary>
        /// Sum two points as if they are vectors
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <returns></returns>
        public static Point operator +(Point p1, Point p2) {
            return new Point(p1.x + p2.x, p1.y + p2.y);
        }

        /// <summary>
        /// A string rapresentation of this point
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return "(" + x.ToString().Replace(",", ".") + "," + y.ToString().Replace(",", ".") + ")";
        }

        public static Point operator +(Point point, Vector vector) {
            return new Point(point.x + vector.x, point.y + vector.y);
        }

        public static Point operator -(Point point, Vector vector) {
            return new Point(point.x - vector.x, point.y - vector.y);
        }

        public static Vector operator -(Point point, Point point2) {
            return new Vector(point.x - point2.x, point.y - point2.y);
        }

    }

    public struct Line {
        public Point point1;
        public Point point2;

        public Line(Point point1, Point point2) {
            this.point1 = point1;
            this.point2 = point2;
        }
        public override string ToString() {
            return "{" + point1 + "," + point2 + "}";
        }
    }

    public struct Size {
        public int width, height;
        public Size(int width, int height) {
            this.width = width;
            this.height = height;
        }


        #region Methods and constructors to copy another Size
        public void set(int width, int height) {
            this.width = width;
            this.height = height;
        }
        public Size duplicate() {
            return new Size(this.width, this.height);
        }
        #endregion

        #region Operators' overload
        public static Size operator +(Size size1, Size size2) {
            return new Size(size1.width + size2.width, size1.height + size2.height);
        }

        public static Size operator -(Size size1, Size size2) {
            return new Size(size1.width - size2.width, size1.height - size2.height);
        }
        public static Size operator *(Size size1, float multiplier) {
            return new Size((int)(size1.width * multiplier), (int)(size1.height * multiplier));
        }
        public static Size operator /(Size size1, float dividend) {
            return new Size((int)(size1.width / dividend), (int)(size1.height / dividend));
        }
        public static Size operator *(Size size1, int multiplier) {
            return new Size(size1.width * multiplier, size1.height * multiplier);
        }
        public static Size operator /(Size size1, int dividend) {
            return new Size(size1.width / dividend, size1.height / dividend);
        }
        #endregion

    }

    #endregion

    #region Physical structures

    #endregion
}
