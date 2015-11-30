using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;
using Leda.Core.Game_Objects.Base_Classes;
using Leda.Core.Game_Objects.Behaviours;
using Leda.Core.Asset_Management;
using Leda.Core.Motion;

namespace Level_Editor.Objects.Terrain
{
    public abstract class TerrainObjectWithCollisionZone : TerrainObjectBase
    {
        public float CollisionZoneTopOffset { get; set; }

        public TerrainObjectWithCollisionZone()
            : base()
        {
            CollisionZoneTopOffset = 0.0f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (Selected)
            {
                RenderTools.Line(
                    spriteBatch,
                    TextureManager.Textures["pixel"],
                    (WorldPosition + new Vector2(0.0f, CollisionZoneTopOffset)) - CameraPosition,
                    (WorldPosition + new Vector2(Frame.Width, CollisionZoneTopOffset)) - CameraPosition,
                    3.0f,
                    Color.Crimson,
                    0.001f);

                RenderTools.Line(
                    spriteBatch,
                    TextureManager.Textures["pixel"],
                    (WorldPosition + new Vector2(0.0f, DimensionsInCells.Y * Definitions.CellSizeInPixels)) - CameraPosition,
                    (WorldPosition + new Vector2(Frame.Width, DimensionsInCells.Y * Definitions.CellSizeInPixels)) - CameraPosition,
                    3.0f,
                    Color.Crimson,
                    0.001f);

                RenderTools.Line(
                    spriteBatch,
                    TextureManager.Textures["pixel"],
                    (WorldPosition + new Vector2(Frame.Width / 2.0f, CollisionZoneTopOffset)) - CameraPosition,
                    (WorldPosition + new Vector2(Frame.Width / 2.0f, DimensionsInCells.Y * Definitions.CellSizeInPixels)) - CameraPosition,
                    3.0f,
                    Color.Crimson,
                    0.001f);
            }
        }


        public override XElement Save()
        {
            XElement node = base.Save();
            node.Add(new XAttribute("zone-top", CollisionZoneTopOffset));

            return node;
        }
    }
}
