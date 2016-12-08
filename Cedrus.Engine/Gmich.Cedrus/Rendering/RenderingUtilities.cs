using Microsoft.Xna.Framework.Graphics;

namespace Gmich.Cedrus.Rendering
{
    public static class RenderingUtilities
    {
        public static RenderTarget2D CreateRenderTarget(this GraphicsDevice device, int sizeX, int sizeY)
        {
            var pp = new PresentationParameters();
            return new RenderTarget2D(device,
                sizeX,
                sizeY,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                pp.MultiSampleCount,
                RenderTargetUsage.DiscardContents);
        }
    }
}
