using System;
using System.Xml.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core.Asset_Management;
using Leda.Core.Timing;
using Leda.Core.Game_Objects.Behaviours;
using Leda.Core.Game_Objects.Controllers.Rendering;

namespace Leda.Core.Gamestate_Management
{
    public abstract class Scene : DrawableGameComponent
    {
        public delegate void DeactivationHandlerFunction(Type nextSceneType);
        public delegate void ObjectRegistrationHandler(IGameObject toRegister);
        public delegate void ObjectUnregistrationHandler(IGameObject toUnRegister);

        private SpriteBatch _spriteBatch;
        private RenderController _renderController;

        private Status _status;
        private Timer _transitionTimer;

        private Type _nextSceneType;

        private RenderTarget2D _backBuffer;
        private Point _bufferDimensions;
        private Rectangle _bufferFrame;
        private Color _clearColour;
        private Color _bufferTint;
        private float _crossfadeDuration;

        private Rectangle _safeDisplayArea;

        private TimeSpan _lastUpdateTime;
        private int _lastUpdateDuration;

        private bool _backButtonHeld;

        private List<IGameObject> _gameObjects;
        private List<ITemporary> _temporaryObjects;

        protected SpriteBatch SpriteBatch { get { return _spriteBatch; } }
        protected RenderController Renderer { get { return _renderController; } }
        protected virtual Type NextSceneType { set { _nextSceneType = value; } }
        protected SceneParameters NextSceneParameters { get { return SceneParameters.Instance; } }
        protected Color ClearColour { set { _clearColour = value; } }
        protected float CrossfadeDuration { set { _crossfadeDuration = value; } }

        public Status CurrentState { get { return _status; } }
        public DeactivationHandlerFunction DeactivationHandler { set; get; }
        public int MillisecondsSinceLastUpdate { get { return _lastUpdateDuration; } }
        public Rectangle ScaledBufferFrame { get { return _bufferFrame; } set { _bufferFrame = value; } }
        public Rectangle SafeDisplayArea { get { return _safeDisplayArea; } }
        public bool DoNotUseBackBuffer { protected get; set; }

        public string CrossFadeTextureName { private get; set; }

        public Scene(int bufferWidth, int bufferHeight)
            : base(GameBase.Instance)
        {
            DeactivationHandler = null;
            DoNotUseBackBuffer = true;

            _spriteBatch = null;
            _renderController = new RenderController();

            _status = Status.Inactive;
            _transitionTimer = new Timer("scene.transitiontimer", HandleTransitionCompletion);

            _nextSceneType = null;

            _backBuffer = null;
            _bufferDimensions = new Point(bufferWidth, bufferHeight);
            _clearColour = Color.Black;
            _bufferTint = Color.Transparent;
            _crossfadeDuration = Default_Crossfade_Duration;
            CrossFadeTextureName = "";

            _gameObjects = new List<IGameObject>();
            _temporaryObjects = new List<ITemporary>();

            GameBase.Instance.Components.Add(this);
            Enabled = false;
            Visible = false;

            GlobalTimerController.GlobalTimer.RegisterUpdateCallback(_transitionTimer.Tick);
        }

        public override void Initialize()
        {
            _safeDisplayArea = new Rectangle(0, 0, _bufferDimensions.X, _bufferDimensions.Y);

            base.Initialize();
            CalculateBackBufferFrameAndSafeArea();
        }

