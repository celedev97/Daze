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

        /// <summary>
        /// Sum a point and a vector
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="vector">The vector to sum to the point</param>
        /// <returns></returns>
        public static Point operator +(Point point, Vector vector) {
            return new Point(point.x + vector.x, point.y + vector.y);
        }

        /// <summary>
        /// Sum the opposite of a vector to a point
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="vector">The vector to sum to the point</param>
        /// <returns></returns>
        public static Point operator -(Point point, Vector vector) {
            return new Point(point.x - vector.x, point.y - vector.y);
        }

        /// <summary>
        /// Decrease the first point x and y by the second point x and y
        /// </summary>
        /// <param name="point">The first point</param>
        /// <param name="point2">The second point</param>
        /// <returns></returns>
        public static Vector operator -(Point point, Point point2) {
            return new Vector(point.x - point2.x, point.y - point2.y);
        }

    }

    /// <summary>
    /// A line segment
    /// </summary>
    public struct Line {
        /// <summary>
        /// One for the point of the line
        /// </summary>
        public Point point1;
        /// <summary>
        /// One for the point of the line
        /// </summary>
        public Point point2;

        /// <summary>
        /// Create a line segment
        /// </summary>
        /// <param name="point1">One of the extremes of the segment</param>
        /// <param name="point2">One of the extremes of the segment</param>
        public Line(Point point1, Point point2) {
            this.point1 = point1;
            this.point2 = point2;
        }

        /// <summary>
        /// Return a string representing the line, can be useful for debug
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return "{" + point1 + "," + point2 + "}";
        }
    }

    /// <summary>
    /// A struct that represent a Size, ideally it's like a rectangle
    /// </summary>
    public struct Size {
        /// <summary>
        /// The width of this item
        /// </summary>
        public int width;
        /// <summary>
        /// The height of this item
        /// </summary>
        public int height;

        /// <summary>
        /// Create a Size object
        /// </summary>
        /// <param name="width">The width of this item</param>
        /// <param name="height">The height of this item</param>
        public Size(int width, int height) {
            this.width = width;
            this.height = height;
        }


        #region Methods and constructors to copy another Size
        /// <summary>
        /// Set this width without recreating it
        /// </summary>
        /// <param name="width">The width of this item</param>
        /// <param name="height">The height of this item</param>
        public void set(int width, int height) {
            this.width = width;
            this.height = height;
        }
        /// <summary>
        /// this return a duplicate of this Size
        /// </summary>
        /// <returns></returns>
        public Size duplicate() {
            return new Size(this.width, this.height);
        }
        #endregion

        #region Operators' overload
        /// <summary>
        /// Add a size to this one
        /// </summary>
        /// <param name="size1">The firse size</param>
        /// <param name="size2">The size to sum</param>
        /// <returns>A new size that is the result of the sum</returns>
        public static Size operator +(Size size1, Size size2) {
            return new Size(size1.width + size2.width, size1.height + size2.height);
        }
        /// <summary>
        /// Subtract a size from this size
        /// </summary>
        /// <param name="size1">This size</param>
        /// <param name="size2">The size to subtract</param>
        /// <returns>The size after the operation</returns>
        public static Size operator -(Size size1, Size size2) {
            return new Size(size1.width - size2.width, size1.height - size2.height);
        }
        /// <summary>
        /// Scale a size by a multiplier
        /// </summary>
        /// <param name="size1">The size to be scaled</param>
        /// <param name="multiplier">The multiplier</param>
        /// <returns>The scaled Size</returns>
        public static Size operator *(Size size1, float multiplier) {
            return new Size((int)(size1.width * multiplier), (int)(size1.height * multiplier));
        }
        /// <summary>
        /// Scale a size by a dividend
        /// </summary>
        /// <param name="size1">The size to be scaled</param>
        /// <param name="dividend">The dividend</param>
        /// <returns>The scaled Size</returns>
        public static Size operator /(Size size1, float dividend) {
            return new Size((int)(size1.width / dividend), (int)(size1.height / dividend));
        }
        /// <summary>
        /// Scale a size by a multiplier
        /// </summary>
        /// <param name="size1">The size to be scaled</param>
        /// <param name="multiplier">The multiplier</param>
        /// <returns>The scaled Size</returns>
        public static Size operator *(Size size1, int multiplier) {
            return new Size(size1.width * multiplier, size1.height * multiplier);
        }
        /// <summary>
        /// Scale a size by a dividend
        /// </summary>
        /// <param name="size1">The size to be scaled</param>
        /// <param name="dividend">The dividend</param>
        /// <returns>The scaled Size</returns>
        public static Size operator /(Size size1, int dividend) {
            return new Size(size1.width / dividend, size1.height / dividend);
        }
        #endregion

    }

    #endregion
}
