﻿using System;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Engine;
using CLIForms.Styles;

namespace CLIForms.Components.Drawings
{
    public class Box : DisplayObject
    {
        public ConsoleColor? BackgroundColor = null;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public BorderStyle Border = BorderStyle.Thick;

        public ShadowStyle Shadow = ShadowStyle.Light;

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

        public Box(Container parent, int width = 30, int height = 12) : base(parent)
        {
            Width = width;
            Height = height;
        }
        
        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer baseBuffer = new ConsoleCharBuffer(Width + 1, Height + 1);

            DrawingHelper.DrawBlockFull(baseBuffer, this, false, 0, 0, Width, Height, BackgroundColor, ForegroundColor, Border, Shadow);

            _dirty = false;

            return baseBuffer;
        }
    }
}
