using Gmich.Cedrus.Scene;
using Microsoft.Xna.Framework;

namespace Gmich.Cedrus.Entities
{
    public abstract class GameEntity
    {
        public GameEntity(ISceneHost host)
        {
            Host = host;
        }

        public ISceneHost Host { get; private set; }
        public Vector2 Location { get; private set; }
        public Vector2 Velocity { get; protected set; }
    }
}
