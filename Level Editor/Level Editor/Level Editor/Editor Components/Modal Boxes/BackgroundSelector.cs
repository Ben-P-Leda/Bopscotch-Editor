using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Leda.Core.Asset_Management;

namespace Level_Editor.Editor_Components.Modal_Boxes
{
    public class BackgroundSelector : SelectionBox
    {
        public override string Selection
        {
            get { return Data_Container.Data.Container.BackgroundTexture; }
            set { if (!string.IsNullOrEmpty(value)) { Data_Container.Data.Container.BackgroundTexture = value; } }
        }

        public BackgroundSelector()
            : base(new Rectangle(440, 150, 720, 600), "Select Background")
        {
        }

        public void CreateButtons()
        {
            int x = 450;
            int y = 200;

            foreach (string s in TextureManager.Textures.Keys)
            {
                if (s.StartsWith("background"))
                {
                    AddButton(new ImageButton(new Rectangle(x, y, 160, 90), s) { DepthBase = -0.005f });
                    x += 160 + Margin;
                    if (x > 1000) { x = 460; y += 90 + Margin; }
                }
            }
        }

        private const int Margin = 20;
    }
}
