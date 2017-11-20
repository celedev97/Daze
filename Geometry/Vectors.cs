using Daze.Geometry;
using System;

namespace Daze.Vectors {
    /// <summary>
    /// A vector
    /// </summary>
    public struct Vector {
        /// <summary>
        /// An empty vector
        /// </summary>
        public static readonly Vector ZERO  = new Vector( 0,  0);
        /// <summary>
        /// A vector pointing up
        /// </summary>
        public static readonly Vector UP    = new Vector( 0, -1);
        /// <summary>
        /// A vector pointing down
        /// </summary>
        public static readonly Vector DOWN  = new Vector( 0,  1);
        /// <summary>
        /// A vector pointing left
        /// </summary>
        public static readonly Vector LEFT  = new Vector(-1,  0);
        /// <summary>
        /// A vector pointing right
        /// </summary>
        public static readonly Vector RIGHT = new Vector(+1,  0);

        /// <summary>
        /// The x of the vector
        /// </summary>
        public float x;
        /// <summary>
        /// The y of the vector
        /// </summary>
        public float y;
        /// <summary>
        /// Create a vector from it's coordinates
        /// </summary>
        /// <param name="x">The x of the vector</param>
        /// <param name="y">The y of the vector</param>
        public Vector(float x, float y) {
            this.x = x;
            this.y = y;
        }

        #region Methods and constructors to copy another Vector
        /// <summary>
        /// Create a Vector by copying another one (it's the same as duplicate)
        /// </summary>
        /// <param name="vectorToCopy"></param>
        public Vector(Vector vectorToCopy) {
            this.x = vectorToCopy.x;
            this.y = vectorToCopy.y;
        }
        /// <summary>
        /// Copy another vector into this one
        /// </summary>
        /// <param name="vectorToCopy">The vector to copy</param>
        public void set(Vector vectorToCopy) {
            this.x = vectorToCopy.x;
            this.y = vectorToCopy.y;
        }
        /// <summary>
        /// Set the vector without recreating it
        /// </summary>
        /// <param name="x">The x coordinate of the Vector</param>
        /// <param name="y">The y coordinate of the Vector</param>
        public void set(float x, float y) {
            this.x = x;
            this.y = y;
        }
        /// <summary>
        /// Create a vector from another one
        /// </summary>
        /// <returns>The new created vector</returns>
        public Vector duplicate() {
            return new Vector(this.x, this.y);
        }
        #endregion

        #region Operators' overload
        #region Math operators
        /// <summary>
        /// Convert a Point to a Vector
        /// </summary>
        /// <param name="value">The point to convert</param>
        public static implicit operator Vector(Point value) {
            return new Vector(value.x, value.y);
        }
        /// <summary>
        /// Sum two vectors
        /// </summary>
        /// <param name="vect1">The first vector to sum</param>
        /// <param name="vect2">The second vector to sum</param>
        /// <returns>The resul of the operation</returns>
        public static Vector operator +(Vector vect1, Vector vect2) {
            return new Vector(vect1.x + vect2.x, vect1.y + vect2.y);
        }
        /// <summary>
        /// Sum two vectors
        /// </summary>
        /// <param name="vect1">The first vector to sum</param>
        /// <param name="vect2">The second vector to sum</param>
        /// <returns>The resul of the operation</returns>
        public static Vector operator +(Vector vect1, IntVector vect2) {
            return new Vector(vect1.x + vect2.x, vect1.y + vect2.y);
        }
        /// <summary>
        /// Sum the first vector to the opposite of the second one
        /// </summary>
        /// <param name="vect1">The first vector</param>
        /// <param name="vect2">The vector that will be subtracted</param>
        /// <returns>The result of the operation</returns>
        public static Vector operator -(Vector vect1, Vector vect2) {
            return new Vector(vect1.x - vect2.x, vect1.y - vect2.y);
        }
        /// <summary>
        /// Sum the first vector to the opposite of the second one
        /// </summary>
        /// <param name="vect1">The first vector</param>
        /// <param name="vect2">The vector that will be subtracted</param>
        /// <returns>The result of the operation</returns>
        public static Vector operator -(Vector vect1, IntVector vect2) {
            return new Vector(vect1.x - vect2.x, vect1.y - vect2.y);
        }
        /// <summary>
        /// Scale the vector by a multiplier
        /// </summary>
        /// <param name="vect1">Vector to scale</param>
        /// <param name="multiplier">multiplier</param>
        /// <returns>The scaled vector</returns>
        public static Vector operator *(Vector vect1, float multiplier) {
            return new Vector(vect1.x * multiplier, vect1.y * multiplier);
        }
        /// <summary>
        /// Scale the vector down by dividing it
        /// </summary>
        /// <param name="vect1">Vector to scale</param>
        /// <param name="dividend">The dividend</param>
        /// <returns>The scaled vector</returns>
        public static Vector operator /(Vector vect1, float dividend) {
            return new Vector(vect1.x / dividend, vect1.y / dividend);
        }
        #endregion
        #region Logical Operators
        /// <summary>
        /// Check it two vectors are not the same vector
        /// </summary>
        /// <param name="v1">The first vector to check</param>
        /// <param name="v2">The second vector to check</param>
        /// <returns>False if the the two vector coincide</returns>
        public static bool operator !=(Vector v1, Vector v2) {
            return v1.x != v2.x || v1.y != v2.y;
        }
        /// <summary>
        /// Check it two vectors are the same vector
        /// </summary>
        /// <param name="v1">The first vector to check</param>
        /// <param name="v2">The second vector to check</param>
        /// <returns>True if the the two vector coincide</returns>
        public static bool operator ==(Vector v1, Vector v2) {
            return v1.x == v2.x && v1.y == v2.y;
        }
        #endregion
        #endregion

