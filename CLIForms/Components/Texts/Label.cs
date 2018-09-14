using System;
using System.Linq;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Extentions;

namespace CLIForms.Components.Texts
{
    public class Label : DisplayObject
    {

        public ConsoleColor? BackgroudColor = null;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

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


        private int? _maxLength;
        public int? MaxLength
        {
            get { return _maxLength; }
            set
            {
                if (_maxLength != value)
                {
                    _maxLength = value;
                    Dirty = true;
                }
            }
        }

        public Label(Container parent, string text = "", int? maxLength = null) : base(parent)
        {
            Text = text;
            MaxLength = maxLength;
        }


        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer buffer;

            string[] lines = Text.Split(new string[] { "\n" }, StringSplitOptions.None);

            if (MaxLength == null)
            {
                buffer = new ConsoleCharBuffer(lines.Max(item => item.Length), lines.Length);

                for (int i = 0; i < lines.Length; i++)
                {
                    buffer.DrawString(this, lines[i], false, 0, i, BackgroudColor, ForegroundColor);
                }
            }
            else
            {
                buffer = new ConsoleCharBuffer(MaxLength.Value, lines.Length);

                for (int i = 0; i < lines.Length; i++)
                {
                    buffer.DrawString(this, lines[i].Truncate(MaxLength.Value), false, 0, i, BackgroudColor, ForegroundColor);
                }

            }

            Dirty = false;

            return buffer;
        }
    }
}
