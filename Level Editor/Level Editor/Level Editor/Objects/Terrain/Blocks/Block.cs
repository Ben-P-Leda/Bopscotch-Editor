using System.Xml.Linq;

using Microsoft.Xna.Framework;

namespace Level_Editor.Objects.Terrain.Blocks
{
    public class Block : TerrainObjectBase
    {
        public override XElement SaveNode { get { return new XElement(Save_Node_Name); } }

        public Block()
        {
            DimensionsInCells = new Point(1, 1);
            Visible = true;
            RenderLayer = 2;
            Frame = new Rectangle(0, 0, Definitions.CellSizeInPixels, Definitions.CellSizeInPixels);
        }

        public const string Save_Node_Name = "block";
    }
}
