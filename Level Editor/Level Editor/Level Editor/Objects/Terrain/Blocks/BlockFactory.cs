using System.Xml.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Leda.Core.Asset_Management;

using Level_Editor.Data_Container;

namespace Level_Editor.Objects.Terrain.Blocks
{
    public sealed class BlockFactory
    {
        private static BlockFactory _factory = null;
        private static BlockFactory Factory { get { if (_factory == null) { _factory = new BlockFactory(); } return _factory; } }

        public static TerrainObjectBase CreateBlockFromTextureName(string textureName)
        {
            Block newBlock = null;

            switch (textureName)
            {
                case "block-spring":
                    newBlock = Factory.CreateSpringBlock(textureName);
                    break;
                case "block-smash-crate":
                    newBlock = Factory.CreateSmashBlock(textureName);
                    break;
                case "block-spike":
                    newBlock = Factory.CreateSpikeBlock(textureName);
                    break;
                case "block-ice":
                    newBlock = Factory.CreateIceBlock(textureName);
                    break;
                default:
                    foreach (string s in TextureManager.Textures.Keys)
                    {
                        if (s == textureName) 
                        {
                            if (s.Contains("obstruction")) { newBlock = Factory.CreateObstructionBlock(textureName); }
                            else { newBlock = Factory.CreateBlock(textureName); }
                        }
                    }
                    break;
            }

            if (newBlock != null) { newBlock.GroupNodeName = Data_Group_Node_Name; }
            return newBlock;
        }

        public static void LoadBlocks(XElement blockDataGroup, Data.RegistrationCallback registerComponent)
        {
            if (blockDataGroup != null)
            {
                foreach (XElement node in blockDataGroup.Elements())
                {
                    Block toAdd = CreateBlockFromXmlNode(node);
                    toAdd.WorldPosition = new Vector2((float)node.Attribute("x"), (float)node.Attribute("y"));
                    registerComponent(toAdd);
                }
            }
        }

        private static Block CreateBlockFromXmlNode(XElement node)
        {
            Block newBlock = null;

            switch (node.Name.ToString())
            {
                case SpringBlock.Save_Node_Name:
                    newBlock = Factory.CreateSpringBlock(node.Attribute("texture").Value);
                    break;
                case SmashBlock.Save_Node_Name:
                    newBlock = Factory.CreateSmashBlock(node.Attribute("texture").Value);
                    ((SmashBlock)newBlock).LoadContents(node);
                    break;
                case SpikeBlock.Save_Node_Name:
                    newBlock = Factory.CreateSpikeBlock(node.Attribute("texture").Value);
                    break;
                case ObstructionBlock.Save_Node_Name:
                    newBlock = Factory.CreateObstructionBlock(node.Attribute("texture").Value);
                    break;
                case IceBlock.Save_Node_Name:
                    newBlock = Factory.CreateIceBlock(node.Attribute("texture").Value);
                    break;
                default:
                    newBlock = Factory.CreateBlock(node.Attribute("texture").Value);
                    break;
            }

            if (newBlock != null) { newBlock.GroupNodeName = Data_Group_Node_Name; }
            return newBlock;
        }

        private Block CreateBlock(string textureName)
        {
            Block newBlock = new Block();
            newBlock.TextureReference = textureName;

            return newBlock;
        }

        private ObstructionBlock CreateObstructionBlock(string textureName)
        {
            ObstructionBlock newBlock = new ObstructionBlock();
            newBlock.TextureReference = textureName;

            return newBlock;
        }

        private SpringBlock CreateSpringBlock(string textureName)
        {
            SpringBlock newBlock = new SpringBlock();
            newBlock.TextureReference = textureName;

            return newBlock;
        }

        private SmashBlock CreateSmashBlock(string textureName)
        {
            SmashBlock newBlock = new SmashBlock();
            newBlock.TextureReference = textureName;

            return newBlock;
        }

        private SpikeBlock CreateSpikeBlock(string textureName)
        {
            SpikeBlock newBlock = new SpikeBlock();
            newBlock.TextureReference = textureName;

            return newBlock;
        }

        private IceBlock CreateIceBlock(string textureName)
        {
            IceBlock newBlock = new IceBlock();
            newBlock.TextureReference = textureName;

            return newBlock;
        }

        public const string Data_Group_Node_Name = "blocks";
    }
}
