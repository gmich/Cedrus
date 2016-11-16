using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gmich.Cedrus.Rendering
{
    public class ScreenRenderer : ARenderer
    {
        public ScreenRenderer(GraphicsDevice device, Func<Color> deviceDefaultColor, Action preRender, Action postRender) 
            : base(device, deviceDefaultColor, preRender, postRender)
        {
        }
    }
}
