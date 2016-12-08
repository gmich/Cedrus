using Microsoft.Xna.Framework;

namespace Gmich.Cedrus
{
    public class GameSettings
    {
        public GameSettings(GameWindow window)
        {
            Window = window;
        }

        GameWindow Window { get; }
    }
}
