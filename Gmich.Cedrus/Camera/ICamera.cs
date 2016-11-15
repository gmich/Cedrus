using Microsoft.Xna.Framework;

namespace Gmich.Cedrus.Camera
{
    public interface ICamera
    {
        Vector3 Position { get; }
        Vector3 Rotation { get; }
        Rectangle ViewPort { get; }
        Rectangle VisibleArea { get; }
        Vector3 Zoom { get; }

        Matrix Update(Vector3 position, Vector3 zoom, Vector3 rotation, Vector2 viewportSize);
    }
}