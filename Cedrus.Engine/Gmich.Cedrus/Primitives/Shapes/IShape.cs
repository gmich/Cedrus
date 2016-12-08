using Microsoft.Xna.Framework;

namespace Gmich.Cedrus.Primitives
{
    public interface IShape
    {
        Color Color { get; set; }
        float ViewportOffsetX { get; set; }
        float ViewportOffsetY { get; set; }

        VertexPosition[] VerticesPosition { get; }

        void Draw();
    }
}