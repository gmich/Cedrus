using System;
using Microsoft.Xna.Framework;

namespace Gmich.Cedrus.Entities
{
    public class CollisionBox : ICollidable
    {
        public CollisionBox(Rectangle rectangle)
        {
            Rectangle = rectangle;
        }
        public Rectangle Rectangle { get; }

        public bool Intersects(CollisionPoint point) => Rectangle.Contains(point.Point);
        public bool Intersects(CollisionBox box) => Rectangle.Intersects(box.Rectangle);
    }
}
