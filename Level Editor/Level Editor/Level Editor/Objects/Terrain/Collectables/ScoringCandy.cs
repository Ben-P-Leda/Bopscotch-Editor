using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;

namespace Level_Editor.Objects.Terrain.Collectables
{
    public class ScoringCandy : Collectable
    {
        public override XElement SaveNode { get { return new XElement(Save_Node_Name); } }
        public int ScoreValue { get; set; }

        public ScoringCandy()
            : base()
        {
            ScoreValue = Default_Score_Value;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            TextWriter.Write(ScoreValue.ToString(), spriteBatch, WorldPosition - CameraPosition, Color.White, 0.1f, TextWriter.Alignment.Left);
        }

        public override XElement Save()
        {
            XElement node = base.Save();
            node.Add(new XAttribute("score", ScoreValue));

            return node;
        }

        private const int Default_Score_Value = 25;

        public const string Save_Node_Name = "scoring-candy";
    }
}
