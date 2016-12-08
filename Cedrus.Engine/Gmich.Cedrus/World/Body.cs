using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Gmich.Cedrus.World
{
    public abstract class Body
    {
        public Vector2 Position { get; }

        public IEnumerable<Constraint> Constraints { get; }

        public class Constraint
        { }

    }

    public class StaticBody : Body
    {

    }

    public class DynamicBody : Body
    {
    }

}
