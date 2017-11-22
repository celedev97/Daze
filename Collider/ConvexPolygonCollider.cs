using System;
using Daze.Geometry;

namespace Daze {
    /// <summary>
    /// A general collider that can have different shapes as long as it's shape is convex
    /// </summary>
    public abstract class ConvexPolygonCollider : Collider {
        /// <summary>
        /// The polygon used to check collisions
        /// </summary>
        internal protected ConvexPolygon polygon;

        /// <summary>
        /// Create a ConvexPolygonCollider
        /// </summary>
        /// <param name="gameObject">The gameObject that will be used to calculate the polygon position</param>
        protected ConvexPolygonCollider(GameObject gameObject) : base(gameObject) { }

        /// <summary>
        /// This check if this collider is colliding with another one
        /// </summary>
        /// <param name="collider2">The second collider to check</param>
        /// <param name="firstTry">Send false if this method is called after a collider couldn't check the collision</param>
        /// <returns>True if the two collider collide, false otherwise</returns>
        public override bool Collide(Collider collider2, bool firstTry = true) {
            Type collider2Type = collider2.GetType();
            if(collider2Type.IsSubclassOf(typeof(ConvexPolygonCollider))) {
                ConvexPolygon poly1 = polygon;
                ConvexPolygon poly2 = ((ConvexPolygonCollider) collider2).polygon;

                float centerDistance = Utility.distance(poly1.center, poly2.center);
                float raySum = (poly1.ray + poly2.ray);

                //if the distance of the centers is > then they sum of the rays of the circonferences that contains the polygons then they can't collide, and it's only a loss of time to check if they do
                if(centerDistance > raySum) {
                    return false;
                } else {
                    //else if the distance is smaller then the raysum they may intersect or one of them may be inside the other one
                    //so i'm checking if one is inside the other one right now to avoid losing time by checking all the edges
                    if(poly1.contains(poly2.center) || poly2.contains(poly1.center)) {
                        return true;
                    }
                }

                //if I'm at this point then the colliders are not too far, but not even too near, there is no other way but to check every edge intersection

                //I do a swap so that i cycle on the polygon with more edges before.
                //This is almost surely useless, but... I kinda had the feeling that it was faster this way.
                //I know this actually doesn't make sense, but anyway this doesn't slow down the code, so why not do it?
                if(poly1.lines.Length < poly2.lines.Length) {
                    ConvexPolygon temp = poly1;
                    poly1 = poly2;
                    poly2 = temp;
                }
                
                //controlling the edge collision one by one is tecnically slower than using ONLY SAT, but this part of the code is executed only if the polygons
                //are very near but not too much, so if this part of code is executed a collision is very likely to be happening,
                //and while SAT is faster if they DON'T collide, this is faster if they DO collide, this is why i think this is a better solution than SAT.
                foreach(Line line in poly1.lines) {
                    foreach(Line line2 in poly2.lines) {
                        if(Utility.linesIntersect(line, line2))
                            return true;
                    }
                }
                //if I've arrived till there then the colliders are really near, but there is no collision
                return false;
            } else if(collider2Type == typeof(CircleCollider)){
                Circle circle = ((CircleCollider)collider2).circle;
                float centerDistance = Utility.distance(polygon.center, circle.center);
                float raySum = (polygon.ray + circle.radius);
                //if the distance of the centers is > then they sum of the rays of the circonferences that contains the polygons then they can't collide, and it's only a loss of time to check if they do
                if(centerDistance > raySum) {
                    return false;
                } else {
                    //else if the distance is smaller then the raysum they may intersect or one of them may be inside the other one
                    //so i'm checking if one is inside the other one right now to avoid losing time by checking all the edges
                    if(circle.contains(polygon.center) || polygon.contains(circle.center)) {
                        return true;
                    }
                }

                //is no one is inside the other one then i have to check collision for every line
                foreach(Line line in polygon.lines) {
                    if(Utility.lineIntersect(line, circle)) return true;
                }
                return false;
            } else {
                return handleNotImplementedCollision(collider2, firstTry);
            }
        }

        /// <summary>
        /// This forces the collider's coordinates recalculation after that the gameObject rotates
        /// </summary>
        /// <param name="gameObject">The gameObject that will be used to check rotation and calculate the new coordinates</param>
        protected override void Rotate(GameObject gameObject) {
            polygon.rotation = gameObject.rotation;
        }

        /// <summary>
        /// This update the collider's coordinates after that the gameObject moves
        /// </summary>
        /// <param name="gameObject">The gameobject that will be used to recalculate the coordinates, it should be this collider's gameobject</param>
        protected override void Move(GameObject gameObject) {
            polygon.center = gameObject.position;
        }

        /// <summary>
        /// This method check if a point is inside the Collider
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is inside the Collider, false otherwise</returns>
        protected internal override bool InCollider(Point point) {
            return polygon.contains(point);
        }

    }
}
