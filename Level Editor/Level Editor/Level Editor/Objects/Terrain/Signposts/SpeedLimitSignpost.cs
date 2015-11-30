using System.Xml.Linq;

using Microsoft.Xna.Framework;

using Leda.Core;
using Leda.Core.Asset_Management;

namespace Level_Editor.Objects.Terrain.Signposts
{
    public class SpeedLimitSignpost : SignpostBase
    {
        private Range _speedLimits;

        public override XElement SaveNode { get { return new XElement(Save_Node_Name); } }
        public Range SpeedLimits { set { _speedLimits = value; } }

        public SpeedLimitSignpost()
            : base()
        {
            _speedLimits = new Range(0, 0);
        }

        public void UpdateRange()
        {
            _speedLimits.Minimum++;
            if (_speedLimits.Minimum > _speedLimits.Maximum)
            {
                _speedLimits.Maximum = (_speedLimits.Maximum + 1) % 4;
                _speedLimits.Minimum = 0;
            }

            TextureReference = string.Concat("sign-speed-", _speedLimits.Minimum + 1, "-", _speedLimits.Maximum + 1);
            Texture = TextureManager.Textures[TextureReference];
        }

        public override XElement Save()
        {
            XElement node = base.Save();
            node.Add(new XAttribute("minimum-speed", _speedLimits.Minimum));
            node.Add(new XAttribute("maximum-speed", _speedLimits.Maximum));

            return node;
        }

        public const string Save_Node_Name = "speed-limit-signpost";
    }
}
