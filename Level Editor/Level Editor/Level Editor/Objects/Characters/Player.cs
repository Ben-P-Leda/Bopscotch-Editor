using System.Xml.Linq;

using Microsoft.Xna.Framework;

using Leda.Core.Asset_Management;

namespace Level_Editor.Objects.Characters
{
    public class Player : CharacterObjectBase
    {
        public Player()
        {
            DimensionsInCells = new Point(1, 1);
            Frame = new Rectangle(0, 0, Definitions.CellSizeInPixels, Definitions.CellSizeInPixels);
            Texture = TextureManager.Textures["player"];
        }
    }
}
