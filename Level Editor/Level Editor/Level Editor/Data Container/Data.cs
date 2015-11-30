using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Leda.Core.Game_Objects.Behaviours;
using Leda.Core.Asset_Management;

using Level_Editor.Interfaces;
using Level_Editor.Objects;
using Level_Editor.Objects.Terrain;
using Level_Editor.Objects.Terrain.Blocks;
using Level_Editor.Objects.Terrain.Collectables;
using Level_Editor.Objects.Terrain.Signposts;
using Level_Editor.Objects.Terrain.Flags;
using Level_Editor.Objects.Characters;

namespace Level_Editor.Data_Container
{
    public class Data
    {
        private static Data _container = null;
        public static Data Container { get { if (_container == null) { _container = new Data(); } return _container; } }

        public delegate void RegistrationCallback(IGameObject targetObject);

        public int RaceLapCount { get; set; }
        public string BackgroundTexture { get; set; }
        public List<IComponent> Components { get; private set; }
        public RegistrationCallback RegisterComponent { private get; set; }
        public RegistrationCallback UnregisterComponent { private get; set; }

        public Data()
        {
            RaceLapCount = 0;
            BackgroundTexture = "";
            Components = new List<IComponent>();
            RegisterComponent = null;
            UnregisterComponent = null;
        }

        public void Save()
        {
            XDocument saveData = new XDocument();
            saveData.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            saveData.Add(new XElement("leveldata"));
            if (RaceLapCount > 0) { saveData.Element("leveldata").Add(RaceLapCountElement); }
            saveData.Element("leveldata").Add(BackgroundSaveElement);
            saveData.Element("leveldata").Add(new XElement("terrain"));

            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i] is TerrainObjectBase)
                {
                    CheckForAndAddGroupingNode(saveData, ((TerrainObjectBase)Components[i]).GroupNodeName);
                    AddTerrainObjectDataNode(saveData, (TerrainObjectBase)Components[i]);
                }

                if ((Components[i] is Player) && (Components[i].Visible))
                {
                    saveData.Element("leveldata").Add(Components[i].Save());
                }
            }

            saveData.Save("BopscotchLevel.xml");
        }

        private void CheckForAndAddGroupingNode(XDocument saveData, string groupingElementName)
        {
            if (saveData.Element("leveldata").Element("terrain").Elements(groupingElementName).Count() < 1)
            {
                saveData.Element("leveldata").Element("terrain").Add(new XElement(groupingElementName));
            }
        }

        private void AddTerrainObjectDataNode(XDocument saveData, TerrainObjectBase toSave)
        {
            saveData.Element("leveldata").Element("terrain").Element(toSave.GroupNodeName).Add(toSave.Save());
        }

        private XElement RaceLapCountElement { get { return new XElement("race-laps", RaceLapCount); } }

        private XElement BackgroundSaveElement
        {
            get
            {
                XElement bgSaveElement = new XElement("background");
                bgSaveElement.Add(new XAttribute("texture", BackgroundTexture));

                return bgSaveElement;
            }
        }

        public bool Load()
        {
            if ((RegisterComponent != null) && (UnregisterComponent != null))
            {
                FlushCurrentComponents();

                try
                {
                    XDocument loadData = XDocument.Load("BopscotchLevel.xml");
                    AttemptToLoadPlayerData(loadData.Element("leveldata"));
                    AttemptToLoadRaceData(loadData.Element("leveldata"));

                    BackgroundTexture = loadData.Element("leveldata").Element("background").Attribute("texture").Value;

                    BlockFactory.LoadBlocks(loadData.Element("leveldata").Element("terrain").Element(BlockFactory.Data_Group_Node_Name), RegisterComponent);
                    CollectableFactory.LoadCollectables(loadData.Element("leveldata").Element("terrain").Element(CollectableFactory.Data_Group_Node_Name), RegisterComponent);
                    SignpostFactory.LoadSignposts(loadData.Element("leveldata").Element("terrain").Element(SignpostFactory.Data_Group_Node_Name), RegisterComponent);
                    FlagFactory.LoadFlags(loadData.Element("leveldata").Element("terrain").Element(FlagFactory.Data_Group_Node_Name), RegisterComponent);

                    return true;
                }
                catch (Exception)
                {
                }
            }

            return false;
        }

        private void FlushCurrentComponents()
        {
            for (int i = Components.Count - 1; i >= 0; i--)
            {
                if (!(Components[i] is Player)) { UnregisterComponent(Components[i]); }
            }
        }

        private void AttemptToLoadPlayerData(XElement levelData)
        {
            Player player = (Player)(from c in Components where c is Player select c).First();
            if (levelData.Elements("player").Count() == 1) { player.Load(levelData.Element("player")); }
        }

        private void AttemptToLoadRaceData(XElement levelData)
        {
            if (levelData.Element("race-laps") != null) { RaceLapCount = (int)levelData.Element("race-laps"); }
        }
    }
}
