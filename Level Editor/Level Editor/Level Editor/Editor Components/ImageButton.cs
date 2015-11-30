using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core.Asset_Management;

namespace Level_Editor.Editor_Components
{
    public class ImageButton : Button
    {
        private Rectangle _imageFrame;
        private string _textureReference;

        protected Color _tint;

        public ImageButton(Rectangle frame, string textureReference)
            : base(frame, textureReference)
        {
            _imageFrame = frame;
            _textureReference = textureReference;
            _tint = Color.White;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(TextureManager.Textures[_textureReference], _imageFrame, null, _tint, 0.0f, Vector2.Zero, SpriteEffects.None, 0.011f);
            }
        }
    }
}
