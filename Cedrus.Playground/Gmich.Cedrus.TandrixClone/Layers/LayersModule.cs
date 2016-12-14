using Gmich.Cedrus.IOC;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gmich.Cedrus.TandrixClone.Layers
{
    internal class LayersModule : CendrusModule
    {
        public override void Register(IocBuilder builder)
        {
            builder.RegisterSingleton<BackgroundLayerRenderer>(c =>
                new BackgroundLayerRenderer(c.Resolve<GraphicsDevice>(), null, () => Color.White));
        }
    }
}
