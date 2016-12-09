using Gmich.Cedrus.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gmich.Cedrus.TandrixClone.Layers
{

    internal class LayerContent : AssetContainerBuilder<Texture2D>
    {

    }

    internal class Graphics
    {
        public static string BackgroundPattern = "BackgroundPattern";
        public static string BackgroundMask = "BackgroundMask";


        public Graphics(LayerContent layerContent)
        { }
    }
}
