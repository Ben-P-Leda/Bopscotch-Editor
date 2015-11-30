using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core.Game_Objects.Controllers.Camera;

namespace Level_Editor.Editor_Components
{
    public class ScreensPositionBox : Box
    {
        public InputHandler Input { private get; set; }
        public CameraControllerBase Camera { private get; set;}

        public ScreensPositionBox()
            : base(new Rectangle(125, 825, 200, 50))
        {
            Camera = null;

            Visible = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Camera != null)
            {
                CaptionText = string.Concat((int)(Camera.WorldPosition.X / 1600), ":", (int)(Camera.WorldPosition.Y / 1600));
                base.Draw(spriteBatch);
            }
        }
    }
}
