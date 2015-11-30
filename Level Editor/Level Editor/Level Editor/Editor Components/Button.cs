using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;
using Leda.Core.Game_Objects.Behaviours;
using Leda.Core.Asset_Management;

namespace Level_Editor.Editor_Components
{
    public class Button : Box
    {
        public Button(Rectangle frame, string caption)
            : base(frame, caption)
        {
        }
    }
}
