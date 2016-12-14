using Gmich.Cedrus.Content;
using Gmich.Cedrus.Input;
using Gmich.Cedrus.IOC;
using Gmich.Cedrus.Logging;
using Gmich.Cedrus.Physics;
using Gmich.Cedrus.Rendering;
using Gmich.Cedrus.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Gmich.Cedrus
{

    public class CedrusGame : Game, IDisposable
    {
        private readonly GraphicsDeviceManager graphics;
        private readonly GameTimeline gameTimeline;
        private readonly IContainer container;
        //private readonly InputManager inputManager;
        //private readonly LogicUpdateManager logicUpdateManager;
        //private readonly PhysicsUpdateManager physicsUpdateManager;
        //private readonly RenderManager RenderManager;

        public IAppender Appender { get; }

        public CedrusGame(IocBuilder builder)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            builder.LogRegistrations();
            builder.RegisterModules(Assembly.GetExecutingAssembly(), type => type.FullName.EndsWith("Module"));

            builder.RegisterSingleton(c => graphics.GraphicsDevice);
            builder.RegisterSingleton(c => new GameSettings(Window));
            builder.RegisterSingleton(c => Content);
            EnumerateAssemblies(assembly => builder.RegisterSingletonSubclassesOf(assembly, typeof(CommonAssetBuilder)));
           
            container = builder.Build();

            gameTimeline = container.Resolve<GameTimeline>();
            Appender = container.Resolve<IAppender>()[GetType()];
        }

        private void EnumerateAssemblies(Action<Assembly> action)
        {
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                action(assembly);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            foreach (var builder in container.Resolve<IEnumerable<CommonAssetBuilder>>())
            {
                builder.Font.Build(Content);
                builder.Textures.Build(Content);
            }
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
