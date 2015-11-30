using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Level_Editor.Objects.Terrain.Blocks
{
    public class SmashBlock : Block
    {
        public Dictionary<string, int> Contents { get; private set; }

        public override XElement SaveNode { get { return new XElement(Save_Node_Name); } }

        public SmashBlock()
            : base()
        {
            Contents = new Dictionary<string, int>();
        }

        public override XElement Save()
        {
            XElement node = base.Save();

            foreach (KeyValuePair<string, int> kvp in Contents)
            {
                XElement contentItem = new XElement("contains-item");
                contentItem.Add(new XAttribute("texture", kvp.Key));
                contentItem.Add(new XAttribute("units", kvp.Value));
                contentItem.Add(new XAttribute("action", GetActionAttribute(kvp.Key)));
                contentItem.Add(new XAttribute("value", GetValueAttribute(kvp.Key)));

                node.Add(contentItem);
            }

            return node;
        }

        private string GetActionAttribute(string itemTextureName)
        {
            switch (itemTextureName)
            {
                case "golden-ticket": return "add-ticket"; break;
                default: return "score"; break;
            }
        }

        private string GetValueAttribute(string ItemTextureName)
        {
            switch (ItemTextureName)
            {
                case "golden-ticket": return "1"; break;
                case "candy-1": return "25"; break;
                case "candy-2": return "50"; break;
                case "candy-3": return "75"; break;
                case "candy-4": return "100"; break;
                case "candy-5": return "150"; break;
            }

            return "0";
        }

        public void LoadContents(XElement node)
        {
            List<XElement> contents = (from el in node.Elements("contains-item") select el).ToList();
            for (int i = 0; i < contents.Count; i++)
            {
                if (!Contents.ContainsKey(contents[i].Attribute("texture").Value)) { Contents.Add(contents[i].Attribute("texture").Value, 0); }
                Contents[contents[i].Attribute("texture").Value] = (int)contents[i].Attribute("units");
            }
        }

        public new const string Save_Node_Name = "smash-block";
    }
}

