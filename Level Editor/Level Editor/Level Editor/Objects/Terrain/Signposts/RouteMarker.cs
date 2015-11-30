using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;
using Leda.Core.Asset_Management;

namespace Level_Editor.Objects.Terrain.Signposts
{
    public class RouteMarker : TerrainObjectBase
    {
        private Texture2D _texture;
        private int _quadrant;

        public override XElement SaveNode { get { return new XElement(Save_Node_Name); } }
        public int Quadrant { set { _quadrant = value; Rotation = MathHelper.PiOver2 * value; } }

        public RouteMarker()
        {
            DimensionsInCells = new Point(2, 2);
            Visible = true;
            RenderLayer = 2;
            Quadrant = 0;
        }

        public void SetTexture(string textureName)
        {
            TextureReference = textureName;
            _texture = TextureManager.Textures[textureName];
            Frame = new Rectangle(0, 0, _texture.Width, _texture.Height);
            Origin = new Vector2(Definitions.CellSizeInPixels);
        }

        public void Rotate()
        {
            Quadrant = (_quadrant + 1) % 4;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, (WorldPosition - CameraPosition) + new Vector2(Definitions.CellSizeInPixels), Frame, Tint, Rotation, Origin, Scale, SpriteEffects.None, RenderDepth);
            DrawFrame(spriteBatch);
        }

        public override XElement Save()
        {
            XElement node = base.Save();
            node.Add(new XAttribute("quadrant", _quadrant));

            return node;
        }

        public const string Save_Node_Name = "marker";
    }
}
