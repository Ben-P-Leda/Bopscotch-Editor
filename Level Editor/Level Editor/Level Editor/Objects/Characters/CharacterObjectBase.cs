using System.Xml.Linq;

using Microsoft.Xna.Framework;

using Leda.Core.Asset_Management;

namespace Level_Editor.Objects.Characters
{
    public class CharacterObjectBase : ObjectBase
    {
        public string GroupNodeName { get; set; }

        public override XElement SaveNode { get { return new XElement("player"); } }
        public bool StartsLevelMovingLeft { get { return Mirror; } set { Mirror = value; } }

        public CharacterObjectBase()
        {
            Visible = false;
            RenderLayer = 2;
            StartsLevelMovingLeft = false;
        }

        public override XElement Save()
        {
            XElement node = base.Save();
            node.Add(new XAttribute("startfacingleft", Mirror));

            return node;
        }

        public void Load(XElement data)
        {
            WorldPosition = new Vector2((float)data.Attribute("x"), (float)data.Attribute("y"));
            Visible = true;
            StartsLevelMovingLeft = (bool)data.Attribute("startfacingleft");
        }
    }
}
