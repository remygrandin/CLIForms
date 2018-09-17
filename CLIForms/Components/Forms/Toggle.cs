using System;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Interfaces;

namespace CLIForms.Components.Forms
{
    public class Toggle : DisplayObject, IFocusable, IAcceptInput
    {
        public ConsoleColor? LabelBackgroudColor = null;
        public ConsoleColor LabelForegroundColor = ConsoleColor.White;

        public ConsoleColor CursorForegroundColor = ConsoleColor.White;
        public ConsoleColor ActiveAreaForegroundColor = ConsoleColor.Green;
        public ConsoleColor InactiveAreaForegroundColor = ConsoleColor.Red;


        public ConsoleColor? BackgroudColor = null;
        public ConsoleColor? BackgroudColorFocused = ConsoleColor.White;
        public ConsoleColor CursorForegroundColorFocused = ConsoleColor.Black;

        private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                if (_label != value)
                {
                    _label = value;
                    Dirty = true;
                }
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    Dirty = true;
                }
            }
        }

        public Toggle(Container parent, string label, bool isChecked = false) : base(parent)
        {
            _label = label;
            _isChecked = isChecked;
        }



        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer buffer = new ConsoleCharBuffer(Label.Length + 4, 1);

            if (_isChecked)
            {
                buffer.data[0, 0] = new ConsoleChar(this, '■', true, _focused ? BackgroudColorFocused : BackgroudColor, ActiveAreaForegroundColor);
                buffer.data[1, 0] = new ConsoleChar(this, '■', true, _focused ? BackgroudColorFocused : BackgroudColor, ActiveAreaForegroundColor);
                buffer.data[2, 0] = new ConsoleChar(this, '■', true, _focused ? BackgroudColorFocused : BackgroudColor, _focused ? CursorForegroundColorFocused : CursorForegroundColor);
            }
            else
            {
                buffer.data[0, 0] = new ConsoleChar(this, '■', true, _focused ? BackgroudColorFocused : BackgroudColor, _focused ? CursorForegroundColorFocused : CursorForegroundColor);
                buffer.data[1, 0] = new ConsoleChar(this, '■', true, _focused ? BackgroudColorFocused : BackgroudColor, InactiveAreaForegroundColor);
                buffer.data[2, 0] = new ConsoleChar(this, '■', true, _focused ? BackgroudColorFocused : BackgroudColor, InactiveAreaForegroundColor);
            }

            buffer.DrawString(this, _label, true, 4, 0, LabelBackgroudColor, LabelForegroundColor);

            _dirty = false;

            return buffer;
        }

        private bool _focused = false;
        public bool Focused
        {
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

        public bool KeyPressed(ConsoleKeyInfo key)
        {

            switch (key.Key)
            {
                case ConsoleKey.Spacebar:
                case ConsoleKey.Enter:
                    {
                        IsChecked = !IsChecked;


                        Clicked?.Invoke(this, new EventArgs());
                        Dirty = true;
                        return true;
                    }
            }
            return false;
        }

        public event EventHandler Clicked;
    }
}
