namespace Daze {

    public struct Size {
        public int width, height;
        public Size(int width, int height) {
            this.width = width;
            this.height = height;
        }


        #region Methods and constructors to copy another Vector
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

}
