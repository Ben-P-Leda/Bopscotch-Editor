using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Leda.Core.Asset_Management;

namespace Level_Editor.Editor_Components.Modal_Boxes
{
    public class EnemySelector : SelectionBox
    {
        public EnemySelector()
            : base(new Rectangle(500, 150, 600, 600), "Select Enemy")
        {
        }

        public void CreateButtons()
        {
            int x = 510;
            int y = 200;

            foreach (string s in TextureManager.Textures.Keys)
            {
                if (s.StartsWith("enemy-"))
                {
                    AddButton(new ImageButton(new Rectangle(x, y, Enemy_Texture_Width, Enemy_Texture_Height), s) { DepthBase = -0.005f });
                    x += Definitions.CellSizeInPixels + Margin;
                    if (x > 1000) { x = 510; y += Definitions.CellSizeInPixels + Margin; }
                }
            }
        }

        private const int Enemy_Texture_Width = 160;
        private const int Enemy_Texture_Height = 110;
        private const int Margin = 20;
    }
}
