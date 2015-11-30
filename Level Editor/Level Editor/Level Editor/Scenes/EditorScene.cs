using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Leda.Core.Gamestate_Management;
using Leda.Core.Asset_Management;
using Leda.Core.Game_Objects.Behaviours;
using Leda.Core.Game_Objects.Controllers;
using Leda.Core.Game_Objects.Controllers.Camera;

using Level_Editor.Data_Container;
using Level_Editor.Interfaces;
using Level_Editor.Editor_Components;
using Level_Editor.Editor_Components.Modal_Boxes;
using Level_Editor.Objects;
using Level_Editor.Objects.Terrain;
using Level_Editor.Objects.Terrain.Blocks;
using Level_Editor.Objects.Terrain.Collectables;
using Level_Editor.Objects.Terrain.Signposts;
using Level_Editor.Objects.Terrain.Flags;
using Level_Editor.Objects.Characters;
using Level_Editor.Objects.Characters.Enemies;

namespace Level_Editor.Scenes
{
    public class EditorScene : Scene
    {
        private InputHandler _inputHandler;
        private ControlPanel _controlPanel;
        private MobileCameraController _cameraController;
        private MotionController _motionController;

        private IComponent _selected;
        private Vector2 _selectionOffset;

        private Dictionary<string, ButtonBox> _modalBoxes;
        private ButtonBox _activeModalBox;

        private Player _player;

        public EditorScene()
            : base(1600, 900)
        {
            _inputHandler = new InputHandler()
            {
                StartMouseAction = StartMouseAction,
                CancelMouseAction = CancelMouseAction,
                CompleteMouseAction = CompleteMouseAction
            };

            _cameraController = new MobileCameraController();
            _motionController = new MotionController();

            CreateControls();
            CreateModalBoxes();

            Data.Container.RegisterComponent = RegisterGameObject;
            Data.Container.UnregisterComponent = UnregisterGameObject;

            _selected = null;
            _activeModalBox = null;           
        }

        public override void HandleAssetLoadCompletion(Type loaderSceneType)
        {
            ((BackgroundSelector)_modalBoxes["background-selector"]).CreateButtons();
            ((BlockSelector)_modalBoxes["block-selector"]).CreateButtons();
            ((CollectableSelector)_modalBoxes["candy-selector"]).CreateButtons();
            ((SignSelector)_modalBoxes["sign-selector"]).CreateButtons();
            ((FlagSelector)_modalBoxes["flag-selector"]).CreateButtons();
            ((SmashBlockItemsSelector)_modalBoxes["smash-block-contents-selector"]).CreateButtons();
            ((EnemySelector)_modalBoxes["enemy-selector"]).CreateButtons();
            ((RouteMarkerSelector)_modalBoxes["route-selector"]).CreateButtons();

            RegisterGameObject(new Background());

            _player = new Player();
            RegisterGameObject(_player);
        }

        protected override void RegisterGameObject(IGameObject toRegister)
        {
            if (toRegister is ICameraLinked) { _cameraController.AddCameraLinkedObject((ICameraLinked)toRegister); }
            if (toRegister is IMobile) { _motionController.AddMobileObject((IMobile)toRegister); }
            if (toRegister is IComponent) { Data.Container.Components.Add((IComponent)toRegister); }
            base.RegisterGameObject(toRegister);
        }

        protected override void UnregisterGameObject(IGameObject toUnregister)
        {
            if (toUnregister is ICameraLinked) { _cameraController.RemoveCameraLinkedObject((ICameraLinked)toUnregister); }
            if (toUnregister is IMobile) { _motionController.RemoveMobileObject((IMobile)toUnregister); }
            if (toUnregister is IComponent) { Data.Container.Components.Remove((IComponent)toUnregister); }
            base.UnregisterGameObject(toUnregister);
        }

        public override void Update(GameTime gameTime)
        {
            _inputHandler.Update();
            _motionController.Update(MillisecondsSinceLastUpdate);

            CheckAndHandleCameraMovement();
            HandleSelectionDrag();
            CheckAndHandleKeyCommands();

            base.Update(gameTime);
        }

