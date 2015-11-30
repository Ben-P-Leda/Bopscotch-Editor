using Microsoft.Xna.Framework;

namespace Level_Editor.Editor_Components.Modal_Boxes
{
    public class ConfirmBox : ButtonBox
    {
        public ConfirmBox(string caption)
            : base(new Rectangle(650, 350, 300, 200), caption)
        {
            AddButton(new Button(new Rectangle(730, 500, 100, 40), "Yes") { DepthBase = -0.005f });
            AddButton(new Button(new Rectangle(840, 500, 100, 40), "No") { DepthBase = -0.005f });
        }
    }
}
