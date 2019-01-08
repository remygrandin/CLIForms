using System;
using System.Collections.Generic;
using System.Linq;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Engine;
using CLIForms.Engine.Events;

namespace CLIForms.Components.Forms
{
    public class Radio : InteractiveObject
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

            KeyDown += Radio_KeyDown;
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

        private void Radio_KeyDown(Engine.Events.KeyboardEvent evt)
        {
            switch (evt.VirtualKeyCode)
            {
                case VirtualKey.Space:
                case VirtualKey.Enter:
                    {
                        IsChecked = !IsChecked;

                        Engine.Engine.Instance.TriggerActivated(this);
                    }
                    break;
            }
        }
    }
}
