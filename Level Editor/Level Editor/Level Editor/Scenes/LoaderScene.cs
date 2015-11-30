using System.Xml.Linq;

using Microsoft.Xna.Framework;

using Leda.Core;
using Leda.Core.Asset_Management;
using Leda.Core.Gamestate_Management;

namespace Level_Editor.Scenes
{
    public class LoaderScene : AssetLoaderScene
    {
        public LoaderScene()
            : base(800, 450)
        {
            NextSceneType = typeof(EditorScene);
        }

        protected override void Render()
        {
            SpriteBatch.Begin();
            TextWriter.Write("Loading", SpriteBatch, new Vector2(800.0f, 450.0f), Color.White, 1.0f, TextWriter.Alignment.Center);
            SpriteBatch.End();
        }

        public override void Activate()
        {
            AssetListFileName = "Content/Files/Loadables.xml";

            base.Activate();
        }

        protected override void LoadCustomContent(XElement asset)
        {
        }
    }
}
