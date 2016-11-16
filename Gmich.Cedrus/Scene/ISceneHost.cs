using Gmich.Cedrus.Input;
using Gmich.Cedrus.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gmich.Cedrus.Scene
{
    public interface ISceneHost
    {
        IAppender Logger { get; }
        GraphicsDevice Device { get; }
        GameSettings GameSettings { get; }
        SceneSettings SceneSettings { get; }
        GameWindow Window { get; }
        ContentContainer ContentContainer { get; }

        void Unload();
        void HandleInput(InputManager inputManager, Timeline time);
        void FixedUpdate(Timeline time);
        void Render();
    }

}
