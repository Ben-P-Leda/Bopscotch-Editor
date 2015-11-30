using System.Xml.Linq;

using Microsoft.Xna.Framework;

namespace Level_Editor.Objects.Terrain.Flags
{
    public class GoalFlag : FlagBase
    {
        public override XElement SaveNode { get { return new XElement(Save_Node_Name); } }

        public GoalFlag()
            : base()
        {
        }

        public const string Save_Node_Name = "goal-flag";
    }
}
