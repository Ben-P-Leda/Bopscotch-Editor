using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;
using Leda.Core.Asset_Management;

namespace Level_Editor.Objects.Terrain.Flags
{
    public abstract class FlagBase : TerrainObjectWithCollisionZone
    {
        public string FlagTextureName { get; set; }
        public bool ActivatedWhenMovingLeft { get; set; }

        public FlagBase()
        {
            DimensionsInCells = new Point(1, 3);
            Visible = true;
            RenderLayer = 2;
            RenderDepth = 0.6f;
            Texture = TextureManager.Textures[Flagpole_Texture_Name];
            Frame = new Rectangle(0, 0, Definitions.CellSizeInPixels, Definitions.CellSizeInPixels * DimensionsInCells.Y);

            ActivatedWhenMovingLeft = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Draw(
                TextureManager.Textures[FlagTextureName],
                (WorldPosition + new Vector2(Definitions.CellSizeInPixels / 2.0f, Flag_Vertical_Offset)) - CameraPosition,
                null,
                Color.White,
                0.0f,
                Vector2.Zero,
                1.0f,
                SpriteEffects.None,
                RenderDepth + 0.001f);

            spriteBatch.Draw(
                TextureManager.Textures[Arrow_Texture_Name],
                (WorldPosition + new Vector2(Definitions.CellSizeInPixels / 2.0f, Arrow_Vertical_Offset)) - CameraPosition,
                null,
                Color.White,
                0.0f,
                new Vector2(TextureManager.Textures[Arrow_Texture_Name].Width / 2.0f, 0.0f),
                1.0f,
                (ActivatedWhenMovingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None),
                RenderDepth - 0.001f);
        }

        public override XElement Save()
        {
            XElement node = base.Save();
            node.Attribute("texture").Value = FlagTextureName;
            node.Add(new XAttribute("left-activation", ActivatedWhenMovingLeft));

            return node;
        }

        private const string Flagpole_Texture_Name = "flagpole";
        private const int Flag_Vertical_Offset = 73;

        private const string Arrow_Texture_Name = "arrow";
        private const int Arrow_Vertical_Offset = 200;
    }
}