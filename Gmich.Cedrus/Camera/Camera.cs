using Microsoft.Xna.Framework;

namespace Gmich.Cedrus.Camera
{

    public sealed class Camera : ICamera
    {
        private Matrix transformationMatrix;
        public Rectangle ViewPort { get; private set; }
        public Rectangle VisibleArea { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 Zoom { get; private set; }
        public Vector3 Rotation { get; private set; }

        private void CalculateVisibleArea(Vector2 viewportSize)
        {
            var inverseViewMatrix = Matrix.Invert(transformationMatrix);
            var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            var tr = Vector2.Transform(new Vector2(viewportSize.X, 0), inverseViewMatrix);
            var bl = Vector2.Transform(new Vector2(0, viewportSize.Y), inverseViewMatrix);
            var br = Vector2.Transform(viewportSize, inverseViewMatrix);
            var min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
            var max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
            VisibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        private void CalculateTransformationMatrix(Vector2 viewportSize)
        {
            transformationMatrix = Matrix.CreateTranslation(new Vector3(viewportSize.X / 2 + Position.X, -viewportSize.Y / 2 + Position.Y, Position.Z)) *
                                                 Matrix.CreateRotationX(Rotation.X) *
                                                 Matrix.CreateRotationY(Rotation.Y) *
                                                 Matrix.CreateRotationZ(Rotation.Z) *
                                                 Matrix.CreateScale(new Vector3(Zoom.X, Zoom.Y, Zoom.Z)) *
                                                 Matrix.CreateTranslation(new Vector3(viewportSize.X / 2, viewportSize.Y / 2, 0));
        }

        public Matrix Update(Vector3 position, Vector3 zoom, Vector3 rotation, Vector2 viewportSize)
        {
            Position = position;
            Zoom = zoom;
            Rotation = rotation;
            ViewPort = new Rectangle((int)Position.X, (int)Position.Y, (int)viewportSize.X, (int)viewportSize.Y);
            CalculateTransformationMatrix(viewportSize);
            CalculateVisibleArea(viewportSize);
            return transformationMatrix;
        }
    }
}
