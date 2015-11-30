using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;
using Leda.Core.Asset_Management;

namespace Level_Editor.Editor_Components
{
    public class QuantitySpinner : ImageButton
    {
        private Rectangle _spinnerFrame;
        private Rectangle _spinnerInner;
        private Vector2 _textPosition;
        private bool _active;

        public bool Active { get { return _active; } set { _active = value; SetTint(); } }
        public int UnitCount { get; set; }

        public QuantitySpinner(Rectangle imageFrame, string textureReference)
            : base(imageFrame, textureReference)
        {
            _spinnerFrame = new Rectangle(imageFrame.X, imageFrame.Y, Total_Width, imageFrame.Height);
            _spinnerInner = new Rectangle(imageFrame.X + 1, imageFrame.Y + 1, Total_Width - 2, imageFrame.Height - 2);
            _textPosition = new Vector2(imageFrame.X + (imageFrame.Width * 1.5f), imageFrame.Y);

            Active = false;
            UnitCount = 0;
        }

        private void SetTint()
        {
            if (_active) { _tint = Color.White; }
            else { _tint = Color.Lerp(Color.White, Color.Transparent, 0.5f); }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(TextureManager.Textures["pixel"], _spinnerFrame, null, _tint, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0113f);
                spriteBatch.Draw(TextureManager.Textures["pixel"], _spinnerInner, null, Color.Black, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0112f);

                TextWriter.Write(string.Concat("x", UnitCount), spriteBatch, _textPosition, _tint, 0.75f, 0.0111f, TextWriter.Alignment.Left);

                base.Draw(spriteBatch);
            }
        }

        public override bool Contains(Vector2 screenPosition)
        {
            return _spinnerFrame.Contains((int)screenPosition.X, (int)screenPosition.Y);
        }

        public const int Total_Width = 240;
    }
}
