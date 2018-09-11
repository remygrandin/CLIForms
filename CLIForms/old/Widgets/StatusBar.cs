using System;
using System.Text;
using CLIForms.Buffer;

namespace CLIForms.Widgets
{
    public class StatusBar : Widget
    {
        internal StatusBar() {}
        public StatusBar(Widget parent) : base(parent) 
        {
            Background = ConsoleColor.Gray;
            Foreground = ConsoleColor.Black;
        }

        private string _textLeft = "";
        public string TextLeft
        {
            get { return _textLeft; }
            set
            {
                if (value != _textLeft)
                {
                    bool parentRedraw = value.Length < _textLeft.Length;
                    _textLeft = value;

                    if (Parent != null)
                    {
                        if (parentRedraw) { Parent.Draw(); }
                        else { Draw(); }
                    }
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
                    bool parentRedraw = value.Length < _textCenter.Length;
                    _textCenter = value;

                    if (Parent != null)
                    {
                        if (parentRedraw) { Parent.Draw(); }
                        else { Draw(); }
                    }
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
                    bool parentRedraw = value.Length < _textRight.Length;
                    _textRight = value;

                    if (Parent != null)
                    {
                        if (parentRedraw) { Parent.Draw(); }
                        else { Draw(); }
                    }
                }
            }
        }


        // Left > Right > Center
        internal override void Render()
        {
            int width = this.RootWindow.Width;

            string textLeftTruncated = _textLeft;

            if (textLeftTruncated.Length > width)
                textLeftTruncated = textLeftTruncated.Substring(0, width - 3) + "...";

            string textCenterTruncated = _textCenter;

            if (textCenterTruncated.Length > width)
                textCenterTruncated = textCenterTruncated.Substring(0, width - 3) + "...";
                    
            string textRightTruncated = _textRight;

            if (textRightTruncated.Length > width)
                textRightTruncated = textRightTruncated.Substring(0, width - 3) + "...";


            StringBuilder outputLine = new StringBuilder(new string(' ', width));


            int centerStartOffset = (width - textCenterTruncated.Length) / 2;


            for (int i = 0; i < textCenterTruncated.Length; i++)
            {
                outputLine[i + centerStartOffset] = textCenterTruncated[i];
            }

            for (int i = 0; i < textRightTruncated.Length; i++)
            {
                outputLine[width - textRightTruncated.Length + i] = textRightTruncated[i];
            }
            
            for (int i = 0; i < textLeftTruncated.Length; i++)
            {
                outputLine[i] = textLeftTruncated[i];
            }


            ConsoleHelper.DrawText(0, this.RootWindow.Height -1, Foreground, Background, outputLine.ToString());
        }
    }
}
