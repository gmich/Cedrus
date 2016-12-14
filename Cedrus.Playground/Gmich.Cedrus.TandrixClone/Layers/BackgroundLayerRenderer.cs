using Gmich.Cedrus.IOC;
using Gmich.Cedrus.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gmich.Cedrus.TandrixClone.Layers
{
    internal class BackgroundLayerRenderer : LayerRenderer
    {
        public BackgroundLayerRenderer(
            GraphicsDevice device, 
            [IocKey("background")]RenderTarget2D renderTarget, 
            [IocKey("backgroundColor")]Func<Color> deviceDefaultColor)
            : base(device, renderTarget, deviceDefaultColor)
        {
        }
    }
}
