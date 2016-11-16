using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Gmich.Cedrus.Rendering
{

    public class LayerRenderer : ARenderer
    {
        public LayerRenderer(GraphicsDevice device, RenderTarget2D renderTarget, Func<Color> deviceDefaultColor)
            : base(device, deviceDefaultColor, () => device.SetRenderTarget(renderTarget), () => device.SetRenderTarget(null))
        {
        }
    }
}