using Gmich.Cedrus.IOC;
using Gmich.Cedrus.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gmich.Cedrus.TandrixClone.Layers
{
    internal class LayersModule : CendrusModule
    {
        public override void Register(IocBuilder builder)
        {
            builder.RegisterSingleton(c => 
                RenderingUtilities.CreateRenderTarget(c.Resolve<GraphicsDevice>(), 10, 10))
                .IdentifiedAs("background")
            .Register(c =>
                new Func<Color>(() => Color.White))
                .IdentifiedAs("backgroundColor")
            .RegisterSingleton<BackgroundLayerRenderer, BackgroundLayerRenderer>();

        }
    }
}
