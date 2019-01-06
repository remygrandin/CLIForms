using System;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Engine;
using CLIForms.Styles;

namespace CLIForms.Components.Drawings
{
    public class VericalLine : DisplayObject
    {
        public ConsoleColor? BackgroundColor = null;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public BorderStyle Border = BorderStyle.Thick;

        public LineEndingStyle End1 = LineEndingStyle.Line;
        public LineEndingStyle End2 = LineEndingStyle.Line;

        private int _height;
        public int Height
        {
            get => _height;
            set
            {
                if (_height != value)
                {
                    _height = value;
                    Dirty = true;
                }
            }
        }

        public VericalLine(Container parent, int height = 10) : base(parent)
        {
            Height = height;
        }

        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer baseBuffer = new ConsoleCharBuffer(1, Height);

            for (int i = 0; i < Height; i++)
            {
                baseBuffer.data[0,i] = new ConsoleChar(this, DrawingHelper.GetVerticalBorder(Border)[0], false, BackgroundColor, ForegroundColor);
            }

            switch (End1)
            {
                case LineEndingStyle.T:
                    baseBuffer.data[0, 0].Char = DrawingHelper.GetBottomTJunctionBorder(Border)[0];
                    break;
                case LineEndingStyle.Plus:
                    baseBuffer.data[0, 0].Char = DrawingHelper.GetCrossJunctionBorder(Border)[0];
                    break;
            }

            switch (End2)
            {
                case LineEndingStyle.T:
                    baseBuffer.data[0, Height - 1].Char = DrawingHelper.GetTopTJunctionBorder(Border)[0];
                    break;
                case LineEndingStyle.Plus:
                    baseBuffer.data[0, Height - 1].Char = DrawingHelper.GetCrossJunctionBorder(Border)[0];
                    break;
            }

            _dirty = false;

            return baseBuffer;
        }
    }
}
