using System;
using System.Collections.Generic;
using System.Linq;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Engine;
using CLIForms.Interfaces;

namespace CLIForms.Components.Forms
{
    public class Radio : DisplayObject, IInterractive
    {
        public ConsoleColor? LabelBackgroundColor = null;
        public ConsoleColor LabelForegroundColor = ConsoleColor.White;

        public ConsoleColor? BackgroundColor = null;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public ConsoleColor? BackgroundColorFocused = ConsoleColor.White;
        public ConsoleColor ForegroundColorFocused = ConsoleColor.Black;

        public ConsoleColor? BackgroundColorChecked = ConsoleColor.Green;
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
                buffer.DrawString(this, "(O)", true, 0, 0, _focused ? BackgroundColorFocused : BackgroundColorChecked, _focused ? ForegroundColorFocused : ForegroundColorChecked);
            else
                buffer.DrawString(this, "( )", true, 0, 0, _focused ? BackgroundColorFocused : BackgroundColor, _focused ? ForegroundColorFocused : ForegroundColor);


            buffer.DrawString(this, _label, false, 4, 0, LabelBackgroundColor, LabelForegroundColor);

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
        public void FocusedIn(ConsoleKeyInfo? key)
        {
            if (FocusIn != null)
                foreach (FocusEventHandler handler in FocusIn.GetInvocationList())
                {
                    if (handler?.Invoke(this) == true)
                        return;
                }
        }

        public void FocusedOut(ConsoleKeyInfo? key)
        {
            if (FocusOut != null)
                foreach (FocusEventHandler handler in FocusOut.GetInvocationList())
                {
                    if (handler?.Invoke(this) == true)
                        return;
                }
        }

        public bool KeyPressed(ConsoleKeyInfo key)
        {

            switch (key.Key)
            {
                case ConsoleKey.Spacebar:
                case ConsoleKey.Enter:
                    {
                        IsChecked = !IsChecked;


                        if (Activated != null)
                            foreach (ActivatedEventHandler handler in Activated.GetInvocationList())
                            {
                                handler?.Invoke(this);
                            }

                        Dirty = true;
                        return true;
                    }
            }
            return false;
        }

        public event ActivatedEventHandler Activated;
    }
}
