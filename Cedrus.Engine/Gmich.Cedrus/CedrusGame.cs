using Gmich.Cedrus.Input;
using Gmich.Cedrus.IOC;
using Gmich.Cedrus.Logging;
using Gmich.Cedrus.Physics;
using Gmich.Cedrus.Rendering;
using Gmich.Cedrus.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Reflection;

namespace Gmich.Cedrus
{

    public class CedrusGame : Game, IDisposable
    {
        private readonly GameTimeline gameTimeline;
        //private readonly InputManager inputManager;
        //private readonly LogicUpdateManager logicUpdateManager;
        //private readonly PhysicsUpdateManager physicsUpdateManager;
        //private readonly RenderManager RenderManager;
        public IAppender Appender { get; }

        private readonly GraphicsDeviceManager graphics;

        public CedrusGame(IocBuilder builder)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            builder.LogRegistrations();
            builder.RegisterModules(Assembly.GetExecutingAssembly(), type => type.FullName.EndsWith("Module"));
            builder.RegisterSingleton(c => new GameSettings(Window));
            builder.RegisterSingleton(c => new ContentContainer(Content));

            var container = builder.Build();

            gameTimeline = container.Resolve<GameTimeline>();
            Appender = container.Resolve<IAppender>()[GetType()];
        }

        protected override void Initialize()
        {
            base.Initialize();
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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