        private void CalculateBackBufferFrameAndSafeArea()
        {
            float windowAspectRatio = (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height;
            float bufferAspectRatio = (float)_bufferDimensions.X / (float)_bufferDimensions.Y;

            if (windowAspectRatio == bufferAspectRatio)
            {
                _bufferFrame = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
            }
            else
            {
                bool scaleToHeight = (windowAspectRatio > bufferAspectRatio);
                if (!GameBase.Instance.EnsureAllContentIsVisible) { scaleToHeight = !scaleToHeight; }

                if (scaleToHeight) { SetScalingToScreenHeight(); }
                else { SetScalingToScreenWidth(); }
            }
        }

        private void SetScalingToScreenWidth()
        {
            float scaleModifier = (float)Game.Window.ClientBounds.Width / (float)_bufferDimensions.X;
            _bufferFrame = new Rectangle(0, 0, Game.Window.ClientBounds.Width, (int)(_bufferDimensions.Y * scaleModifier));
            _bufferFrame.Y = (Game.Window.ClientBounds.Height - _bufferFrame.Height) / 2;

            if (_bufferFrame.Y < 0)
            {
                int overspill = (int)((-_bufferFrame.Y) / scaleModifier) + Safe_Area_Margin;
                _safeDisplayArea.Y = overspill;
                _safeDisplayArea.Height -= (overspill * 2);
            }
        }

        private void SetScalingToScreenHeight()
        {
            float scaleModifier = (float)Game.Window.ClientBounds.Height / (float)_bufferDimensions.Y;
            _bufferFrame = new Rectangle(0, 0, (int)(_bufferDimensions.X * scaleModifier), Game.Window.ClientBounds.Height);
            _bufferFrame.X = (Game.Window.ClientBounds.Width - _bufferFrame.Width) / 2;

            if (_bufferFrame.X < 0)
            {
                int overspill = (int)((-_bufferFrame.X) / scaleModifier) + Safe_Area_Margin;
                _safeDisplayArea.X = overspill;
                _safeDisplayArea.Width -= (overspill * 2);
            }
        }

        private void CreateBackBuffer()
        {
            _backBuffer = new RenderTarget2D(GraphicsDevice, _bufferDimensions.X, _bufferDimensions.Y);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            if (_lastUpdateTime != TimeSpan.Zero)
            {
                TimeSpan difference = (gameTime.TotalGameTime - _lastUpdateTime);
                _lastUpdateDuration = difference.Milliseconds + (difference.Seconds / 1000);
            }

            if (AnyBackButtonHasBeenPressed) { HandleBackButtonPress(); }

            if (_status == Status.Activating) { _bufferTint = Color.Lerp(Color.Transparent, Color.White, _transitionTimer.CurrentActionProgress); }
            else if (_status == Status.Deactivating) { _bufferTint = Color.Lerp(Color.White, Color.Transparent, _transitionTimer.CurrentActionProgress); }

            RemoveDisposedObjects();

            base.Update(gameTime);

            _lastUpdateTime = gameTime.TotalGameTime;
        }

        private bool AnyBackButtonHasBeenPressed
        {
            get
            {
                bool isPressed = Keyboard.GetState().IsKeyDown(Keys.Escape);
                int padIndex = 0;

                while ((padIndex < Maximum_Game_Pads) && (!isPressed)) 
                { 
                    isPressed = (GamePad.GetState((PlayerIndex)padIndex++).Buttons.Back == ButtonState.Pressed);
                }

                if (!isPressed) { _backButtonHeld = false; }
                else if (_backButtonHeld) { isPressed = false; }
                else { _backButtonHeld = true; }

                return isPressed;
            }
        }

        public void RenderContentToBackBuffer()
        {
            BeginRender();

            if (!DoNotUseBackBuffer)
            {
                if (_backBuffer == null) { CreateBackBuffer(); }

                GraphicsDevice.SetRenderTarget(_backBuffer);
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.Clear(_clearColour);

                Render();
            }
        }

        protected virtual void BeginRender()
        {
        }

        protected virtual void Render()
        {
            _renderController.RenderObjects(_spriteBatch);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_spriteBatch != null)
            {
                if (DoNotUseBackBuffer)
                {
                    GraphicsDevice.Clear(_clearColour);
                    Render();

                    if (CurrentState == Status.Activating) { OverlayCrossfader(1.0f - _transitionTimer.CurrentActionProgress); }
                    else if (CurrentState == Status.Deactivating) { OverlayCrossfader(_transitionTimer.CurrentActionProgress); }
                }
                else if (_backBuffer != null)
                {
                    _spriteBatch.Begin();
                    _spriteBatch.Draw(_backBuffer, _bufferFrame, null, _bufferTint, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                    _spriteBatch.End();
                }
            }

            base.Draw(gameTime);
        }

        private void OverlayCrossfader(float opacity)
        {
            if (!string.IsNullOrEmpty(CrossFadeTextureName))
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(TextureManager.Textures[CrossFadeTextureName], GraphicsDevice.Viewport.Bounds, Color.Lerp(Color.Transparent, Color.Black, opacity));
                SpriteBatch.End();
            }
        }

        private void HandleTransitionCompletion()
        {
            if (_status == Status.Activating) { CompleteActivation(); }
            else if (_status == Status.Deactivating) { CompleteDeactivation(); }
        }

        protected virtual void HandleBackButtonPress()
        {
        }

