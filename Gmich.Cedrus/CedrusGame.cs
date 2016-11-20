using Autofac;
using Microsoft.Xna.Framework;
using System;

namespace Gmich.Cedrus
{
    public class CedrusGame : Game
    {
        private readonly GameTimeline gameTimeline;

        public CedrusGame(ContainerBuilder builder)
        {
            var container = builder.Build();
            gameTimeline = container.Resolve<GameTimeline>();
        }

        protected override void Initialize()
        {
        }

        protected override void LoadContent()
        {
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            gameTimeline.DeltaTime = gameTime.ElapsedGameTime;
            gameTimeline.TotalTime = gameTime.TotalGameTime;


        }

        protected override void Draw(GameTime gameTime)
        {
        }
    }
}
