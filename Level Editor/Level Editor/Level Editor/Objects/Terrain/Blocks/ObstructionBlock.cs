using System.Xml.Linq;

using Microsoft.Xna.Framework;

namespace Level_Editor.Objects.Terrain.Blocks
{
    public class ObstructionBlock : Block
    {
        public override XElement SaveNode { get { return new XElement(Save_Node_Name); } }

        public ObstructionBlock()
            : base()
        {
        }

        public new const string Save_Node_Name = "obstruction-block";
    }
}