using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;
using Leda.Core.Game_Objects.Behaviours;

namespace Leda.Core.Renderable
{
    public class Box : ISimpleRenderable
    {
        public Vector2 Position { get; set; }
        public virtual Point Dimensions { get; set; }
        public Texture2D EdgeTexture { private get; set; }
        public Texture2D CornerTexture { private get; set; }
        public virtual Color EdgeTint { protected get; set; }
        public Texture2D BackgroundTexture { private get; set; }
        public virtual Color BackgroundTint { protected get; set; }

        public int RenderLayer { get; set; }
        public virtual bool Visible { get; set; }
        public float RenderDepth { get; set; }

        public Box()
        {
            Position = Vector2.Zero;
            Dimensions = Point.Zero;
            EdgeTexture = null;
            CornerTexture = null;
            EdgeTint = Color.White;
            BackgroundTexture = null;
            BackgroundTint = Color.Black;

            RenderLayer = -1;
            Visible = false;
            RenderDepth = 0.5f;
        }

        public virtual void Initialize()
        {
        }

        public virtual void Reset()
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            float left = Position.X + (EdgeTexture.Height / 2.0f);
            float top = Position.Y + (EdgeTexture.Height / 2.0f);
            float right = left + Dimensions.X - EdgeTexture.Height;
            float bottom = top + Dimensions.Y - EdgeTexture.Height;

            RenderTools.Line(spriteBatch, EdgeTexture, new Vector2(left, top), new Vector2(right, top), 1.0f, EdgeTint, RenderDepth);
            RenderTools.Line(spriteBatch, EdgeTexture, new Vector2(left, top), new Vector2(left, bottom), 1.0f, EdgeTint, RenderDepth);
            RenderTools.Line(spriteBatch, EdgeTexture, new Vector2(left, bottom), new Vector2(right, bottom), 1.0f, EdgeTint, RenderDepth);
            RenderTools.Line(spriteBatch, EdgeTexture, new Vector2(right, top), new Vector2(right, bottom), 1.0f, EdgeTint, RenderDepth);

            if (CornerTexture != null)
            {
                spriteBatch.Draw(CornerTexture, new Vector2(left, top), null, EdgeTint, 0.0f, new Vector2(CornerTexture.Width, CornerTexture.Height) / 2.0f, 1.0f, SpriteEffects.None, RenderDepth - 0.005f);
                spriteBatch.Draw(CornerTexture, new Vector2(right, top), null, EdgeTint, MathHelper.PiOver2, new Vector2(CornerTexture.Width, CornerTexture.Height) / 2.0f, 1.0f, SpriteEffects.None, RenderDepth - 0.005f);
                spriteBatch.Draw(CornerTexture, new Vector2(right, bottom), null, EdgeTint, MathHelper.Pi, new Vector2(CornerTexture.Width, CornerTexture.Height) / 2.0f, 1.0f, SpriteEffects.None, RenderDepth - 0.005f);
                spriteBatch.Draw(CornerTexture, new Vector2(left, bottom), null, EdgeTint, MathHelper.Pi * 1.5f, new Vector2(CornerTexture.Width, CornerTexture.Height) / 2.0f, 1.0f, SpriteEffects.None, RenderDepth - 0.005f);
            }

            if (BackgroundTexture != null)
            {
                Rectangle area = new Rectangle((int)left, (int)top, (int)(right - left),(int)(bottom - top));
                spriteBatch.Draw(BackgroundTexture, area, null, BackgroundTint, 0.0f, Vector2.Zero, SpriteEffects.None, RenderDepth + 0.005f);
            }
        }
    }
}