        /// <summary>
        /// Return this vector normalized (with the same orientation but a length of 1)
        /// </summary>
        /// <returns>The normalized vector</returns>
        public Vector normalize() {
            float ratio = Geometry.Utility.getHypotenuse(x,y);
            x /= ratio;
            y /= ratio;
            return this;
        }

        /// <summary>
        /// Return true if the second object is a vector with the same informations as this one
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>True if the two vectors coincide</returns>
        public override bool Equals(object obj) {
            if(typeof(Vector) != obj.GetType()) return false;
            return this == (Vector)obj;
        }

        /// <summary>
        /// Get the hash code of this object
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode() {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }
    }

    /// <summary>
    /// A vector with int coordinates, it should be used only for drawing, use Vector for physics
    /// </summary>
    public struct IntVector {
        /// <summary>
        /// An empty vector
        /// </summary>
        public static readonly IntVector ZERO  = new IntVector( 0,  0);
        /// <summary>
        /// A vector pointing up
        /// </summary>
        public static readonly IntVector UP    = new IntVector( 0, -1);
        /// <summary>
        /// A vector pointing down
        /// </summary>
        public static readonly IntVector DOWN  = new IntVector( 0,  1);
        /// <summary>
        /// A vector pointing left
        /// </summary>
        public static readonly IntVector LEFT  = new IntVector(-1,  0);
        /// <summary>
        /// A vector pointing right
        /// </summary>
        public static readonly IntVector RIGHT = new IntVector(+1,  0);

        /// <summary>
        /// The x offset of the Vector
        /// </summary>
        public int x;
        /// <summary>
        /// The y offset of the Vector
        /// </summary>
        public int y;
        /// <summary>
        /// Create an IntVector
        /// </summary>
        /// <param name="x">The x offset of the Vector</param>
        /// <param name="y">The y offset of the Vector</param>
        public IntVector(int x, int y) {
            this.x = x;
            this.y = y;
        }


        #region Methods and constructors to copy another Vector
        /// <summary>
        /// Create an IntVector from another IntVector (it is equivalent to IntVector.duplicate())
        /// </summary>
        /// <param name="vectorToCopy"></param>
        public IntVector(IntVector vectorToCopy) {
            this.x = vectorToCopy.x;
            this.y = vectorToCopy.y;
        }
        /// <summary>
        /// Copy the vector passed as a parameter into this one (it's the same as duplicate())
        /// </summary>
        /// <param name="vectorToCopy">The vector to copy</param>
        public void set(IntVector vectorToCopy) {
            this.x = vectorToCopy.x;
            this.y = vectorToCopy.y;
        }
        /// <summary>
        /// Change this vector values without recreating it
        /// </summary>
        /// <param name="x">The x coordinate of the vector</param>
        /// <param name="y">The y coordinate of the vector</param>
        public void set(int x, int y) {
            this.x = x;
            this.y = y;
        }
        /// <summary>
        /// Return a new IntVector with the same information as this one
        /// </summary>
        /// <returns></returns>
        public IntVector duplicate() {
            return new IntVector(this.x, this.y);
        }

