using Microsoft.Xna.Framework;

namespace Level_Editor.Editor_Components
{
    public abstract class SelectionBox : ButtonBox
    {
        public virtual string Selection { set; get; }

        public SelectionBox(Rectangle frame, string caption)
            : base(frame, caption)
        {
            Selection = "";
        }
    }
}
