using System.Xml.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Leda.Core;
using Leda.Core.Asset_Management;

using Level_Editor.Data_Container;

namespace Level_Editor.Objects.Terrain.Signposts
{
    public sealed class SignpostFactory
    {
        private static SignpostFactory _factory = null;
        private static SignpostFactory Factory { get { if (_factory == null) { _factory = new SignpostFactory(); } return _factory; } }

        public static TerrainObjectBase CreateSignpostFromTextureName(string textureName)
        {
            TerrainObjectBase newSignpost = null;

            switch (textureName)
            {
                case "sign-one-way": newSignpost = Factory.CreateOneWaySignpost(textureName); break;
                case "sign-speed-1-4": newSignpost = Factory.CreateSpeedLimitSignpost("sign-speed-1-1"); break;
            }

            if (newSignpost != null) { newSignpost.GroupNodeName = Data_Group_Node_Name; }
            return newSignpost;
        }

        public static RouteMarker CreateRouteMarkerFromTextureName(string textureName)
        {
            RouteMarker newMarker = Factory.CreateRouteMarker(textureName);
            newMarker.GroupNodeName = Data_Group_Node_Name;

            return newMarker;
        }

        public static void LoadSignposts(XElement SignpostDataGroup, Data.RegistrationCallback registerComponent)
        {
            if (SignpostDataGroup != null)
            {
                foreach (XElement node in SignpostDataGroup.Elements())
                {
                    if (node.Name == RouteMarker.Save_Node_Name)
                    {
                        RouteMarker toAdd = CreateRouteMarkerFromXmlNode(node);
                        toAdd.WorldPosition = new Vector2((float)node.Attribute("x"), (float)node.Attribute("y"));
                        registerComponent(toAdd);
                    }
                    else
                    {
                        SignpostBase toAdd = CreateSignpostFromXmlNode(node);
                        toAdd.WorldPosition = new Vector2((float)node.Attribute("x"), (float)node.Attribute("y"));
                        registerComponent(toAdd);
                    }
                }
            }
        }

        public static RouteMarker CreateRouteMarkerFromXmlNode(XElement node)
        {
            RouteMarker newMarker = Factory.CreateRouteMarker(node.Attribute("texture").Value);
            newMarker.GroupNodeName = Data_Group_Node_Name;
            newMarker.Quadrant = (int)node.Attribute("quadrant");

            return newMarker;
        }

        private static SignpostBase CreateSignpostFromXmlNode(XElement node)
        {
            SignpostBase newSignpost = null;

            switch (node.Name.ToString())
            {
                case OneWaySignpost.Save_Node_Name:
                    newSignpost = Factory.CreateOneWaySignpost(node.Attribute("texture").Value);
                    ((OneWaySignpost)newSignpost).Mirror = (bool)node.Attribute("mirror");
                    break;
                case SpeedLimitSignpost.Save_Node_Name:
                    newSignpost = Factory.CreateSpeedLimitSignpost(node.Attribute("texture").Value);
                    ((SpeedLimitSignpost)newSignpost).SpeedLimits = new Range((int)node.Attribute("minimum-speed"), (int)node.Attribute("maximum-speed"));
                    break;
            }

            if (newSignpost != null) 
            { 
                newSignpost.GroupNodeName = Data_Group_Node_Name;
                newSignpost.CollisionZoneTopOffset = (float)node.Attribute("zone-top");
            }

            return newSignpost;
        }

        private OneWaySignpost CreateOneWaySignpost(string textureName)
        {
            OneWaySignpost newSignpost = new OneWaySignpost();
            newSignpost.TextureReference = textureName;

            return newSignpost;
        }

        private SpeedLimitSignpost CreateSpeedLimitSignpost(string textureName)
        {
            SpeedLimitSignpost newSignpost = new SpeedLimitSignpost();
            newSignpost.TextureReference = textureName;

            return newSignpost;
        }

        public RouteMarker CreateRouteMarker(string textureName)
        {
            RouteMarker newMarker = new RouteMarker();
            newMarker.SetTexture(textureName);

            return newMarker;
        }

        public const string Data_Group_Node_Name = "signposts";
    }
}
