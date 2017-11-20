using System;

namespace Daze.Geometry {
    /// <summary>
    /// A static class with some methods that can be useful for creating new collider Types
    /// </summary>
    public static class Utility {
        /// <summary>
        /// Get the distance between 2 points
        /// </summary>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        /// <returns>The calculated distance</returns>
        public static float distance(Point point1, Point point2) {
            return getHypotenuse((point1.x - point2.x), (point1.y - point2.y));
        }

        /// <summary>
        /// Get the hypotenuse lenght in a triangle with an angle of 90 degrees
        /// </summary>
        /// <param name="cathetus1">The length of one of the cathetus</param>
        /// <param name="cathetus2">The length of one of the cathetus</param>
        /// <returns></returns>
        public static float getHypotenuse(float cathetus1, float cathetus2) {
            return (float)Math.Sqrt(cathetus1 * cathetus1 + cathetus2 * cathetus2);
        }

        /// <summary>
        /// Check if two lines have an intersection
        /// </summary>
        /// <param name="line1">The firse line</param>
        /// <param name="line2">The second line</param>
        /// <returns>True if they have an intersection, false otherwise</returns>
        public static bool linesIntersect(Line line1, Line line2) {
            //ax + by + c = 0
            // by = -ax -c
            // y = x (-a/b) (-c/b)

            // a = y1 - y2
            float A1 = line1.point1.y - line1.point2.y;
            float A2 = line2.point1.y - line2.point2.y;

            //b = x2 - x1
            float B1 = line1.point2.x - line1.point1.x;
            float B2 = line2.point2.x - line2.point1.x;

            //c = x1y2 -y1x2
            float C1 = line1.point1.x * line1.point2.y - line1.point1.y * line1.point2.x;
            float C2 = line2.point1.x * line2.point2.y - line2.point1.y * line2.point2.x;

            float delta = A1*B2 - A2*B1; //determinante del sistema di equazioni
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
                float x = (B1*C2 - C1*B2)/delta;
                float y = (A2*C1 - A1*C2)/delta;

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

        /// <summary>
        /// Check if a line and a circle intersect
        /// </summary>
        /// <param name="line">The line</param>
        /// <param name="circle">The circle</param>
        /// <returns>True if they have an intersection, false otherwise</returns>
        public static bool lineIntersect(Line line, Circle circle) {
            //ax + by + c = 0
            // by = -ax -c
            // y = x (-a/b) (-c/b)

            // a = y1 - y2
            float a = line.point1.y - line.point2.y;

            //b = x2 - x1
            float b = line.point2.x - line.point1.x;

            //c = x1y2 -y1x2
            float c = line.point1.x * line.point2.y - line.point1.y * line.point2.x;

            //creo degli alias per i componenti del cerchio altrimenti mi sparo quando scrivo le formule
            float cx = circle.center.x;
            float cy = circle.center.y;
            float r = circle.radius;

            if(b != 0) {
                //la linea non è parallela a Y, quindi posso svolgere il sistema sostituendo Y nell'equazione della circonferenza
                //sistema: (x-cx)^2 + (y/cy)^2 = r^2   |  y = mx+q
                float m = -a/b;
                float q = -c/b;

                //componenti dell'equazione: ax^2 + bx +c = 0
                float eqA = 1+m*m;
                float eqB = 2*cx + 2*m*q + 2*m*cy;
                float eqC = cy*cy + cx*cx - 2*q*cy - r*r;

                float delta = eqB*eqB - 4*eqA*eqC;
                if(delta < 0) {
                    return false;
                } else {
                    //trovo la x e la y della/delle intersezione/i e uso la funzione between per capire se è nel segmento
                    float x2 = (float)((-eqB - Math.Sqrt(delta))/2*eqA);
                    float x1 = (float)((-eqB + Math.Sqrt(delta))/2*eqA);
                    float y2 = m*x2 + q;
                    float y1 = m*x2 + q;
                    if((between(x1, line.point1.x, line.point2.x) && between(y1, line.point1.y, line.point2.y))
                        || (between(x2, line.point1.x, line.point2.x) && between(y2, line.point1.y, line.point2.y))) {
                        return true;
                    }
                    return false;
                }
            } else {
                //la linea è parallela a Y
                //retta = x = -c/a => x = altQ
                //sistema: (x-cx)^2 + (y/cy)^2 = r^2   |  x = -c/a 
                float altQ = -c/a;

                float eqB = 2*cy;
                float eqC = cx*cx + cy*cy - r*r + altQ*altQ - 2*altQ*cx;

                float delta = eqB*eqB - 4*eqC;
                if(delta < 0) {
                    return false;
                } else {
                    //trovo la x e la y dell'intersezione e uso la funzione between per capire se è nel segmento
                    float x = altQ;
                    float y2 = (float)((-eqB - Math.Sqrt(delta))/2);
                    float y1 = (float)((-eqB + Math.Sqrt(delta))/2);

                    if(between(x, line.point1.x, line.point2.x) &&
                        (between(y1, line.point1.y, line.point2.y)) || between(y2, line.point1.y, line.point2.y)) {
                        return true;
                    }
                    return false;
                }

            }
        }
        /// <summary>
        /// Check if a number is between an interval
        /// </summary>
        /// <param name="toCheck">The number to check</param>
        /// <param name="extreme1">One of the extremes of the interval</param>
        /// <param name="extreme2">One of the extremes of the interval</param>
        /// <returns></returns>
        public static bool between(float toCheck, float extreme1, float extreme2) {
            if(extreme1 > extreme2) {
                return extreme1 >= toCheck && toCheck >= extreme2;
            } else {
                return extreme1 <= toCheck && toCheck <= extreme2;
            }
        }

        /// <summary>
        /// Get the angular coefficient from a segment
        /// </summary>
        /// <param name="line">The segment</param>
        /// <returns>The angular coefficient</returns>
        public static float getAngularCoefficient(Line line) {
            return getAngularCoefficient(line.point1, line.point2);
        }

        //funziona con bounding box con rotazione 0
        /// <summary>
        /// Check if a point is in the bounding box of a line (to be use only to do fast checks)
        /// </summary>
        /// <param name="pointToCheck">The point</param>
        /// <param name="line">The line</param>
        /// <returns>True when the segment is inside the segment bounding box</returns>
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
        
        /// <summary>
        /// Get the angular coefficient from a segment
        /// </summary>
        /// <param name="p1">One of the extremes of the segment</param>
        /// <param name="p2">One of the extremes of the segment</param>
        /// <returns>The angular coefficient</returns>
        public static float getAngularCoefficient(Point p1, Point p2) {
            return (p2.y - p1.y) / (p2.x - p1.x);
        }

        /// <summary>
        /// Calculate the offset of a line on the Y axis from the origin point
        /// </summary>
        /// <param name="line">The line</param>
        /// <returns>The offset on the Y axis from O</returns>
        public static float calculateLineOffsetYFromO(Line line) {
            return (line.point1.y * line.point2.x - line.point1.x * line.point2.y) / (line.point2.x - line.point1.x);
        }

        /// <summary>
        /// Check if a line intersect with a circle
        /// </summary>
        /// <param name="line1">The line</param>
        /// <param name="circle">The circle</param>
        /// <returns></returns>
        public static bool linesIntersect(Line line1, Circle circle) {
            throw new NotImplementedException();
        }

    }
}