        private void CheckAndHandleCameraMovement()
        {
            Vector2 cameraDelta = Vector2.Zero;
            float movementStep = 8.0f;

            if (_inputHandler.KeyDown(Keys.LeftShift)) { movementStep = 32.0f; }

            if (_inputHandler.KeyDown(Keys.Up)) { cameraDelta.Y = -Math.Min(_cameraController.WorldPosition.Y, movementStep); }
            else if (_inputHandler.KeyDown(Keys.Down)) { cameraDelta.Y = movementStep; }

            if (_inputHandler.KeyDown(Keys.Left)) { cameraDelta.X = -Math.Min(_cameraController.WorldPosition.X, movementStep); }
            else if (_inputHandler.KeyDown(Keys.Right)) { cameraDelta.X = movementStep; }

            _cameraController.WorldPosition += cameraDelta;
        }

        private void HandleSelectionDrag()
        {
            if ((_selected != null) && (_inputHandler.MouseButtonHeld) && (_selected.Contains(_inputHandler.MousePosition + _cameraController.WorldPosition)))
            {
                _selected.WorldPosition = (_inputHandler.MousePosition + _cameraController.WorldPosition) - _selectionOffset;
            }
        }

        private void CheckAndHandleKeyCommands()
        {
            if ((_inputHandler.KeyPressed(Keys.Delete)) && (_selected != null) && (!(_selected is Player)))
            {
                if (_selected is CheckpointFlag) { RemoveSelectedCheckpointFromSortOrder((CheckpointFlag)_selected); }
                UnregisterGameObject(_selected); _selected = null; 
            }

            if ((_inputHandler.NumberKeyIsPressed) && (_selected is CheckpointFlag))
            {
                UpdateCheckpointSequence((CheckpointFlag)_selected, _inputHandler.NumberKeyPressed);
            }

            if (_inputHandler.KeyPressed(Keys.S)) { Data.Container.Save(); ActivateModalBox("save-acknowledge"); }
            else if (_inputHandler.KeyPressed(Keys.L)) { ActivateModalBox("load-confirm"); }

            if (_inputHandler.KeyPressed(Keys.Enter))
            {
                if (_selected is Player) { _player.StartsLevelMovingLeft = !_player.StartsLevelMovingLeft; }
                if (_selected is OneWaySignpost) { ((OneWaySignpost)_selected).Mirror = !((OneWaySignpost)_selected).Mirror; }
                if (_selected is SpeedLimitSignpost) { ((SpeedLimitSignpost)_selected).UpdateRange(); }
                if (_selected is RouteMarker) { ((RouteMarker)_selected).Rotate(); }
                if (_selected is SmashBlock) { ActivateSmashBlockContentSelector((SmashBlock)_selected); }
                if (_selected is FlagBase) { ((FlagBase)_selected).ActivatedWhenMovingLeft = !((FlagBase)_selected).ActivatedWhenMovingLeft; }
            }

            if (_inputHandler.KeyDown(Keys.OemPlus))
            {
                if (_inputHandler.KeyPressed(Keys.OemPlus))
                {
                    if (_activeModalBox is SmashBlockItemsSelector) { ((SmashBlockItemsSelector)_activeModalBox).UpdateSelectedItemUnits(1); }
                    if (_activeModalBox is RaceLapCountSelector) { ((RaceLapCountSelector)_activeModalBox).UpdateLapCount(1); }
                }

                if (_selected is TerrainObjectWithCollisionZone) { ((TerrainObjectWithCollisionZone)_selected).CollisionZoneTopOffset -= 2.0f; }
            }

            if (_inputHandler.KeyDown(Keys.OemMinus))
            {
                if (_inputHandler.KeyPressed(Keys.OemMinus))
                {
                    if (_activeModalBox is SmashBlockItemsSelector) { ((SmashBlockItemsSelector)_activeModalBox).UpdateSelectedItemUnits(-1); }
                    if (_activeModalBox is RaceLapCountSelector) { ((RaceLapCountSelector)_activeModalBox).UpdateLapCount(-1); }
                }

                if (_selected is TerrainObjectWithCollisionZone) { ((TerrainObjectWithCollisionZone)_selected).CollisionZoneTopOffset += 2.0f; }
            }
        }

