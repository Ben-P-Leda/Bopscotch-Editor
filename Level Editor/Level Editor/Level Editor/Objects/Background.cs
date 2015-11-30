using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core.Game_Objects.Behaviours;
using Leda.Core.Asset_Management;

namespace Level_Editor.Objects
{
    public class Background : ISimpleRenderable
    {
        public bool Visible { get { return true; } set { } }
        public int RenderLayer { get { return 0; } set { } }

        public void Initialize()
        {
        }

        public void Reset()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if ((Visible) && (!string.IsNullOrEmpty(Data_Container.Data.Container.BackgroundTexture)))
            {
                spriteBatch.Draw(
                    TextureManager.Textures[Data_Container.Data.Container.BackgroundTexture],
                    Vector2.Zero,
                    null,
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    1.0f,
                    SpriteEffects.None,
                    0.9f);
            }
        }
    }
}
