using System.Xml.Linq;

using Microsoft.Xna.Framework;

namespace Level_Editor.Objects.Terrain.Signposts
{
    public class OneWaySignpost : SignpostBase
    {
        public override XElement SaveNode { get { return new XElement(Save_Node_Name); } }

        public OneWaySignpost()
            : base()
        {
        }

        public override XElement Save()
        {
            XElement node = base.Save();
            node.Add(new XAttribute("mirror", Mirror));

            return node;
        }

        public const string Save_Node_Name = "one-way-signpost";
    }
}
