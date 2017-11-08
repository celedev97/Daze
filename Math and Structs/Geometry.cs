using System;

namespace Daze.Geometry {
    public static class Utility {
        public static float distance(Point point1, Point point2) {
            return getHypotenuse((point1.x - point2.x), (point1.y - point2.y));
        }

        public static float getHypotenuse(float cathetus1, float cathetus2) {
            return (float)Math.Sqrt(cathetus1 * cathetus1 + cathetus2 * cathetus2);
        }

        public static bool linesIntersect(Line line1, Line line2) {
            //ax + by + c = 0
            // by = -ax -c
            // y = x (-a/b) (-c/b)
            float A1 = line1.point2.y - line1.point1.y;
            float A2 = line2.point2.y - line2.point1.y;

            float B1 = line1.point1.x - line1.point2.x;
            float B2 = line2.point1.x - line2.point2.x;

            float C1 = A1 * line1.point1.x + B1 * line1.point1.y;
            float C2 = A2 * line2.point1.x + B2 * line2.point1.y;

            float delta = A1*B2 - A2*B1;
            if(delta == 0) {
                //le linee sono parallele o sono la stessa linea
                float Q1 = -C1/B1;
                float Q2 = -C2/B2;

                if(Q1 != Q2) {
                    //sono su rette parallele
                    return false;
                } else {
                    //sono sulla stessa retta
                    if(B1 == 0) {
                        //se B = 0 allora M = -C/B = infinito, quindi la retta è parallela all'asse Y e devo fare i controlli su X
                        Point point1 = line1.point1;
                        Point point2 = line1.point2;

                        Point point3 = line2.point1;
                        Point point4 = line2.point2;

                        //riordino i punti
                        Point temp;
                        if(point1.x > point2.x) {
                            temp = point1;
                            point1 = point2;
                            point2 = temp;
                        }
                        if(point3.x > point4.x) {
                            temp = point3;
                            point3 = point4;
                            point4 = temp;
                        }


                        //CASI DI INTERSEZIONE: 1----3---2----4; 3-----1----4---2;
                        if(between(point3.x, point1.x, point2.x) || between(point1.x, point3.x, point4.x)) {
                            return true;
                        } else {
                            return false;
                        }
                    } else {
                        //la retta è una normale retta non parallela a Y, c'è anche la possibilità che al contrario sia parallela a C, perciò per sicurezza faccio i controlli su Y
                        //se B = 0 allora M = -C/B = infinito, quindi la retta è parallela all'asse Y e devo fare i controlli su X
                        Point point1 = line1.point1;
                        Point point2 = line1.point2;

                        Point point3 = line2.point1;
                        Point point4 = line2.point2;

                        //riordino i punti
                        Point temp;
                        if(point1.y > point2.y) {
                            temp = point1;
                            point1 = point2;
                            point2 = temp;
                        }
                        if(point3.y > point4.y) {
                            temp = point3;
                            point3 = point4;
                            point4 = temp;
                        }


                        //CASI DI INTERSEZIONE: 1----3---2----4; 3-----1----4---2;
                        if(between(point3.y, point1.y, point2.y) || between(point1.y, point3.y, point4.y)) {
                            return true;
                        } else {
                            return false;
                        }
                    }
                }
            } else {
                //le rette non sono parallele, trovo l'intersezione normalmente
                float x = (B2*C1 - B1*C2)/delta;
                float y = (A1*C2 - A2*C1)/delta;

                if(B1 == 0) {
                    //linea parallela a Y, devo fare i controlli sulla Y
                    if(!between(y, line1.point1.y, line1.point2.y)) return false;
                } else {
                    //linea non parallela a Y, potenzialmente parallela a X
                    if(!between(x, line1.point1.x, line1.point2.x)) return false;
                }

                if(B2 == 0) {
                    //linea parallela a Y, devo fare i controlli sulla X
                    if(!between(y, line2.point1.y, line2.point2.y)) return false;
                } else {
                    //linea non parallela a Y, potenzialmente parallela a X
                    if(!between(x, line2.point1.x, line2.point2.x)) return false;
                }
                return true;
            }
        }
        
        public static bool between(float toCheck, float extreme1, float extreme2) {
            if(extreme1 > extreme2) {
                return extreme1 >= toCheck && toCheck >= extreme2;
            } else {
                return extreme1 <= toCheck && toCheck <= extreme2;
            }
        }

        public static float getAngularCoefficient(Line line) {
            return getAngularCoefficient(line.point1, line.point2);
        }

        //funziona con bounding box con rotazione 0
        public static bool pointInSegmentBoundingBox(Point pointToCheck, Line line) {
            float firstX = line.point1.x > line.point2.x ? line.point2.x : line.point1.x;
            float firstY = line.point1.y > line.point2.y ? line.point2.y : line.point1.y;

            float lastX = firstX + Math.Abs(line.point1.x - line.point2.x);
            float lastY = firstY + Math.Abs(line.point1.y - line.point2.y);

            if(pointToCheck.x >= firstX && pointToCheck.x <= lastX && pointToCheck.y >= firstY && pointToCheck.y <= lastY) {
                return true;
            }
            return false;
        }

        public static float getAngularCoefficient(Point p1, Point p2) {
            return (p2.y - p1.y) / (p2.x - p1.x);
        }

        public static float calculateLineOffsetYFromO(Line line) {
            return (line.point1.y * line.point2.x - line.point1.x * line.point2.y) / (line.point2.x - line.point1.x);
        }

        public static bool linesIntersect(Line line1, Circle circle) {
            throw new NotImplementedException();
        }

    }

    #region Basic Structures

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

    public struct Point {
        public float x;
        public float y;

        public static readonly Point O = new Point(0,0);

        public Point(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public void rotatePointAroundO(float angle) {
            double sinA = Math.Sin(angle);
            double cosA = Math.Cos(angle);
            x = (float)(x * cosA - y * sinA);
            y = (float)(x * sinA + y * cosA);
        }

        public Point duplicate() {
            return new Point(x, y);
        }

        public static Point operator +(Point p1, Point p2) {
            return new Point(p1.x + p2.x, p1.y + p2.y);
        }

        public override string ToString() {
            return "{" + x.ToString().Replace(",",".") + "," + y.ToString().Replace(",", ".") + "}";
        }
    }

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

    #endregion

    #region Shapes
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
        private float hiddenWidth; public float width { get => hiddenWidth; }
        private float hiddenHeight; public float height { get => hiddenHeight; }

        private Point hiddenCenter;
        public override Point center {
            get => hiddenCenter;
            set {
                A = RA.duplicate();
                B = RB.duplicate();
                C = RC.duplicate();
                D = RD.duplicate();

                A += value;
                B += value;
                C += value;
                D += value;

                hiddenCenter = value;

                calculateLines();
            }
        }

        private float hiddenRotation;
        public override float rotation {
            get => hiddenRotation;
            set {
                hiddenRotation = value;
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
            hiddenWidth = width;
            hiddenHeight = height;

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

            ray = Utility.distance(Point.O,A0);


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

            RA.rotatePointAroundO(hiddenRotation);
            RB.rotatePointAroundO(hiddenRotation);
            RC.rotatePointAroundO(hiddenRotation);
            RD.rotatePointAroundO(hiddenRotation);
        }

        private void calculateLines() {
            lines[0] = new Line(A, B);
            lines[1] = new Line(B, D);
            lines[2] = new Line(D, C);
            lines[3] = new Line(C, A);
        }

    }

    public struct Circle{
        public Point center;
        public float radius;

        internal bool contains(Point center) {
            throw new NotImplementedException();
        }
    }
    #endregion

}


