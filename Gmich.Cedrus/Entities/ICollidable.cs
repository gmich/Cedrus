namespace Gmich.Cedrus.Entities
{
    interface ICollidable
    {
        bool Intersects(CollisionBox box);

        bool Intersects(CollisionPoint point);
    }
}