        public virtual void Activate()
        {
            Reset();

            if (_crossfadeDuration <= 0.0f)
            {
                CompleteActivation();
            }
            else
            {
                _status = Status.Activating;
                _bufferTint = Color.Transparent;
                _transitionTimer.NextActionDuration = _crossfadeDuration;
                _transitionTimer.ActionSpeed = 1.0f;
            }

            Enabled = true;
            Visible = true;
        }

        protected virtual void CompleteActivation()
        {
            NextSceneParameters.Clear();
            _status = Status.Active;
            _bufferTint = Color.White;
        }

        protected void Deactivate()
        {
            if (_crossfadeDuration <= 0.0f)
            {
                CompleteDeactivation();
            }
            else
            {
                _status = Status.Deactivating;
                _transitionTimer.NextActionDuration = _crossfadeDuration;
                _transitionTimer.ActionSpeed = 1.0f;
            }
        }

        protected virtual void CompleteDeactivation()
        {
            Enabled = false;
            Visible = false;

            _status = Status.Inactive;
            _bufferTint = Color.Transparent;

            for (int i = _temporaryObjects.Count - 1; i >= 0; i--) { _temporaryObjects[i].ReadyForDisposal = true; }
            RemoveDisposedObjects();

            if ((_nextSceneType != null) && (DeactivationHandler != null)) { DeactivationHandler(_nextSceneType); }
            _nextSceneType = null;
        }

        protected virtual void RegisterGameObject(IGameObject toRegister)
        {
            if (!_gameObjects.Contains(toRegister)) { _gameObjects.Add(toRegister); }
            if (toRegister is ITemporary) { _temporaryObjects.Add((ITemporary)toRegister); }
            if (toRegister is ISimpleRenderable) { _renderController.AddRenderableObject((ISimpleRenderable)toRegister); }
        }

        protected virtual void UnregisterGameObject(IGameObject toUnregister)
        {
            if (_gameObjects.Contains(toUnregister)) { _gameObjects.Remove(toUnregister); }
            if (toUnregister is ITemporary) { _temporaryObjects.Remove((ITemporary)toUnregister); }
            if (toUnregister is ISimpleRenderable) { _renderController.RemoveRenderableObject((ISimpleRenderable)toUnregister); }
        }

        protected void FlushGameObjects()
        {
            for (int i = _gameObjects.Count - 1; i >= 0; i--) { UnregisterGameObject(_gameObjects[i]); }
        }

        public List<IGameObject> GameObjects(Type toGet)
        {
            List<IGameObject> objects = new List<IGameObject>();
            for (int i = 0; i < _gameObjects.Count; i++)
            {
                if (_gameObjects[i].GetType() == toGet) { objects.Add(_gameObjects[i]); }
            }
            return objects;
        }

        private void RemoveDisposedObjects()
        {
            for (int i = _temporaryObjects.Count - 1; i >= 0; i--)
            {
                if (_temporaryObjects[i].ReadyForDisposal)
                {
                    _temporaryObjects[i].PrepareForDisposal();
                    UnregisterGameObject((IGameObject)_temporaryObjects[i]); 
                }
            }
        }

        protected void InitializeGameObjects()
        {
            for (int i = 0; i < _gameObjects.Count; i++) { _gameObjects[i].Initialize(); }
        }

        protected virtual void Reset()
        {
            _lastUpdateDuration = 0;
            _lastUpdateTime = TimeSpan.Zero;

            _backButtonHeld = false;

            for (int i = 0; i < _gameObjects.Count; i++) { _gameObjects[i].Reset();  }
        }

        public virtual void HandleAssetLoadCompletion(Type loaderSceneType)
        {
        }

        public virtual void HandleTombstone()
        {
        }

		// Used by Windows Phone version to ensure that fast resume is handled correctly
        public virtual void HandleFastResume()
        {
        }

		// Used by Android version to ensure that the back buffer is cleaned down correctly in a tombstone event
		public void DisposeOfBackBufferCleanly()
		{
			if (_backBuffer != null) { _backBuffer.Dispose(); }
			_backBuffer = null;
		}

        public enum Status
        {
            Inactive,
            Activating,
            Active,
            Deactivating
        }

        private const float Default_Crossfade_Duration = 350.0f;
        private const int Safe_Area_Margin = 5;
        private const int Maximum_Game_Pads = 4;
    }
}
