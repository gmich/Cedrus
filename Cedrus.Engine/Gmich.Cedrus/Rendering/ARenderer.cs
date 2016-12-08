using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Gmich.Cedrus.Rendering
{

    public class RenderedComponentEventArgs : EventArgs
    {
        public RenderedComponentEventArgs(Identity identity, RenderedComponent component)
        {
            Identity = identity;
            Component = component;
        }
        public Identity Identity { get; }
        public RenderedComponent Component { get; }
    }

    public class RenderedTextEventArgs : EventArgs
    {
        public RenderedTextEventArgs(Identity identity, RenderedText text)
        {
            Identity = identity;
            Text = text;
        }
        public Identity Identity { get; }
        public RenderedText Text { get; }
    }

    public abstract class ARenderer
    {
        private readonly SpriteBatch batch;
        private readonly GraphicsDevice device;
        private readonly Func<Color> deviceDefaultColor;
        private readonly Dictionary<Identity, RenderedComponent> renderedComponents = new Dictionary<Identity, RenderedComponent>();
        private readonly Dictionary<Identity, RenderedText> renderedText = new Dictionary<Identity, RenderedText>();
        private readonly Action preRender;
        private readonly Action postRender;

        public ARenderer(GraphicsDevice device, Func<Color> deviceDefaultColor, Action preRender, Action postRender)
        {
            this.device = device;
            this.deviceDefaultColor = deviceDefaultColor;
            batch = new SpriteBatch(device);
            this.preRender = preRender;
            this.postRender = postRender;
        }

        public EventHandler<RenderedComponentEventArgs> ComponentAdded { get; set; }
        public EventHandler<RenderedComponentEventArgs> ComponentRemoved { get; set; }
        public EventHandler<RenderedTextEventArgs> TextAdded { get; set; }
        public EventHandler<RenderedTextEventArgs> TextRemoved { get; set; }

        public Result<IDisposable> AddComponent(Identity identity, RenderedComponent component)
        {
            if (renderedComponents.ContainsKey(identity))
            {
                return Result.FailWith<IDisposable>(State.Forbidden, $"TargetRenderer already contains component {identity.Id}.");
            }
            renderedComponents.Add(identity, component);
            ComponentAdded?.Invoke(this, new RenderedComponentEventArgs(identity, component));

            return Result.Ok(Disposable.For(() =>
            {
                if (renderedComponents.ContainsKey(identity))
                {
                    renderedComponents.Remove(identity);
                    ComponentRemoved?.Invoke(this, new RenderedComponentEventArgs(identity, component));
                }
            }));
        }

        public Result<IDisposable> AddText(Identity identity, RenderedText text)
        {
            if (renderedText.ContainsKey(identity))
            {
                return Result.FailWith<IDisposable>(State.Forbidden, $"TargetRenderer already contains text {identity.Id}.");
            }
            renderedText.Add(identity, text);
            TextAdded?.Invoke(this, new RenderedTextEventArgs(identity, text));

            return Result.Ok(Disposable.For(() =>
            {
                if (renderedText.ContainsKey(identity))
                {
                    renderedText.Remove(identity);
                    TextRemoved?.Invoke(this, new RenderedTextEventArgs(identity, text));
                }
            }));
        }

        public void Render(Matrix tranformationMatrix)
        {
            device.Clear(deviceDefaultColor());

            batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, tranformationMatrix);

            foreach (var item in renderedComponents.Values)
            {
                batch.Draw(item.Texture, item.Position, null, item.TextureRectangle, item.Origin, item.Rotation, item.Scale, item.Color, item.Effects, item.Layer);
            }

            foreach (var item in renderedText.Values)
            {
                batch.DrawString(item.SpriteFont, item.Text, item.Position, item.Color, item.Rotation, item.Origin, item.Scale, item.Effects, item.Layer);
            }

            batch.End();
        }
    }
}