using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Gmich.Cedrus.Rendering
{
    public class TargetRenderer
    {
        private readonly SpriteBatch batch;
        private readonly GraphicsDevice device;
        private readonly Func<Color> deviceDefaultColor;

        public RenderTarget2D RenderTarget { get; }
        public Dictionary<Identity, RenderedObject> RenderMap { get; } = new Dictionary<Identity, RenderedObject>();

        public TargetRenderer(GraphicsDevice device, Point targetsize,Func<Color> deviceDefaultColor)
        {
            this.device = device;
            this.deviceDefaultColor = deviceDefaultColor;
            batch = new SpriteBatch(device);
            var pp = new PresentationParameters();
            RenderTarget = new RenderTarget2D(device,
                targetsize.X,
                targetsize.Y,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                pp.MultiSampleCount,
                RenderTargetUsage.DiscardContents);
        }

        public void Render(Matrix tranformationMatrix)
        {
            device.SetRenderTarget(RenderTarget);

            device.Clear(deviceDefaultColor());

            batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, tranformationMatrix);

            foreach (var item in RenderMap.Values)
            {
                batch.Draw(item.Texture, item.Position, null, item.TextureRectangle, item.Origin, item.Rotation, item.Scale, item.Color, item.Effects, item.Layer);
            }

            batch.End();

            device.SetRenderTarget(null);
        }
    }
}