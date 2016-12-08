using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gmich.Cedrus.Rendering
{
    public sealed class RenderedText
    {
        private readonly Func<SpriteFont> font;
        private readonly Func<string> text;
        private readonly Func<Vector2> position;
        private readonly Func<Color> color;
        private readonly Func<float> rotation;
        private readonly Func<Vector2> scale;
        private readonly Func<Vector2> origin;
        private readonly Func<SpriteEffects> effects;
        private readonly Func<float> layer;

        public RenderedText(Func<SpriteFont> font, Func<string> text, Func<Vector2> position, Func<Color> color, Func<float> rotation, Func<Vector2> scale, Func<Vector2> origin, Func<SpriteEffects> effects, Func<float> layer)
        {
            this.font = font;
            this.text = text;
            this.position = position;
            this.color = color;
            this.rotation = rotation;
            this.scale = scale;
            this.origin = origin;
            this.effects = effects;
            this.layer = layer;
        }

        public SpriteFont SpriteFont => font();
        public string Text => text();
        public Vector2 Position => position();
        public Color Color => color();
        public float Rotation => rotation();
        public Vector2 Scale => scale();
        public Vector2 Origin => origin();
        public SpriteEffects Effects => effects();
        public float Layer => layer();

    }
}
