using System;
using System.Xml.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Leda.Core.Timing;
using Leda.Core.Serialization;
using Leda.Core.Gamestate_Management;
using Leda.Core.Asset_Management;
using Leda.Core.Game_Objects.Behaviours;

#if WINDOWS_PHONE
using Microsoft.Phone.Shell;
#endif

namespace Leda.Core
{
    public class GameBase : Game
    {
        private static GameBase _instance = null;
        public static GameBase Instance { get { return _instance; } }

        private Dictionary<Type, Scene> _scenes;
        private Scene _currentScene;
        private string _tombstoneFileName;
        private string _sceneTransitionCrossFadeTextureName;

        public string TombstoneFileName { set { _tombstoneFileName = value; } }
        public int MillisecondsSinceLastUpdate { get { if (_currentScene != null) { return _currentScene.MillisecondsSinceLastUpdate; } else { return 0; } } }

        public bool EnsureAllContentIsVisible { get; set; }

        public Rectangle SceneBackBufferArea
        {
            set
            {
                foreach (KeyValuePair<Type, Scene> kvp in _scenes) { kvp.Value.ScaledBufferFrame = value; }
            }
        }

        public string SceneTransitionCrossFadeTextureName
        {
            set
            {
                _sceneTransitionCrossFadeTextureName = value;
                foreach (KeyValuePair<Type, Scene> kvp in _scenes) { kvp.Value.CrossFadeTextureName = _sceneTransitionCrossFadeTextureName; }
            }
        }

        public GameBase(int screenWidth, int screenHeight)
            : this(screenWidth, screenHeight, true)
        {
        }

        public GameBase(int screenWidth, int screenHeight, bool fullScreen)
            : base()
        {
            _instance = this;

            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.IsFullScreen = fullScreen;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;

            Content.RootDirectory = "Content";

			GlobalTimerController.ClearInstance();

            _scenes = new Dictionary<Type, Scene>();
            _currentScene = null;
            _tombstoneFileName = "";
        }

        protected override void Initialize()
        {
            MusicManager.Initialize();
            base.Initialize();
        }

        protected void AddScene(Scene toAdd)
        {
            if (toAdd.DeactivationHandler == null) { toAdd.DeactivationHandler = SceneTransitionHandler; }
            if (toAdd is AssetLoaderScene) { ((AssetLoaderScene)toAdd).LoadCompletionHandler = HandleAssetLoadCompletion; }

            if (!string.IsNullOrEmpty(_sceneTransitionCrossFadeTextureName)) { toAdd.CrossFadeTextureName = _sceneTransitionCrossFadeTextureName; }

            _scenes.Add(toAdd.GetType(), toAdd);
        }

        protected void StartInitialScene(Type startingSceneType)
        {
            if ((_currentScene == null) && (_scenes.ContainsKey(startingSceneType)))
            {
                SceneTransitionHandler(startingSceneType); 
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override bool BeginDraw()
        {
            if (_currentScene != null) { _currentScene.RenderContentToBackBuffer(); }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            return base.BeginDraw();
        }

        private void SceneTransitionHandler(Type nextSceneType)
        {
            if (_scenes.ContainsKey(nextSceneType)) 
            {
                _currentScene = _scenes[nextSceneType];
                _scenes[nextSceneType].Activate(); 
            }
        }

        private void HandleAssetLoadCompletion(Type loaderSceneType)
        {
            foreach (KeyValuePair<Type, Scene> kvp in _scenes) { kvp.Value.HandleAssetLoadCompletion(loaderSceneType); }
        }
    }
}
