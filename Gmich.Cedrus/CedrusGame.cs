using Autofac;
using Microsoft.Xna.Framework;

namespace Gmich.Cedrus
{
    public class CedrusGame : Game
    {
        private readonly TimeManager timeManager;
        public CedrusGame(ContainerBuilder builder)
        {
            var container = builder.Build();
            var timeManager = container.Resolve<TimeManager>();
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
            timeManager.TimeFactory = () => new Timeline(() => gameTime.ElapsedGameTime.TotalSeconds, () => gameTime.ElapsedGameTime.TotalMilliseconds);

        }

        protected override void Draw(GameTime gameTime)
        {
        }
    }
}
