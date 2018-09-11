using System;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Interfaces;
using CLIForms.Styles;

namespace CLIForms.Components
{
    public class Button : DisplayObject, IFocusable, IAcceptInput
    {
        public ConsoleColor? BackgroudColor = ConsoleColor.DarkGray;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public ConsoleColor? BackgroudColorFocused = ConsoleColor.Magenta;
        public ConsoleColor ForegroundColorFocused = ConsoleColor.Black;

        public ShadowStyle Shadow = ShadowStyle.None;
        public ShadowStyle ShadowFocused = ShadowStyle.Light;

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    Dirty = true;
                }
            }
        }

        public Button(Container parent, string text)
        {
            _text = text;
            Parent = parent;
            parent.AddChild(this);


        }

        private int _width = 0;
        public int Width
        {
            get => _width;
            set
            {
                _autoWidth = false;
                _width = value;
                Dirty = true;
            }
        }

        private int _height = 3;
        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                Dirty = true;
            }
        }

        private bool _autoWidth = true;
        public bool AutoWidth
        {
            get => _autoWidth;
            set
            {
                _autoWidth = value;
                Dirty = true;
            }
        }

        private int _autoWidthMargin = 1;
        public int AutoWidthMargin
        {
            get => _autoWidthMargin;
            set
            {
                _autoWidthMargin = value;
                Dirty = true;
            }
        }


        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            if (_autoWidth)
            {
                _width = _text.Length + _autoWidthMargin * 2;

            }

            ConsoleCharBuffer buffer = new ConsoleCharBuffer(_width + 1, _height + 1);

            DrawingHelper.DrawBlockFull(buffer, this, true, 0, 0, _width, _height,
                _focused ? BackgroudColorFocused : BackgroudColor, _focused ? ForegroundColorFocused : ForegroundColor,
                BorderStyle.None, _focused ? ShadowFocused : Shadow);
            
            int yOffset = (int) Math.Floor((Height - 1) / 2.0);

            int xOffset = (int)Math.Floor(((double)Width - _text.Length) / 2);

            buffer.DrawString(this, _text, true, xOffset, yOffset, _focused ? BackgroudColorFocused : BackgroudColor,
                _focused ? ForegroundColorFocused : ForegroundColor);

            _dirty = false;

            return buffer;
        }

        private bool _focused = false;
        public bool Focused {
            get => _focused;
            set
            {
                _focused = value;
                Dirty = true;
            }
        }
        public event FocusEventHandler FocusIn;
        public event FocusEventHandler FocusOut;
        public void FireFocusIn(ConsoleKeyInfo? key)
        {

        }

        public void FireFocusOut(ConsoleKeyInfo? key)
        {

        }

        public bool FireKeypress(ConsoleKeyInfo key)
        {
            return false;
        }

        public event KeyPressEventHandler Keypress;
    }
}
