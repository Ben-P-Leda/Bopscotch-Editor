using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;
using Leda.Core.Game_Objects.Behaviours;
using Leda.Core.Asset_Management;

namespace Level_Editor.Editor_Components
{
    public class Box : ISimpleRenderable
    {
        private Rectangle _frame;

        public virtual bool Visible { get; set; }
        public int RenderLayer { get; set; }
        public float DepthBase { private get; set; }
        public Color Tint { private get; set; }

        public string CaptionText { get; set; }

        public Box(Rectangle frame)
            : this(frame, "")
        {
        }

        public Box(Rectangle frame, string captionText)
        {
            _frame = frame;
            CaptionText = captionText;

            Visible = false;
            RenderLayer = 4;
            Tint = Color.White;
        }

        public void Initialize()
        {
        }

        public void Reset()
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(TextureManager.Textures["pixel"], _frame, null, Tint, 0.0f, Vector2.Zero, SpriteEffects.None, DepthBase + 0.12f);
                spriteBatch.Draw(TextureManager.Textures["pixel"], new Rectangle(_frame.X + 1, _frame.Y + 1, _frame.Width - 2, _frame.Height - 2), null, Color.Black, 0.0f, Vector2.Zero, SpriteEffects.None, DepthBase + 0.119f);

                if (!string.IsNullOrEmpty(CaptionText))
                {
                    TextWriter.Write(CaptionText, spriteBatch, new Vector2(_frame.X + (_frame.Width / 2.0f), _frame.Y + 5.0f), Tint, DepthBase + 0.118f, TextWriter.Alignment.Center);
                }
            }
        }

        public virtual bool Contains(Vector2 screenPosition)
        {
            return _frame.Contains((int)screenPosition.X, (int)screenPosition.Y);
        }
    }
}
