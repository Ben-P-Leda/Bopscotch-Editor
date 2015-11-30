using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;
using Leda.Core.Asset_Management;

namespace Level_Editor.Objects.Terrain.Signposts
{
    public abstract class SignpostBase : TerrainObjectWithCollisionZone
    {
        public SignpostBase()
        {
            DimensionsInCells = new Point(1, 2);
            Visible = true;
            RenderLayer = 2;
            Frame = new Rectangle(0, 0, Definitions.CellSizeInPixels, Definitions.CellSizeInPixels + Plate_Vertical_Offset);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.Draw(TextureManager.Textures["post"], WorldPosition - CameraPosition, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, RenderDepth + 0.001f);
        }

        private const int Plate_Vertical_Offset = 10;
    }
}