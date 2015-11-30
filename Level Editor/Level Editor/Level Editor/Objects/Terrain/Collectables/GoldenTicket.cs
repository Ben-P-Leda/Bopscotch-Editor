using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;

namespace Level_Editor.Objects.Terrain.Collectables
{
    public class GoldenTicket : Collectable
    {
        public override XElement SaveNode { get { return new XElement(Save_Node_Name); } }

        public GoldenTicket()
        {
            TextureReference = "golden-ticket";
        }

        public const string Save_Node_Name = "golden-ticket";
    }
}
