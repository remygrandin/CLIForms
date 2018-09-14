using System;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Styles;

namespace CLIForms.Components.Drawings
{
    public class HorizontalLine : DisplayObject
    {
        public ConsoleColor? BackgroundColor = null;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public BorderStyle Border = BorderStyle.Thick;

        public LineEndingStyle End1 = LineEndingStyle.Line;
        public LineEndingStyle End2 = LineEndingStyle.Line;

        private int _width;
        public int Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;
                    Dirty = true;
                }
            }
        }

        public HorizontalLine(Container parent, int width = 10) : base(parent)
        {
            Width = width;
        }

        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer baseBuffer = new ConsoleCharBuffer(Width, 1);

            for (int i = 0; i < Width; i++)
            {
                baseBuffer.data[i,0] = new ConsoleChar(this, DrawingHelper.GetHorizontalBorder(Border)[0], false, BackgroundColor, ForegroundColor);
            }

            switch (End1)
            {
                case LineEndingStyle.T:
                    baseBuffer.data[0, 0].Char = DrawingHelper.GetRightTJunctionBorder(Border)[0];
                    break;
                case LineEndingStyle.Plus:
                    baseBuffer.data[0, 0].Char = DrawingHelper.GetCrossJunctionBorder(Border)[0];
                    break;
            }

            switch (End2)
            {
                case LineEndingStyle.T:
                    baseBuffer.data[Width - 1, 0].Char = DrawingHelper.GetLeftTJunctionBorder(Border)[0];
                    break;
                case LineEndingStyle.Plus:
                    baseBuffer.data[Width - 1, 0].Char = DrawingHelper.GetCrossJunctionBorder(Border)[0];
                    break;
            }


            _dirty = false;

            return baseBuffer;
        }
    }
}
