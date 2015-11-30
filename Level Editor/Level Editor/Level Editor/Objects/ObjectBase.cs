using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;
using Leda.Core.Game_Objects.Base_Classes;
using Leda.Core.Game_Objects.Behaviours;
using Leda.Core.Asset_Management;
using Leda.Core.Motion;

namespace Level_Editor.Objects
{
    public abstract class ObjectBase : DisposableSimpleDrawableObject, Interfaces.IComponent, IMobile
    {
        private PositionRestoreMotionEngine _motionEngine;
        private bool _selected;
        private Vector2 _startPosition;
        private string _textureReference;

        public string TextureReference 
        {
            protected get { return _textureReference; }
            set { _textureReference = value; Texture = TextureManager.Textures[value]; } 
        }

        public IMotionEngine MotionEngine { get { return _motionEngine; } }

        public abstract XElement SaveNode { get; }

        public Point DimensionsInCells { get; protected set; }
        public bool Selected { get { return _selected; } set { _selected = value; if (_selected) { _startPosition = WorldPosition; } } }

        public Point GridPositionInCells { get { return new Point((int)(WorldPosition.X / Definitions.CellSizeInPixels), (int)(WorldPosition.Y / Definitions.CellSizeInPixels)); } }
        public Vector2 WorldPositionSnappedToGrid { get { return new Vector2(GridPositionInCells.X * Definitions.CellSizeInPixels, GridPositionInCells.Y * Definitions.CellSizeInPixels); } }

        public Rectangle WorldPositionedContainer
        {
            get
            {
                return new Rectangle(
                    (int)WorldPosition.X,
                    (int)WorldPosition.Y,
                    DimensionsInCells.X * Definitions.CellSizeInPixels,
                    DimensionsInCells.Y * Definitions.CellSizeInPixels);
            }
        }

        public bool ReturningToPreviousLocation { get; set; }

        public ObjectBase()
        {
            _textureReference = "";
            _motionEngine = new PositionRestoreMotionEngine();
            _motionEngine.ObjectToMove = this;
            _selected = false;

            ReturningToPreviousLocation = false;
        }

        public void Update(int millisecondsSinceLastUpdate)
        {
            if (ReturningToPreviousLocation)
            {
                _motionEngine.Target = _startPosition;

                MotionEngine.Update(millisecondsSinceLastUpdate);

                if ((WorldPosition == _startPosition) || (Vector2.DistanceSquared(WorldPosition, _startPosition) < MotionEngine.Delta.LengthSquared()))
                {
                    WorldPosition = _startPosition;
                    ReturningToPreviousLocation = false;
                }
                else
                {
                    WorldPosition += MotionEngine.Delta;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            DrawFrame(spriteBatch);
        }

        protected void DrawFrame(SpriteBatch spriteBatch)
        {
            if (Selected)
            {
                Vector2 topLeft = WorldPosition - CameraPosition;
                Vector2 bottomRight = topLeft + new Vector2(DimensionsInCells.X * Definitions.CellSizeInPixels, DimensionsInCells.Y * Definitions.CellSizeInPixels);

                RenderTools.Line(spriteBatch, TextureManager.Textures["pixel"], topLeft, new Vector2(bottomRight.X, topLeft.Y), 1.0f, Color.White, 0.01f);
                RenderTools.Line(spriteBatch, TextureManager.Textures["pixel"], new Vector2(bottomRight.X, topLeft.Y), bottomRight, 1.0f, Color.White, 0.01f);
                RenderTools.Line(spriteBatch, TextureManager.Textures["pixel"], bottomRight, new Vector2(topLeft.X, bottomRight.Y), 1.0f, Color.White, 0.01f);
                RenderTools.Line(spriteBatch, TextureManager.Textures["pixel"], new Vector2(topLeft.X, bottomRight.Y), topLeft, 1.0f, Color.White, 0.01f);
            }
        }

        public virtual XElement Save()
        {
            XElement node = SaveNode;
            node.Add(new XAttribute("x", WorldPosition.X));
            node.Add(new XAttribute("y", WorldPosition.Y));

            return node;
        }

        public virtual bool Contains(Vector2 worldPosition)
        {
            return WorldPositionedContainer.Contains((int)worldPosition.X, (int)worldPosition.Y);
        }

        public bool Intersects(ObjectBase checkAgainst)
        {
            return ((WorldPositionedContainer.Intersects(checkAgainst.WorldPositionedContainer)) ||
                (WorldPositionedContainer.Contains(checkAgainst.WorldPositionedContainer)) ||
                (checkAgainst.WorldPositionedContainer.Contains(WorldPositionedContainer)));
        }
    }
}
