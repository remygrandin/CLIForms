using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Interfaces;

namespace CLIForms.Components.Forms
{
    public class Radio : DisplayObject, IFocusable, IAcceptInput
    {
        public ConsoleColor? LabelBackgroudColor = null;
        public ConsoleColor LabelForegroundColor = ConsoleColor.White;

        public ConsoleColor? BackgroudColor = null;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public ConsoleColor? BackgroudColorFocused = ConsoleColor.White;
        public ConsoleColor ForegroundColorFocused = ConsoleColor.Black;

        public ConsoleColor? BackgroudColorChecked = ConsoleColor.Green;
        public ConsoleColor ForegroundColorChecked = ConsoleColor.Black;

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

        private string _radioGroup = "";
        public string RadioGroup
        {
            get { return _radioGroup; }
            set
            {
                if (_radioGroup != value)
                {
                    _radioGroup = value;
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
                    if (_isChecked)
                    {
                        IEnumerable<Radio> siblings = Parent?.GetSiblings(this).Where(item => item != this && item is Radio radio && radio.RadioGroup == this.RadioGroup).Cast<Radio>();
                        if (siblings != null)
                            foreach (Radio sibling in siblings)
                            {
                                sibling.IsChecked = false;
                            }
                    }

                    Dirty = true;
                }
            }
        }

        public Radio(Container parent, string label, string radioGroup = "", bool isChecked = false) : base(parent)
        {
            _label = label;
            _radioGroup = radioGroup;
            IsChecked = isChecked;
        }



        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer buffer = new ConsoleCharBuffer(Label.Length + 4, 1);

            if (_isChecked)
                buffer.DrawString(this, "(O)", true, 0, 0, _focused ? BackgroudColorFocused : BackgroudColorChecked, _focused ? ForegroundColorFocused : ForegroundColorChecked);
            else
                buffer.DrawString(this, "( )", true, 0, 0, _focused ? BackgroudColorFocused : BackgroudColor, _focused ? ForegroundColorFocused : ForegroundColor);


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
