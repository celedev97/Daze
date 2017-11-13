namespace Daze.Vectors {
    public struct Vector {
        public static readonly Vector ZERO  = new Vector( 0,  0);
        public static readonly Vector UP    = new Vector( 0, -1);
        public static readonly Vector DOWN  = new Vector( 0,  1);
        public static readonly Vector LEFT  = new Vector(-1,  0);
        public static readonly Vector RIGHT = new Vector(+1,  0);

        public float x, y;
        public Vector(float x, float y) {
            this.x = x;
            this.y = y;
        }

        #region Methods and constructors to copy another Vector
        public Vector(Vector vectorToCopy) {
            this.x = vectorToCopy.x;
            this.y = vectorToCopy.y;
        }
        public void set(Vector vectorToCopy) {
            this.x = vectorToCopy.x;
            this.y = vectorToCopy.y;
        }
        public void set(float x, float y) {
            this.x = x;
            this.y = y;
        }
        public Vector duplicate() {
            return new Vector(this.x, this.y);
        }
        #endregion

        #region Operators' overload
        #region Math operators
        public static implicit operator Vector(System.Drawing.Point value) {
            return new Vector(value.X, value.Y);
        }
        public static Vector operator +(Vector vect1, Vector vect2) {
            return new Vector(vect1.x + vect2.x, vect1.y + vect2.y);
        }
        public static Vector operator +(Vector vect1, IntVector vect2) {
            return new Vector(vect1.x + vect2.x, vect1.y + vect2.y);
        }
        public static Vector operator -(Vector vect1, Vector vect2) {
            return new Vector(vect1.x - vect2.x, vect1.y - vect2.y);
        }
        public static Vector operator -(Vector vect1, IntVector vect2) {
            return new Vector(vect1.x - vect2.x, vect1.y - vect2.y);
        }
        public static Vector operator *(Vector vect1, float multiplier) {
            return new Vector(vect1.x * multiplier, vect1.y * multiplier);
        }
        public static Vector operator /(Vector vect1, float dividend) {
            return new Vector(vect1.x / dividend, vect1.y / dividend);
        }
        #endregion
        #region Logical Operators
        public static bool operator !=(Vector v1, Vector v2) {
            return v1.x != v2.x || v1.y != v2.y;
        }

        public static bool operator ==(Vector v1, Vector v2) {
            return v1.x == v2.x && v1.y == v2.y;
        }
        #endregion
        #endregion

        public Vector normalize() {
            float ratio = Geometry.Utility.getHypotenuse(x,y);
            x /= ratio;
            y /= ratio;
            return this;
        }

        public override bool Equals(object obj) {
            if(typeof(Vector) != obj.GetType()) return false;
            return this == (Vector)obj;
        }
    }

    public struct IntVector {
        public static readonly Vector UP    = new Vector( 0, -1);
        public static readonly Vector DOWN  = new Vector( 0,  1);
        public static readonly Vector LEFT  = new Vector(-1,  0);
        public static readonly Vector RIGHT = new Vector(+1,  0);

        public int x, y;
        public IntVector(int x, int y) {
            this.x = x;
            this.y = y;
        }


        #region Methods and constructors to copy another Vector
        public IntVector(IntVector vectorToCopy) {
            this.x = vectorToCopy.x;
            this.y = vectorToCopy.y;
        }
        public void set(IntVector vectorToCopy) {
            this.x = vectorToCopy.x;
            this.y = vectorToCopy.y;
        }
        public void set(int x, int y) {
            this.x = x;
            this.y = y;
        }
        public IntVector duplicate() {
            return new IntVector(this.x, this.y);
        }
        #endregion

        #region Operators' overload
        public static implicit operator IntVector(System.Drawing.Point value) {
            return new IntVector(value.X, value.Y);
        }

        public static IntVector operator +(IntVector vect1, IntVector vect2) {
            return new IntVector(vect1.x + vect2.x, vect1.y + vect2.y);
        }

        public static IntVector operator -(IntVector vect1, IntVector vect2) {
            return new IntVector(vect1.x - vect2.x, vect1.y - vect2.y);
        }
        public static IntVector operator *(IntVector vect1, float multiplier) {
            return new IntVector((int)(vect1.x * multiplier), (int)(vect1.y * multiplier));
        }
        public static IntVector operator /(IntVector vect1, float dividend) {
            return new IntVector((int)(vect1.x / dividend), (int)(vect1.y / dividend));
        }
        public static IntVector operator *(IntVector vect1, int multiplier) {
            return new IntVector(vect1.x * multiplier, vect1.y * multiplier);
        }
        public static IntVector operator /(IntVector vect1, int dividend) {
            return new IntVector(vect1.x / dividend, vect1.y / dividend);
        }
        #endregion

    }

}