        protected override void HandleBackButtonPress()
        {
            if (_inputHandler.KeyPressed(Keys.Escape))
            {
                if (_activeModalBox != null) 
                {
                    if (_activeModalBox is SelectionBox) { ((SelectionBox)_activeModalBox).Selection = ""; }
                    _activeModalBox.Visible = false; 
                    _activeModalBox = null;
                    _controlPanel.UpdateSelectedOption(Vector2.Zero);
                }
                else { ActivateModalBox("exit-confirm"); }
            }
        }

        private void CreateControls()
        {
            _controlPanel = new Editor_Components.ControlPanel();
            RegisterGameObject(_controlPanel);

            RegisterGameObject(new MouseWorldPositionBox() { Input = _inputHandler, Camera = _cameraController });
            RegisterGameObject(new ScreensPositionBox() { Camera = _cameraController });
        }

        private void CreateModalBoxes()
        {
            _modalBoxes = new Dictionary<string, ButtonBox>();

            _modalBoxes.Add("exit-confirm", new ConfirmBox("Exit Editor?"));
            _modalBoxes.Add("save-acknowledge", new AcknowledgeBox("Save Complete!"));
            _modalBoxes.Add("load-confirm", new ConfirmBox("Load Level Data?"));
            _modalBoxes.Add("load-acknowledge", new AcknowledgeBox("Load Complete!"));
            _modalBoxes.Add("load-error", new AcknowledgeBox("Load Error!"));
            _modalBoxes.Add("background-selector", new BackgroundSelector());
            _modalBoxes.Add("block-selector", new BlockSelector());
            _modalBoxes.Add("candy-selector", new CollectableSelector());
            _modalBoxes.Add("sign-selector", new SignSelector());
            _modalBoxes.Add("flag-selector", new FlagSelector());
            _modalBoxes.Add("smash-block-contents-selector", new SmashBlockItemsSelector());
            _modalBoxes.Add("race-lap-count-selector", new RaceLapCountSelector());
            _modalBoxes.Add("enemy-selector", new EnemySelector());
            _modalBoxes.Add("route-selector", new RouteMarkerSelector());

            foreach (KeyValuePair<string, ButtonBox> kvp in _modalBoxes) { RegisterGameObject(kvp.Value); }
        }

        private void RemoveSelectedCheckpointFromSortOrder(CheckpointFlag toRemove)
        {
            List<IGameObject> flags = GameObjects(typeof(CheckpointFlag));
            for (int i = 0; i < flags.Count; i++)
            {
                if (((CheckpointFlag)flags[i]).CheckpointIndex > toRemove.CheckpointIndex) { ((CheckpointFlag)flags[i]).CheckpointIndex--; }
            }
            FlagFactory.DecrementNextCheckpointIndex();
        }

        private void UpdateCheckpointSequence(CheckpointFlag targetFlag, int numberToSetTo)
        {
            List<IGameObject> flags = GameObjects(typeof(CheckpointFlag));
            if ((numberToSetTo < flags.Count) && (targetFlag.CheckpointIndex != numberToSetTo))
            {
                for (int i = 0; i < flags.Count; i++)
                {
                    if (((CheckpointFlag)flags[i]).CheckpointIndex == numberToSetTo)
                    {
                        ((CheckpointFlag)flags[i]).CheckpointIndex = targetFlag.CheckpointIndex;
                        targetFlag.CheckpointIndex = numberToSetTo;
                        break;
                    }
                }
            }
        }

        private void ActivateModalBox(string boxName)
        {
            _activeModalBox = _modalBoxes[boxName];
            _activeModalBox.Visible = true;
        }

        private void ActivateSmashBlockContentSelector(SmashBlock target)
        {
            ((SmashBlockItemsSelector)_modalBoxes["smash-block-contents-selector"]).Target = target;
            ActivateModalBox("smash-block-contents-selector");
        }

        private void DeactivateModalBox()
        {
            if (_activeModalBox != null) { _activeModalBox.Visible = false; }
            _activeModalBox = null;
        }

        private void StartMouseAction()
        {
            if ((_activeModalBox == null) && (!_controlPanel.Contains(_inputHandler.MousePosition)))
            {
                CheckForSelection();
            }
        }

