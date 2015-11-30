using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;

namespace Level_Editor.Objects.Terrain.Flags
{
    public class CheckpointFlag : FlagBase
    {
        public override XElement SaveNode { get { return new XElement(Save_Node_Name); } }
        public int CheckpointIndex { get; set; }

        public CheckpointFlag(int checkpointIndex)
            : base()
        {
            CheckpointIndex = checkpointIndex;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            TextWriter.Write(CheckpointIndex.ToString(), spriteBatch, WorldPosition - CameraPosition, Color.White, Color.Black, 2.0f, 
                RenderDepth - 0.001f, TextWriter.Alignment.Left);
        }

        public override XElement Save()
        {
            XElement node = base.Save();
            node.Attribute("texture").Value = FlagTextureName;
            node.Add(new XAttribute("index", CheckpointIndex));

            return node;
        }

        public const string Save_Node_Name = "checkpoint-flag";
    }
}
