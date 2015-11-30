using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;
using Leda.Core.Asset_Management;

namespace Level_Editor.Objects.Terrain
{
    public abstract class TerrainObjectBase : ObjectBase
    {
        public string GroupNodeName { get; set; }

        public TerrainObjectBase()
            : base()
        {
        }

        public override XElement Save()
        {
            XElement node = base.Save();
            node.Add(new XAttribute("texture", TextureReference));

            return node;
        }
    }
}
