using Microsoft.Xna.Framework;

using Leda.Core.Motion;

namespace Level_Editor.Objects
{
    public class PositionRestoreMotionEngine : IMotionEngine
    {
        public Vector2 Delta { get; set; }
        public ObjectBase ObjectToMove { private get; set; }
        public Vector2 Target { private get; set; }

        public void Update(int millisecondsSinceLastUpdate)
        {
            Delta = Vector2.Normalize(Target - ObjectToMove.WorldPosition) * millisecondsSinceLastUpdate * Speed;
        }

        private const float Speed = 2.5f;
    }
}
