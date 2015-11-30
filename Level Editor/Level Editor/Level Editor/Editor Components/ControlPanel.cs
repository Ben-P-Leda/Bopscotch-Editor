using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;

namespace Level_Editor.Editor_Components
{
    public class ControlPanel : ButtonBox
    {
        public string ActiveOption { get; set; }

        public ControlPanel()
            : base(new Rectangle(100, 20, 1400, 50))
        {
            int x = 110;

            foreach (string s in ButtonList.Split(','))
            {
                AddButton(new Button(new Rectangle(x, 25, ButtonWidth, 40), s) { DepthBase = -0.005f });
                x += ButtonWidth + ButtonMargin;
            }

            Visible = true;

            ActiveOption = "";
        }

        public void UpdateSelectedOption(Vector2 mouseScreenPosition)
        {
            ActiveOption = ButtonAtScreenPosition(mouseScreenPosition, Color.Red);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (Data_Container.Data.Container.RaceLapCount > 0)
            {
                TextWriter.Write(string.Concat(Data_Container.Data.Container.RaceLapCount, " lap race"), spriteBatch, new Vector2(1480.0f, 30.0f),
                    Color.White, 0.1f, TextWriter.Alignment.Right);
            }
            else
            {
                TextWriter.Write("Adventure level", spriteBatch, new Vector2(1480.0f, 30.0f), Color.White, 0.1f, TextWriter.Alignment.Right);
            }
        }

        private const string ButtonList = "Race,BG,Player,Blocks,Candies,Signs,Flags,Enemies,Route";
        private const int ButtonWidth = 100;
        private const int ButtonMargin = 10;
    }
}
