using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core.Game_Objects.Controllers.Camera;

namespace Level_Editor.Editor_Components
{
    public class MouseWorldPositionBox : Box
    {
        public InputHandler Input { private get; set; }
        public CameraControllerBase Camera { private get; set; }

        public MouseWorldPositionBox()
            : base(new Rectangle(1375, 825, 200, 50))
        {
            Input = null;
            Camera = null;

            Visible = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if ((Input != null) && (Camera != null))
            {
                CaptionText = (Input.MousePosition + Camera.WorldPosition).ToString();
                base.Draw(spriteBatch);
            }
        }
    }
}
