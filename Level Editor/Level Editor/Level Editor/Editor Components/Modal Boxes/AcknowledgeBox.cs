using Microsoft.Xna.Framework;

namespace Level_Editor.Editor_Components.Modal_Boxes
{
    public class AcknowledgeBox : ButtonBox
    {
        public AcknowledgeBox(string caption)
            : base(new Rectangle(700, 400, 200, 100), caption)
        {
            AddButton(new Button(new Rectangle(790, 450, 100, 40), "OK") { DepthBase = -0.005f });
        }
    }
}
