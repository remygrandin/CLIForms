using System;
using System.Linq;
using System.Text;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Extentions;
using CLIForms.Styles;

namespace CLIForms.Components.Globals
{
    public class StatusBar : DisplayObject
    {
        public ConsoleColor? BackgroundColor = ConsoleColor.Gray;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        private string _textLeft = "";
        public string TextLeft
        {
            get { return _textLeft; }
            set
            {
                if (value != _textLeft)
                {
                    _textLeft = value;

                    Dirty = true;
                }
            }
        }

        private string _textCenter = "";
        public string TextCenter
        {
            get { return _textCenter; }
            set
            {
                if (value != _textCenter)
                {
                    _textCenter = value;

                    Dirty = true;
                }
            }
        }

        private string _textRight = "";
        public string TextRight
        {
            get { return _textRight; }
            set
            {
                if (value != _textRight)
                {
                    _textRight = value;

                    Dirty = true;
                }
            }
        }


        public StatusBar(Container parent) : base(parent)
        {
        }

        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            

            DisplayObject rootScreenCandidate = this.Parents<DisplayObject>(item => item.Parent).LastOrDefault();

            if (!(rootScreenCandidate is Screen))
            {
                _dirty = false;
                return new ConsoleCharBuffer(0,0);
            }

            Screen screen = (Screen)rootScreenCandidate;

            
       
            if (screen.Height == 0)
            {
                _dirty = false;
                return new ConsoleCharBuffer(0, 0);

            }

            ConsoleCharBuffer baseBuffer = new ConsoleCharBuffer(screen.Width, 1);

            baseBuffer.yOffset = screen.Height - 1;

            string textLeftTruncated = _textLeft.Truncate(screen.Width);

            string textCenterTruncated = _textCenter.Truncate(screen.Width);

            string textRightTruncated = _textRight.Truncate(screen.Width);

           
            StringBuilder outputLine = new StringBuilder(new string(' ', screen.Width));


            int centerStartOffset = (screen.Width - textCenterTruncated.Length) / 2;


            for (int i = 0; i < textCenterTruncated.Length; i++)
            {
                outputLine[i + centerStartOffset] = textCenterTruncated[i];
            }

            for (int i = 0; i < textRightTruncated.Length; i++)
            {
                outputLine[screen.Width - textRightTruncated.Length + i] = textRightTruncated[i];
            }

            for (int i = 0; i < textLeftTruncated.Length; i++)
            {
                outputLine[i] = textLeftTruncated[i];
            }

            baseBuffer.DrawString(this, outputLine.ToString(), false, 0, 0, BackgroundColor, ForegroundColor);

            displayBuffer = baseBuffer;

            _dirty = false;

            return baseBuffer;
        }
    }
}
