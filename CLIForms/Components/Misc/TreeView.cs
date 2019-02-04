using System;
using System.Collections.Generic;
using System.Linq;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Components.Globals;
using CLIForms.Engine;

namespace CLIForms.Components.Misc
{
    public class TreeView : InteractiveObject
    {
        public ConsoleColor? BackgroundColor = ConsoleColor.DarkGray;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public ConsoleColor? FocusedBackgroundColor = ConsoleColor.DarkMagenta;
        public ConsoleColor FocusedForegroundColor = ConsoleColor.Black;

        public ConsoleColor? CursorBackGroundColor = ConsoleColor.White;
        public ConsoleColor CursorForegroundColor = ConsoleColor.Black;

        public List<MenuItem> RootNodes;

        public MenuItem SelectedNode;

        private int _width;
        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                Dirty = true;
            }
        }

        private int _height;
        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                Dirty = true;
            }
        }


        public TreeView(Container parent, int width = 10, int height = 20) : base(parent)
        {
            _width = width;
            _height = height;
        }

        private int _displayOffset = 0;
        private int _cursorOffset = 0;

        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer buffer = new ConsoleCharBuffer(Width, Height);

            int yOffset = 0;

            foreach (MenuItem item in RootNodes)
            {
                string prefix = "  ";
                if (item.Children.Count == 0)
                    prefix = "   ";
                else
                {
                    if(item.IsOpen)
                        prefix = "[-]";
                    else
                        prefix = "[+]";
                }

                buffer.DrawString(this, prefix + " " + item.Text,true, 0, yOffset, BackgroundColor, ForegroundColor);
                yOffset++;


            }

            Dirty = false;

            return buffer;
        }
    }
}
