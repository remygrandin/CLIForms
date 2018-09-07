using System;
using CLIForms.Extentions;
using CLIForms.Styles;

namespace CLIForms.Components
{
    public class Dialog : Container
    {

        public ConsoleColor? BackgroudColor = ConsoleColor.Gray;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public BorderStyle Border = BorderStyle.Thick;

        public ShadowStyle Shadow = ShadowStyle.Light;

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    Dirty = true;
                }
            }
        }

        public Dialog(Container parent, int width = 30, int height = 12) : base(width, height)
        {
            Parent = parent;
            parent.AddChild(this);
        }

        

        protected override ConsoleCharBuffer RenderContainer()
        {
            ConsoleCharBuffer buffer = new ConsoleCharBuffer(Width + 1, Height + 1);

            DrawingHelper.DrawBlockFull(buffer,this, 0, 0, Width, Height, BackgroudColor, ForegroundColor, Border, Shadow);

            buffer.DrawString(this, _title.Truncate(Width - 2), 1, 0, ConsoleColor.Black, ConsoleColor.White);

            return buffer;
        }
    }
}
