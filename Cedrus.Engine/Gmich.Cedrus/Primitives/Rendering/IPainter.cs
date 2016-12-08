using System.Collections.Generic;

namespace Gmich.Cedrus.Primitives
{
    public interface IPainter
    {
        List<IShape> Shapes { get; }
        void Paint(double timeDelta);
    }
}