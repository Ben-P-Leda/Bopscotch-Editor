﻿using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Leda.Core.Asset_Management;

namespace Level_Editor.Editor_Components.Modal_Boxes
{
    public class BlockSelector : SelectionBox
    {
        public BlockSelector()
            : base(new Rectangle(500, 150, 600, 600), "Select Block")
        {
        }

        public void CreateButtons()
        {
            int x = 510;
            int y = 200;

            foreach (string s in TextureManager.Textures.Keys)
            {
                if (s.StartsWith("block"))
                {
                    AddButton(new ImageButton(new Rectangle(x, y, Definitions.CellSizeInPixels, Definitions.CellSizeInPixels), s) { DepthBase = -0.005f });
                    x += Definitions.CellSizeInPixels + Margin;
                    if (x > 1000) { x = 510; y += Definitions.CellSizeInPixels + Margin; }
                }
            }
        }

        private const int Margin = 20;
    }
}
