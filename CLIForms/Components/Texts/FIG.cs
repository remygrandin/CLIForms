using System;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms_FIGFonts;

namespace CLIForms.Components.Texts
{
    public class FIG : DisplayObject
    {

        public ConsoleColor? BackgroundColor = null;
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

        public FIG(Container parent, string text = "", int? maxLength = null) : base(parent)
        {
            Text = text;
            MaxLength = maxLength;
        }


        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer buffer = new ConsoleCharBuffer(1,1);

            Font figfont = FontFactory.GetFont("banner");

            string aaa = figfont.Render(Text).StringRender;
            
            Dirty = false;

            return buffer;
        }
    }
}
