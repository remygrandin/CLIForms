using System;
using System.Linq;
using CLIForms.Buffer;
using CLIForms.Extentions;
using CLIForms.Styles;

namespace CLIForms.Components.Containers
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

        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer baseBuffer = RenderContainer();

            ConsoleCharBuffer componentsBuffer = new ConsoleCharBuffer(Width - 2, Height - 2);

            foreach (DisplayObject child in Children.Where(item => item.Visible))
            {
                componentsBuffer = componentsBuffer.Merge(child.Render(), child.X, child.Y);


            }

            baseBuffer.Merge(componentsBuffer, 1, 1);

            displayBuffer = baseBuffer;

            _dirty = false;

            return baseBuffer;
        }

        protected override ConsoleCharBuffer RenderContainer()
        {
            ConsoleCharBuffer buffer = new ConsoleCharBuffer(Width + 1, Height + 1);

            DrawingHelper.DrawBlockFull(buffer, this, false, 0, 0, Width, Height, BackgroudColor, ForegroundColor, Border, Shadow);

            buffer.DrawString(this, _title.Truncate(Width - 2), false, 1, 0, ConsoleColor.Black, ConsoleColor.White);

            return buffer;
        }
    }
}
