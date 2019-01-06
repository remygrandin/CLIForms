using System;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Engine;
using CLIForms_FIGFonts;

namespace CLIForms.Components.Texts
{
    public class FIG : DisplayObject
    {

        public ConsoleColor? BackgroundColor = null;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        private char? _transparentChar;
        public char? TransparentChar
        {
            get { return _transparentChar; }
            set
            {
                if (_transparentChar != value)
                {
                    _transparentChar = value;
                    Dirty = true;
                }
            }
        }

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

        private string _fontName;
        public string FontName
        {
            get { return _fontName; }
            set
            {
                if (_fontName != value)
                {
                    _fontName = value;
                    Dirty = true;
                }
            }
        }

        public FIG(Container parent, string text = "", string fontName = "banner") : base(parent)
        {
            Text = text;
            FontName = fontName;
        }


        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            Font figfont = FontFactory.GetFont(FontName);

            FIGBuffer rendered = figfont.Render(Text);

            ConsoleCharBuffer buffer = new ConsoleCharBuffer(rendered.Width, rendered.Height);

            buffer.Clear();

            for (int x = 0; x < buffer.Width; x++)
            {
                for (int y = 0; y < buffer.Height; y++)
                {
                    char ch = rendered.data[x, y].Char;
                    if(ch != TransparentChar)
                        buffer.data[x, y] = new ConsoleChar(this, ch, false, BackgroundColor, ForegroundColor);
                }
            }

            Dirty = false;

            return buffer;
        }
    }
}
