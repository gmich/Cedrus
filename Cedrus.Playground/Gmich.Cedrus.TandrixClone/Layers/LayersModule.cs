using Gmich.Cedrus.IOC;
using System;

namespace Gmich.Cedrus.TandrixClone.Layers
{
    internal class LayersModule : CendrusModule
    {
        public override void Register(IocBuilder builder)
        {
            builder.RegisterSingleton<BackgroundLayerRenderer, BackgroundLayerRenderer>();
        }
    }
}
