using Microsoft.Xna.Framework;

namespace Level_Editor.Objects.Terrain.Collectables
{
    public abstract class Collectable : TerrainObjectBase
    {
        public Collectable()
        {
            DimensionsInCells = new Point(1, 1);
            Visible = true;
            RenderLayer = 2;
            Frame = new Rectangle(0, 0, Definitions.CellSizeInPixels, Definitions.CellSizeInPixels);
        }
    }
}
