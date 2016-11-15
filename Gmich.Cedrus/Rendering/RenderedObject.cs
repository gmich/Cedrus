using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gmich.Cedrus.Rendering
{
    public sealed class RenderedObject
    {
        private readonly Func<Texture2D> texture;
        private readonly Func<Vector2> position;
        private readonly Func<Color> color;
        private readonly Func<float> rotation;
        private readonly Func<Vector2> scale;
        private readonly Func<Vector2> origin;
        private readonly Func<SpriteEffects> effects;
        private readonly Func<float> layer;
        private readonly Func<Rectangle> textureRectangle;

        public RenderedObject(Func<Texture2D> texture, Func<Rectangle> textureRectangle, Func<Vector2> position, Func<Color> color, Func<float> rotation, Func<Vector2> scale, Func<Vector2> origin, Func<SpriteEffects> effects, Func<float> layer)
        {
            this.texture = texture;
            this.textureRectangle = textureRectangle;
            this.position = position;
            this.color = color;
            this.rotation = rotation;
            this.scale = scale;
            this.origin = origin;
            this.effects = effects;
            this.layer = layer;
        }

        public Texture2D Texture => texture();
        public Rectangle TextureRectangle => textureRectangle();
        public Vector2 Position => position();
        public Color Color => color();
        public float Rotation => rotation();
        public Vector2 Scale => scale();
        public Vector2 Origin => origin();
        public SpriteEffects Effects => effects();
        public float Layer => layer();

    }
}
