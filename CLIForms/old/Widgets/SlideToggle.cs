﻿using System;
using System.Xml.Serialization;
using CLIForms.Buffer;

namespace CLIForms.Widgets
{
    public class SlideToggle : Widget
    {
        internal SlideToggle() {
            this.Clicked += SlideToggle_Clicked;
        }
        public SlideToggle(Widget parent)
            : base(parent)
        {
            ActiveBackground = ConsoleColor.DarkMagenta;
            SelectedBackground = ConsoleColor.Magenta;
            Background = ConsoleColor.Black;
            Foreground = ConsoleColor.White;
            this.Clicked += SlideToggle_Clicked;
        }

        public event EventHandler ValueChanged;

        private bool _checked;
        [XmlAttribute]
        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                if (value != _checked)
                {
                    _checked = value;
                    Draw();
                    if (ValueChanged != null) { ValueChanged(this, EventArgs.Empty); }
                }
            }
        }

        void SlideToggle_Clicked(object sender, EventArgs e)
        {
            Checked = !Checked;
        }

        internal override void Render()
        {
            string Val = ((Checked) ? "  █" : "█  ");
            var useBG = (HasFocus) ? ActiveBackground : Background;
            ConsoleHelper.DrawText(DisplayLeft, DisplayTop, Foreground, useBG, Val);
            ConsoleHelper.DrawText(DisplayLeft + 4, DisplayTop, Foreground, Parent.Background, Text);
        }
    }
}
