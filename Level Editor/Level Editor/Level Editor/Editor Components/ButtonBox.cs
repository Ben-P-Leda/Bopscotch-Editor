using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leda.Core;
using Leda.Core.Game_Objects.Behaviours;
using Leda.Core.Asset_Management;

namespace Level_Editor.Editor_Components
{
    public class ButtonBox : Box
    {
        protected List<Button> _buttons = new List<Button>();

        public override bool Visible
        {
            get { return base.Visible; }
            set { for (int i = 0; i < _buttons.Count; i++) { _buttons[i].Visible = value; } base.Visible = value; }
        }

        public ButtonBox(Rectangle frame)
            : this(frame, "")
        {
        }

        public ButtonBox(Rectangle frame, string caption)
            : base(frame, caption)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            for (int i = 0; i < _buttons.Count; i++) { _buttons[i].Draw(spriteBatch); }
        }

        public void AddButton(Button toAdd)
        {
            _buttons.Add(toAdd);
        }

        public string ButtonAtScreenPosition(Vector2 screenPosition)
        {
            return ButtonAtScreenPosition(screenPosition, Color.White);
        }

        public string ButtonAtScreenPosition(Vector2 screenPosition, Color selectedTint)
        {
            string selection = "";

            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i].Contains(screenPosition)) { selection = _buttons[i].CaptionText; _buttons[i].Tint = selectedTint; }
                else { _buttons[i].Tint = Color.White; }
            }

            return selection;
        }
    }
}
