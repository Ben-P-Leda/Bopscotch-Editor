using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Level_Editor.Editor_Components
{
    public class InputHandler
    {
        public delegate void MouseActionHandler();

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        private List<Keys> _numberKeys;

        public MouseActionHandler StartMouseAction { private get; set; }
        public MouseActionHandler CancelMouseAction { private get; set; }
        public MouseActionHandler CompleteMouseAction { private get; set; }

        private bool MouseActive { get { return new Rectangle(0, 0, 1600, 900).Contains(_currentMouseState.X, _currentMouseState.Y); } }
        public Vector2 MousePosition { get { return new Vector2(_currentMouseState.X, _currentMouseState.Y); } }
        public Vector2 DragStartPosition { get; private set; }

        public bool NumberKeyIsPressed { get { return (NumberKeyPressed > -1); } }
        public int NumberKeyPressed { get; private set; }

        public bool MouseButtonHeld
        {
            get
            {
                return ((_currentMouseState.LeftButton == ButtonState.Pressed) && (_previousMouseState.LeftButton == ButtonState.Pressed));
            }
        }

        public InputHandler()
        {
            _numberKeys = new List<Keys>() { Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9 };
            NumberKeyPressed = -1;
        }

        public void Update()
        {
            _previousMouseState = _currentMouseState;
            _previousKeyboardState = _currentKeyboardState;

            _currentMouseState = Mouse.GetState();
            _currentKeyboardState = Keyboard.GetState();

            if ((_currentMouseState.LeftButton == ButtonState.Pressed) && (_previousMouseState.LeftButton == ButtonState.Released) && (MouseActive))
            {
                DragStartPosition = MousePosition;
                StartMouseAction();
            }
            else if (_previousMouseState.LeftButton == ButtonState.Pressed)
            {
                if ((!MouseActive) || (_currentMouseState.RightButton == ButtonState.Pressed)) { CancelMouseAction(); }
                else if (_currentMouseState.LeftButton == ButtonState.Released) { CompleteMouseAction(); }
            }

            NumberKeyPressed = -1;
            for (int i = 0; i < _numberKeys.Count; i++)
            {
                if (KeyPressed(_numberKeys[i])) { NumberKeyPressed = i; break; }
            }
        }

        public bool KeyPressed(Keys toCheck)
        {
            return ((_currentKeyboardState.IsKeyDown(toCheck)) && (!_previousKeyboardState.IsKeyDown(toCheck)));
        }

        public bool KeyDown(Keys toCheck)
        {
            return (_currentKeyboardState.IsKeyDown(toCheck));
        }
    }
}
