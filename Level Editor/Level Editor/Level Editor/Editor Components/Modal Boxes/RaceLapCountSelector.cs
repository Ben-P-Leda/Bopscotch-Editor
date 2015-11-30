using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;

using Level_Editor.Data_Container;

namespace Level_Editor.Editor_Components.Modal_Boxes
{
    public class RaceLapCountSelector : ButtonBox
    {
        public int LapCount { get; private set; }
        public override bool Visible { get { return base.Visible; } set { base.Visible = value; LapCount = Data.Container.RaceLapCount; } }

        public RaceLapCountSelector()
            : base(new Rectangle(600, 350, 400, 200), "Select Lap Count")
        {
            AddButton(new Button(new Rectangle(610, 500, 100, 40), "Clear") { DepthBase = -0.005f });
            AddButton(new Button(new Rectangle(780, 500, 100, 40), "Save") { DepthBase = -0.005f });
            AddButton(new Button(new Rectangle(890, 500, 100, 40), "Cancel") { DepthBase = -0.005f });
        }

        public void UpdateLapCount(int delta)
        {
            if (((delta < 0) && (LapCount > 0)) || ((delta > 0) && (LapCount < 5))) { LapCount += delta; }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            TextWriter.Write(string.Concat(LapCount, " laps"), spriteBatch, new Vector2(800.0f, 375.0f), Color.White, 0.75f, 0.1f, TextWriter.Alignment.Center);
        }
    }
}
