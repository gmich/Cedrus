using System;
using Microsoft.Xna.Framework;

namespace Gmich.Cedrus.Entities
{
    public class CollisionPoint : ICollidable
    {
        public CollisionPoint(Point point)
        {
            Point = point;
        }
        public Point Point { get; }

        public bool Intersects(CollisionPoint point) => point.Equals(Point);
        public bool Intersects(CollisionBox other) => other.Rectangle.Contains(Point);
    }
}
