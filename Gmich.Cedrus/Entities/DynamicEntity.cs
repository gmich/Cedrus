using Gmich.Cedrus.Scene;
using Microsoft.Xna.Framework;

namespace Gmich.Cedrus.Entities
{
    public abstract class DynamicEntity
    {
        public DynamicEntity(ISceneHost host)
        {
            Host = host;
        }

        public Input.InputConfiguration InputConfiguration { get; }

        public ISceneHost Host { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; set; }
        public float MaxVelocityX { get; set; }
        public double Acceleration { get; set; }

        public void MoveLeft(double deltaTime)
        {
            Velocity += new Vector2(-(float)(Acceleration * deltaTime), 0);
            NormalizeVelocityX();
        }

        public void MoveRight(double deltaTime)
        {
            Velocity += new Vector2((float)(Acceleration * deltaTime), 0);
            NormalizeVelocityX();
        }

        private void NormalizeVelocityX()
        {
            Velocity = new Vector2(MathHelper.Clamp(Velocity.X, -MaxVelocityX, MaxVelocityX), Velocity.Y);
        }

        public void UpdateLocation(double deltaTime)
        {
            Position += Velocity * (float)deltaTime;
        }

        void HandleInput(Input.InputHandler inputManager, Timeline time)
        {
            InputConfiguration.Check(inputManager);
        }

        void FixedUpdate(Timeline time)
        {

        }
    }
}