        private void CheckForSelection()
        {
            for (int i = 0; i < Data.Container.Components.Count; i++)
            {
                Data.Container.Components[i].Selected = false;

                if ((Data.Container.Components[i].Contains(_inputHandler.MousePosition + _cameraController.WorldPosition)) &&
                    (!Data.Container.Components[i].ReturningToPreviousLocation))
                {
                    _selected = Data.Container.Components[i];
                    _selected.Selected = true;
                    _selectionOffset = (_inputHandler.MousePosition + _cameraController.WorldPosition) - _selected.WorldPosition;
                }
            }

            if ((_selected != null) && (!_selected.Selected)) { _selected = null; }
        }

        private void CancelMouseAction()
        {
            if (_activeModalBox == null)
            {
                if (_selected is TerrainObjectBase) { _selected.ReturningToPreviousLocation = true; }
            }
        }

        private void CompleteMouseAction()
        {
            if (_activeModalBox != null) { CheckAndHandleModalAction(); }
            else if (_controlPanel.Contains(_inputHandler.MousePosition)) { CheckAndHandleCommandChange(); }
            else if (_selected == null)
            {
                switch (_controlPanel.ActiveOption)
                {
                    case "Player":
                        _player.WorldPosition = SnapToGrid(_inputHandler.MousePosition + _cameraController.WorldPosition - new Vector2(Definitions.CellSizeInPixels / 2.0f));
                        _player.Visible = true; 
                        break;
                    case "Blocks": 
                        if (ModalBoxSelectionCanBePlaced("block-selector")) 
                        {
                            AddTerrainObject(BlockFactory.CreateBlockFromTextureName(SelectorValue("block-selector")), _inputHandler.MousePosition);
                        }
                        break;
                    case "Candies":
                        if (ModalBoxSelectionCanBePlaced("candy-selector"))
                        {
                            AddTerrainObject(CollectableFactory.CreateCandyFromTextureName(SelectorValue("candy-selector")), _inputHandler.MousePosition);
                        }
                        break;
                    case "Signs":
                        if (ModalBoxSelectionCanBePlaced("sign-selector"))
                        {
                            AddTerrainObject(SignpostFactory.CreateSignpostFromTextureName(SelectorValue("sign-selector")), _inputHandler.MousePosition);
                        }
                        break;
                    case "Flags":
                        if (ModalBoxSelectionCanBePlaced("flag-selector"))
                        {
                            AddTerrainObject(FlagFactory.CreateFlagFromTextureName(SelectorValue("flag-selector")), _inputHandler.MousePosition);
                        }
                        break;
                    case "Enemies":
                        if (EnemySelectionCanBePlaced())
                        {
                            AddEnemy(EnemyFactory.CreateEnemyFromTextureName(SelectorValue("enemy-selector")), _inputHandler.MousePosition);
                        }
                        break;
                    case "Route":
                        if (ModalBoxSelectionCanBePlaced("route-selector"))
                        {
                            AddTerrainObject(SignpostFactory.CreateRouteMarkerFromTextureName(SelectorValue("route-selector")), _inputHandler.MousePosition);
                        }
                        break;
                }
            }
            else if (_selected is ObjectBase)
            {
                if (!SnapDraggedObjectToGrid()) { _selected.ReturningToPreviousLocation = true; _selected.Selected = false; _selected = null; }
            }
        }

        private void CheckAndHandleModalAction()
        {
            if (_activeModalBox is ConfirmBox)
            {
                if (_activeModalBox.ButtonAtScreenPosition(_inputHandler.MousePosition) == "Yes")
                {
                    switch (_activeModalBox.CaptionText)
                    {
                        case "Load Level Data?":
                            DeactivateModalBox();
                            if (Data.Container.Load()) { ActivateModalBox("load-acknowledge"); } else { ActivateModalBox("load-error"); }
                            break;
                        case "Exit Editor?": Game.Exit(); break;
                    }
                }
                else { DeactivateModalBox(); }
            }

            if ((_activeModalBox is AcknowledgeBox) && (_activeModalBox.ButtonAtScreenPosition(_inputHandler.MousePosition) == "OK"))
            {
                DeactivateModalBox();
            }

            if (_activeModalBox is SelectionBox)
            {
                ((SelectionBox)_activeModalBox).Selection = _activeModalBox.ButtonAtScreenPosition(_inputHandler.MousePosition);
                if (!string.IsNullOrEmpty(((SelectionBox)_activeModalBox).Selection)) { HandleSelectionBoxSelection(); }
            }

            if (_activeModalBox is SmashBlockItemsSelector)
            {
                string selectedItem = _activeModalBox.ButtonAtScreenPosition(_inputHandler.MousePosition);
                switch (selectedItem)
                {
                    case "Save": UpdateSmashBlockContents(); break;
                    case "Cancel": DeactivateModalBox(); break;
                    default: ((SmashBlockItemsSelector)_activeModalBox).ActivateOption(selectedItem); break;
                }
            }

            if (_activeModalBox is RaceLapCountSelector)
            {
                switch (_activeModalBox.ButtonAtScreenPosition(_inputHandler.MousePosition))
                {
                    case "Clear": Data.Container.RaceLapCount = 0; break;
                    case "Save": Data.Container.RaceLapCount = ((RaceLapCountSelector)_activeModalBox).LapCount; break;
                }
                DeactivateModalBox();
                _controlPanel.UpdateSelectedOption(Vector2.Zero);
            }
        }

