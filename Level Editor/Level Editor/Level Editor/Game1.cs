using Microsoft.Xna.Framework.Graphics;

using Leda.Core;
using Leda.Core.Asset_Management;

namespace Level_Editor
{
    public class Game1 : GameBase
    {
        public Game1()
            : base(1600, 900, false)
        {
            //DoNotUseBackBuffer = true;
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;

            AddScene(new Scenes.LoaderScene());
            AddScene(new Scenes.EditorScene());

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            TextWriter.SetDefaults(Content.Load<SpriteFont>("Font\\Komikandy"), 0.35f);

            StartInitialScene(typeof(Scenes.LoaderScene));
        }
    }
}
