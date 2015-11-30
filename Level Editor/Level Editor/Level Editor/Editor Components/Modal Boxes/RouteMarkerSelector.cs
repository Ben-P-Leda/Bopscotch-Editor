using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Leda.Core.Asset_Management;

namespace Level_Editor.Editor_Components.Modal_Boxes
{
    public class RouteMarkerSelector : SelectionBox
    {
        public RouteMarkerSelector()
            : base(new Rectangle(500, 150, 600, 200), "Select Marker")
        {
        }

        public void CreateButtons()
        {
            AddButton(new ImageButton(new Rectangle(510, 200, 160, 160), "straight-arrow") { DepthBase = -0.005f });
            AddButton(new ImageButton(new Rectangle(710, 200, 160, 160), "up-curved-arrow") { DepthBase = -0.005f });
            AddButton(new ImageButton(new Rectangle(910, 200, 160, 160), "down-curved-arrow") { DepthBase = -0.005f });
        }

        private const int Margin = 20;
    }
}
