using System.Xml.Linq;

using Microsoft.Xna.Framework;

namespace Level_Editor.Objects.Terrain.Blocks
{
    public class BombBlock : Block
    {
        public override XElement SaveNode { get { return new XElement(Save_Node_Name); } }

        public BombBlock()
            : base()
        {
        }

        public new const string Save_Node_Name = "bomb-block";
    }
}