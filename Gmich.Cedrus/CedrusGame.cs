using Autofac;
using Gmich.Cedrus.Input;
using Gmich.Cedrus.IOC;
using Gmich.Cedrus.Physics;
using Gmich.Cedrus.Rendering;
using Gmich.Cedrus.Scene;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;

namespace Gmich.Cedrus
{
    public class CedrusGame : Game
    {
        private readonly GameTimeline gameTimeline;
        private readonly InputManager inputManager;
        private readonly LogicUpdateManager logicUpdateManager;
        private readonly PhysicsUpdateManager physicsUpdateManager;
        private readonly RenderManager RenderManager;

        public CedrusGame(IocBuilder builder)
        {
            builder.RegisterModules(Assembly.GetExecutingAssembly(), type => type.FullName.EndsWith("Module"));
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
