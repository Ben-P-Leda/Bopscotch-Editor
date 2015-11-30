using System.Xml.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Leda.Core.Asset_Management;

using Level_Editor.Data_Container;

namespace Level_Editor.Objects.Characters.Enemies
{
    public sealed class EnemyFactory
    {
        private static EnemyFactory _factory = null;
        private static EnemyFactory Factory { get { if (_factory == null) { _factory = new EnemyFactory(); } return _factory; } }

        public static CharacterObjectBase CreateEnemyFromTextureName(string textureName)
        {
            CharacterObjectBase newEnemy = null;

            if (textureName.IndexOf("-flying-") > 0)
            {
                newEnemy = Factory.CreateFlyingEnemy(textureName);
            }

            if (newEnemy != null) { newEnemy.GroupNodeName = Data_Group_Node_Name; }
            return newEnemy;
        }

        public static void LoadEnemies(XElement EnemyDataGroup, Data.RegistrationCallback registerComponent)
        {
            if (EnemyDataGroup != null)
            {
                foreach (XElement node in EnemyDataGroup.Elements())
                {
                    CharacterObjectBase toAdd = CreateEnemyFromXmlNode(node);
                    toAdd.WorldPosition = new Vector2((float)node.Attribute("x"), (float)node.Attribute("y"));
                    registerComponent(toAdd);
                }
            }
        }

        private static CharacterObjectBase CreateEnemyFromXmlNode(XElement node)
        {
            CharacterObjectBase newEnemy = null;

            //switch (node.Name.ToString())
            //{
            //    case GoalFlag.Save_Node_Name:
            //        newFlag = Factory.CreateGoalFlag(node.Attribute("texture").Value);
            //        newFlag.CollisionZoneTopOffset = (float)node.Attribute("zone-top");
            //        break;
            //    case CheckpointFlag.Save_Node_Name:
            //        newFlag = Factory.CreateRestartFlag(node.Attribute("texture").Value);
            //        newFlag.CollisionZoneTopOffset = (float)node.Attribute("zone-top");
            //        break;
            //}

            if (newEnemy != null) { newEnemy.GroupNodeName = Data_Group_Node_Name; }
            return newEnemy;
        }

        private CharacterObjectBase CreateFlyingEnemy(string textureName)
        {
            //GoalFlag newFlag = new GoalFlag();
            //newFlag.FlagTextureName = textureName;

            //return newFlag;

            return null;
        }

        public const string Data_Group_Node_Name = "enemies";
    }
}
