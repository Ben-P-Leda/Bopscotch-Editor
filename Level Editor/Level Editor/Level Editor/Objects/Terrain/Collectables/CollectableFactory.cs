using System.Xml.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Leda.Core.Asset_Management;

using Level_Editor.Data_Container;

namespace Level_Editor.Objects.Terrain.Collectables
{
    public sealed class CollectableFactory
    {
        private static CollectableFactory _factory = null;
        private static CollectableFactory Factory { get { if (_factory == null) { _factory = new CollectableFactory(); } return _factory; } }

        public static TerrainObjectBase CreateCandyFromTextureName(string textureName)
        {
            Collectable newCollectable = null;

            switch (textureName)
            {
                case "golden-ticket": newCollectable = Factory.CreateGoldenTicket(); break;
                default: newCollectable = Factory.CreateScoringCandy(textureName); break;
            }

            if (newCollectable != null) { newCollectable.GroupNodeName = Data_Group_Node_Name; }
            return newCollectable;
        }

        public static void LoadCollectables(XElement collectableDataGroup, Data.RegistrationCallback registerComponent)
        {
            if (collectableDataGroup != null)
            {
                foreach (XElement node in collectableDataGroup.Elements())
                {
                    TerrainObjectBase toAdd = CreateCollectableFromXmlNode(node);
                    toAdd.WorldPosition = new Vector2((float)node.Attribute("x"), (float)node.Attribute("y"));
                    registerComponent(toAdd);
                }
            }
        }

        private static Collectable CreateCollectableFromXmlNode(XElement node)
        {
            Collectable newCollectable = null;

            switch (node.Name.ToString())
            {
                case GoldenTicket.Save_Node_Name:
                    newCollectable = Factory.CreateGoldenTicket();
                    break;
                case ScoringCandy.Save_Node_Name:
                    newCollectable = Factory.CreateScoringCandy(node.Attribute("texture").Value, ((int)node.Attribute("score")));
                    break;
            }

            if (newCollectable != null) { newCollectable.GroupNodeName = Data_Group_Node_Name; }
            return newCollectable;
        }

        private GoldenTicket CreateGoldenTicket()
        {
            return new GoldenTicket();
        }

        private ScoringCandy CreateScoringCandy(string textureName)
        {
            foreach (string s in TextureManager.Textures.Keys)
            {
                if (s == textureName) { return CreateScoringCandy(textureName, GetScoreForTextureName(textureName)); break; }
            }

            return null;
        }

        private ScoringCandy CreateScoringCandy(string textureName, int scoreValue)
        {
            ScoringCandy newCandy = new ScoringCandy();
            newCandy.TextureReference = textureName;
            if (scoreValue != 0) { newCandy.ScoreValue = scoreValue; }

            return newCandy;
        }

        private int GetScoreForTextureName(string textureName)
        {
            switch (textureName)
            {
                case "candy-1": return 25; break;
                case "candy-2": return 50; break;
                case "candy-3": return 75; break;
                case "candy-4": return 100; break;
                case "candy-5": return 150; break;
            }

            return 0;
        }

        public const string Data_Group_Node_Name = "collectables";
    }
}
