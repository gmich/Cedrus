using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gmich.Cedrus.World
{
    class World
    {
        public IList<Fixture> Fixtures { get; }

        public IList<Body> Bodies { get; }

        public IList<Joint> Joints { get; }


    }
}
