using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Level_Editor.Editor_Components.Modal_Boxes
{
    public class SmashBlockItemsSelector : ButtonBox
    {
        private QuantitySpinner _activeSpinner;
        private Objects.Terrain.Blocks.SmashBlock _target;

        public Objects.Terrain.Blocks.SmashBlock Target
        {
            set
            {
                _target = value;
                for (int i = 0; i < _buttons.Count; i++)
                {
                    if (_buttons[i] is QuantitySpinner)
                    {
                        if (_target.Contents.ContainsKey(_buttons[i].CaptionText))
                        {
                            ((QuantitySpinner)_buttons[i]).UnitCount = _target.Contents[_buttons[i].CaptionText];
                        }
                        else
                        {
                            ((QuantitySpinner)_buttons[i]).UnitCount = 0;
                        }
                    }
                }
            }
        }

        public SmashBlockItemsSelector()
            : base(new Rectangle(530, 150, 540, 600), "Select Items")
        {
            Target = null;
        }

        public void CreateButtons()
        {
            int x = 550;
            int y = 200;

            foreach (string s in Item_Textures.Split(','))
            {
                AddButton(new QuantitySpinner(new Rectangle(x, y, Definitions.CellSizeInPixels, Definitions.CellSizeInPixels), s) { DepthBase = -0.005f });

                x += QuantitySpinner.Total_Width + Margin;
                if (x > 1000) { x = 550; y += Definitions.CellSizeInPixels + Margin; }
            }

            AddButton(new Button(new Rectangle(830, 690, 100, 40), "Save") { DepthBase = -0.005f });
            AddButton(new Button(new Rectangle(950, 690, 100, 40), "Cancel") { DepthBase = -0.005f });

            ActivateOption(Item_Textures.Split(',')[0]);
        }

        public void ActivateOption(string selectedItem)
        {
            _activeSpinner = null;

            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i] is QuantitySpinner)
                {
                    if (_buttons[i].CaptionText == selectedItem)
                    {
                        ((QuantitySpinner)_buttons[i]).Active = true;
                        _activeSpinner = ((QuantitySpinner)_buttons[i]);
                    }
                    else
                    {
                        ((QuantitySpinner)_buttons[i]).Active = false;
                    }
                }
            }
        }

        public void UpdateSelectedItemUnits(int delta)
        {
            if (((delta < 0) && (_activeSpinner.UnitCount > 0)) || ((delta > 0) && (_activeSpinner.UnitCount < 25)))
            {
                _activeSpinner.UnitCount += delta;
            }
        }

        public void StoreContentsSettingsForTargetSmashBlock()
        {
            _target.Contents.Clear();
            for (int i = 0; i < _buttons.Count; i++)
            {
                if ((_buttons[i] is QuantitySpinner) && (((QuantitySpinner)_buttons[i]).UnitCount > 0))
                {
                    _target.Contents.Add(_buttons[i].CaptionText, ((QuantitySpinner)_buttons[i]).UnitCount);
                }
            }
        }

        private const string Item_Textures = "golden-ticket,candy-1,candy-2,candy-3,candy-4,candy-5";
        private const int Margin = 20;
    }
}
