using Microsoft.Xna.Framework;

namespace Gmich.Cedrus.Camera
{
    public static class CameraExtensions
    {
        public static bool IsVisible(this ICamera camera, Vector2 location)
        {
            return (location.X > camera.Position.X && location.X < camera.Position.X + camera.ViewPort.Width
                 && location.Y > camera.Position.Y && location.Y < camera.Position.Y + camera.ViewPort.Height);
        }

        public static bool IsVisible(this ICamera camera, Rectangle bounds)
        {
            return (camera.ViewPort.Intersects(bounds));
        }

        public static Vector2 AdjustInWorldBounds(this ICamera camera, Vector2 location, float width, float height)
        {
            location.X = MathHelper.Clamp(location.X, camera.Position.X, camera.Position.X + camera.ViewPort.Width - width);
            location.Y = MathHelper.Clamp(location.Y, camera.Position.Y, camera.Position.Y + camera.ViewPort.Height - height);
            return location;
        }

        public static Vector2 AdjustInWorldBounds(this ICamera camera, Vector2 location, float width, float height, Vector2 origin)
        {
            location.X = MathHelper.Clamp(location.X, camera.Position.X + origin.X, camera.Position.X + camera.ViewPort.Width - width + origin.X);
            location.Y = MathHelper.Clamp(location.Y, camera.Position.Y + origin.Y, camera.Position.Y + camera.ViewPort.Height - height + origin.Y);
            return location;
        }

        public static Vector2 WorldToScreen(this ICamera camera, Vector2 worldLocation)
        {
            return worldLocation - camera.Position.ToVector2();
        }

        public static Rectangle WorldToScreen(this ICamera camera, Rectangle worldRectangle)
        {
            return new Rectangle(worldRectangle.Left - (int)camera.Position.X, worldRectangle.Top - (int)camera.Position.Y, worldRectangle.Width, worldRectangle.Height);
        }

        public static Vector2 ScreenToWorld(this ICamera camera, Vector2 screenLocation)
        {
            return screenLocation + camera.Position.ToVector2();
        }

        public static Rectangle ScreenToWorld(this ICamera camera, Rectangle screenRectangle)
        {
            return new Rectangle(screenRectangle.Left + (int)camera.Position.X, screenRectangle.Top + (int)camera.Position.Y, screenRectangle.Width, screenRectangle.Height);
        }

        public static Vector2 TranslateScreenToWorld(this Matrix matrix, Vector2 location)
        {
            return Vector2.Transform(location, Matrix.Invert(matrix));
        }

        public static Vector2 TranslateWorldToScreen(this Matrix matrix, Vector2 location)
        {
            return Vector2.Transform(location, matrix);
        }
    }
}