        private void HandleSelectionBoxSelection()
        {
            if (_activeModalBox is BackgroundSelector) { _controlPanel.UpdateSelectedOption(Vector2.Zero); }

            DeactivateModalBox(); 
        }

        private void UpdateSmashBlockContents()
        {
            ((SmashBlockItemsSelector)_activeModalBox).StoreContentsSettingsForTargetSmashBlock();
            DeactivateModalBox();
        }

        private void CheckAndHandleCommandChange()
        {
            _controlPanel.UpdateSelectedOption(_inputHandler.MousePosition);

            switch (_controlPanel.ActiveOption)
            {
                case "Race": ActivateModalBox("race-lap-count-selector"); break;
                case "BG": ActivateModalBox("background-selector"); break;
                case "Blocks": ActivateModalBox("block-selector"); break;
                case "Candies": ActivateModalBox("candy-selector"); break;
                case "Signs": ActivateModalBox("sign-selector"); break;
                case "Flags": ActivateModalBox("flag-selector"); break;
                case "Enemies": ActivateModalBox("enemy-selector"); break;
                case "Route": ActivateModalBox("route-selector"); break;
            }
        }

        private string SelectorValue(string boxName)
        {
            if ((_modalBoxes.ContainsKey(boxName)) && (_modalBoxes[boxName] is SelectionBox))
            {
                return ((SelectionBox)_modalBoxes[boxName]).Selection;
            }

            return "";
        }

        private bool ModalBoxSelectionCanBePlaced(string boxName)
        {
            return ((_selected == null) && (!string.IsNullOrEmpty(SelectorValue(boxName))));
        }

        private void AddTerrainObject(TerrainObjectBase toAdd, Vector2 worldPosition)
        {
            toAdd.WorldPosition = SnapToGrid(worldPosition + _cameraController.WorldPosition - new Vector2(Definitions.CellSizeInPixels / 2.0f));
            RegisterGameObject(toAdd);
        }

        private Vector2 SnapToGrid(Vector2 worldPosition)
        {
            Vector2 snapper = new Vector2(worldPosition.X % Definitions.CellSizeInPixels, worldPosition.Y % Definitions.CellSizeInPixels);
            if (snapper.X >= Definitions.CellSizeInPixels / 2.0f) { snapper.X -= Definitions.CellSizeInPixels; }
            if (snapper.Y >= Definitions.CellSizeInPixels / 2.0f) { snapper.Y -= Definitions.CellSizeInPixels; }

            return worldPosition - snapper;
        }

        private bool SnapDraggedObjectToGrid()
        {
            _selected.WorldPosition = SnapToGrid(_selected.WorldPosition);
            for (int i = 0; i < Data.Container.Components.Count; i++)
            {
                if (( Data.Container.Components[i] is ObjectBase) && ( Data.Container.Components[i] != _selected))
                {
                    if (((ObjectBase) Data.Container.Components[i]).Intersects((ObjectBase)_selected)) { return false; }
                }
            }

            return true;
        }

        private bool EnemySelectionCanBePlaced()
        {
            return true;
        }

        private void AddEnemy(CharacterObjectBase toAdd, Vector2 worldPosition)
        {
            //toAdd.WorldPosition = SnapToGrid(worldPosition + _cameraController.WorldPosition - new Vector2(Definitions.CellSizeInPixels / 2.0f));
            //RegisterGameObject(toAdd);
        }
    }
}
