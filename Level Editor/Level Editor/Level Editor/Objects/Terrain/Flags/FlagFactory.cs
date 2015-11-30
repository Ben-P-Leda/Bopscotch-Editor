using System.Xml.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Leda.Core.Asset_Management;

using Level_Editor.Data_Container;

namespace Level_Editor.Objects.Terrain.Flags
{
    public sealed class FlagFactory
    {
        private static FlagFactory _factory = null;
        private static FlagFactory Factory { get { if (_factory == null) { _factory = new FlagFactory(); } return _factory; } }

        public static TerrainObjectBase CreateFlagFromTextureName(string textureName)
        {
            TerrainObjectBase newFlag = null;

            switch (textureName)
            {
                case "flag-goal": newFlag = Factory.CreateGoalFlag(textureName); break;
                case "flag-checkpoint": newFlag = Factory.CreateRestartFlag(textureName); break;
            }

            if (newFlag != null) { newFlag.GroupNodeName = Data_Group_Node_Name; }
            return newFlag;
        }

        public static void LoadFlags(XElement FlagDataGroup, Data.RegistrationCallback registerComponent)
        {
            if (FlagDataGroup != null)
            {
                foreach (XElement node in FlagDataGroup.Elements())
                {
                    FlagBase toAdd = CreateFlagFromXmlNode(node);
                    toAdd.WorldPosition = new Vector2((float)node.Attribute("x"), (float)node.Attribute("y"));
                    if (node.Attribute("left-activation") != null) { toAdd.ActivatedWhenMovingLeft = (bool)node.Attribute("left-activation"); }
                    registerComponent(toAdd);
                }
            }
        }

        private static FlagBase CreateFlagFromXmlNode(XElement node)
        {
            FlagBase newFlag = null;

            switch (node.Name.ToString())
            {
                case GoalFlag.Save_Node_Name:
                    newFlag = Factory.CreateGoalFlag(node.Attribute("texture").Value);
                    newFlag.CollisionZoneTopOffset = (float)node.Attribute("zone-top");
                    break;
                case CheckpointFlag.Save_Node_Name:
                    newFlag = Factory.CreateRestartFlag(node.Attribute("texture").Value);
                    newFlag.CollisionZoneTopOffset = (float)node.Attribute("zone-top");
                    if (node.Attribute("index") != null) { ((CheckpointFlag)newFlag).CheckpointIndex = (int)node.Attribute("index"); }
                    break;
            }

            if (newFlag != null) { newFlag.GroupNodeName = Data_Group_Node_Name; }
            return newFlag;
        }

        public static void DecrementNextCheckpointIndex()
        {
            Factory._nextCheckpointIndex--;
        }

        private int _nextCheckpointIndex;

        private FlagFactory()
        {
            _nextCheckpointIndex = 0;
        }

        private GoalFlag CreateGoalFlag(string textureName)
        {
            GoalFlag newFlag = new GoalFlag();
            newFlag.FlagTextureName = textureName;

            return newFlag;
        }

        private CheckpointFlag CreateRestartFlag(string textureName)
        {
            CheckpointFlag newFlag = new CheckpointFlag(_nextCheckpointIndex++);
            newFlag.FlagTextureName = textureName;

            return newFlag;
        }

        public const string Data_Group_Node_Name = "flags";
    }
}
