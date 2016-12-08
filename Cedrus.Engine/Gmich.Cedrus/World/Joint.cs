using Microsoft.Xna.Framework;

namespace Gmich.Cedrus.World
{
    public class Joint
    {
        public Body First { get; }

        public Body Second { get; }

        public Vector2 From { get; }

        public Vector2 To { get; }
    }
}