        /// <summary>
        /// Return true if the second object is a vector with the same informations as this one
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>True if the two vectors coincide</returns>
        public override bool Equals(object obj) {
            return base.Equals(obj);
        }
        /// <summary>
        /// Get the Hash code of this object
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode() {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }
        #endregion

        #region Operators' overload
        /// <summary>
        /// Convert a Point to an IntVector
        /// </summary>
        /// <param name="value">The converted IntVector</param>
        public static implicit operator IntVector(Point value) {
            return new IntVector((int)Math.Round(value.x), (int)Math.Round(value.y));
        }

        /// <summary>
        /// Check if two vectors are the same vector
        /// </summary>
        /// <param name="vect1">The first vector</param>
        /// <param name="vect2">The second vector</param>
        /// <returns>True if the two vectors have the same information</returns>
        public static bool operator ==(IntVector vect1, IntVector vect2) {
            return vect1.x == vect2.x && vect1.y == vect2.y;
        }

        /// <summary>
        /// Check if two vectors are the same vector
        /// </summary>
        /// <param name="vect1">The first vector</param>
        /// <param name="vect2">The second vector</param>
        /// <returns>False if the two vectors have the same information</returns>
        public static bool operator !=(IntVector vect1, IntVector vect2) {
            return !(vect1 == vect2);
        }
        /// <summary>
        /// Sum two vectors
        /// </summary>
        /// <param name="vect1">The first vector to sum</param>
        /// <param name="vect2">The second vector to sum</param>
        /// <returns>The resul of the operation</returns>
        public static IntVector operator +(IntVector vect1, IntVector vect2) {
            return new IntVector(vect1.x + vect2.x, vect1.y + vect2.y);
        }
        /// <summary>
        /// Sum the first vector to the opposite of the second one
        /// </summary>
        /// <param name="vect1">The first vector</param>
        /// <param name="vect2">The vector that will be subtracted</param>
        /// <returns>The result of the operation</returns>
        public static IntVector operator -(IntVector vect1, IntVector vect2) {
            return new IntVector(vect1.x - vect2.x, vect1.y - vect2.y);
        }
        /// <summary>
        /// Scale the vector by a multiplier
        /// </summary>
        /// <param name="vect1">Vector to scale</param>
        /// <param name="multiplier">multiplier</param>
        /// <returns>The scaled vector</returns>
        public static IntVector operator *(IntVector vect1, float multiplier) {
            return new IntVector((int)(vect1.x * multiplier), (int)(vect1.y * multiplier));
        }
        /// <summary>
        /// Scale the vector down by dividing it
        /// </summary>
        /// <param name="vect1">Vector to scale</param>
        /// <param name="dividend">The dividend</param>
        /// <returns>The scaled vector</returns>
        public static IntVector operator /(IntVector vect1, float dividend) {
            return new IntVector((int)(vect1.x / dividend), (int)(vect1.y / dividend));
        }
        /// <summary>
        /// Scale the vector by a multiplier
        /// </summary>
        /// <param name="vect1">Vector to scale</param>
        /// <param name="multiplier">multiplier</param>
        /// <returns>The scaled vector</returns>
        public static IntVector operator *(IntVector vect1, int multiplier) {
            return new IntVector(vect1.x * multiplier, vect1.y * multiplier);
        }
        /// <summary>
        /// Scale the vector down by dividing it
        /// </summary>
        /// <param name="vect1">Vector to scale</param>
        /// <param name="dividend">The dividend</param>
        /// <returns>The scaled vector</returns>
        public static IntVector operator /(IntVector vect1, int dividend) {
            return new IntVector(vect1.x / dividend, vect1.y / dividend);
        }
        #endregion

    }

}
