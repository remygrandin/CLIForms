using System;
using System.Collections.Generic;
using System.Linq;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Components.Globals;
using CLIForms.Engine;
using CLIForms.Engine.Events;
using CLIForms.Extentions;
using CLIForms.Styles;

namespace CLIForms.Components.Misc
{
    public class TreeView : InteractiveObject
    {
        public ConsoleColor? BackgroundColor = ConsoleColor.DarkGray;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public ConsoleColor? ActiveBackgroundColor = ConsoleColor.DarkMagenta;
        public ConsoleColor ActiveForegroundColor = ConsoleColor.Black;

        public ConsoleColor? CursorBackGroundColor = ConsoleColor.White;
        public ConsoleColor CursorForegroundColor = ConsoleColor.Black;

        public List<MenuItem> _rootNodes;

        public List<MenuItem> RootNodes
        {
            get => _rootNodes;
            set
            {
                _rootNodes = value;
                Dirty = true;
            }
        }

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

        public TreeView(Container parent, int width = 30, int height = 50) : base(parent)
        {
            _width = width;
            _height = height;

            FocusIn += TreeView_FocusIn;
            KeyDown += TreeView_KeyDown;
        }

        private List<MenuItem> FlatNodes
        {
            get
            {
                List<MenuItem> items = new List<MenuItem>();

                foreach (MenuItem item in RootNodes)
                {
                    items.AddRange(item.FlattOpenChildren);
                }

                return items;
            }
        }

        private void TreeView_KeyDown(Engine.Events.KeyboardEvent evt)
        {
            switch (evt.VirtualKeyCode)
            {
                case VirtualKey.Up:
                    {
                        List<MenuItem> flatNodes = FlatNodes;

                        if (!flatNodes.IsFirst(FocusedItem))
                            FocusedItem = flatNodes.Prev(FocusedItem);
                    }
                    break;
                case VirtualKey.Down:
                    {
                        List<MenuItem> flatNodes = FlatNodes;

                        if (!flatNodes.IsLast(FocusedItem))
                            FocusedItem = flatNodes.Next(FocusedItem);
                    }
                    break;
                case VirtualKey.Right:
                    if (FocusedItem.HasChildren)
                    {
                        FocusedItem.IsOpen = true;
                        FocusedItem = FocusedItem.Children.First();
                    }

                    break;
                case VirtualKey.Left:
                    if (FocusedItem.Parent != null)
                    {
                        FocusedItem = FocusedItem.Parent;
                    }
                    break;
                case VirtualKey.Space:
                    if (FocusedItem.HasChildren)
                    {
                        FocusedItem.IsOpen = !FocusedItem.IsOpen;
                        Dirty = true;
                    }
                    break;

            }


        }

        private MenuItem _focusedItem = null;
        public MenuItem FocusedItem
        {
            get => _focusedItem;
            set
            {
                _focusedItem = value;
                Dirty = true;
            }
        }


        private void TreeView_FocusIn(Engine.Events.FocusEvent evt, Direction vector)
        {
            if (FocusedItem == null)
            {
                FocusedItem = RootNodes.First();
            }
        }

        private int _displayOffset = 0;
        private int _cursorOffset = 0;

        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;


            ConsoleCharBuffer frameBuffer = new ConsoleCharBuffer(Width, Height);
            frameBuffer.Clear(new ConsoleChar(this, ' ', true, BackgroundColor, ForegroundColor));

            ConsoleCharBuffer treeBuffer = new ConsoleCharBuffer(Width, 0);

            int yOffset = 0;
            foreach (MenuItem item in RootNodes)
            {
                yOffset = ItemRender(treeBuffer, yOffset, item);
            }

            frameBuffer.Merge(treeBuffer, 0, 0);

            displayBuffer = frameBuffer;

            Dirty = false;

            return frameBuffer;
        }

        private int ItemRender(ConsoleCharBuffer buffer, int yOffset, MenuItem item)
        {
            string prefix = "";


            foreach (MenuItem itemParent in item.Parents.Reverse())
            {
                if (itemParent.Parent != null)
                {
                    if (itemParent == item)
                    {

                        if (itemParent.Parent.Children.IsLast(itemParent))
                        {
                            // └─
                            prefix += " " + DrawingHelper.GetBottomLeftCornerBorder(BorderStyle.Thin) +
                                      DrawingHelper.GetHorizontalBorder(BorderStyle.Thin);
                        }
                        else
                        {
                            // ├─
                            prefix += " " + DrawingHelper.GetRightTJunctionBorder(BorderStyle.Thin) +
                                      DrawingHelper.GetHorizontalBorder(BorderStyle.Thin);
                        }
                    }
                    else
                    {

                        if (itemParent.Parent.Children.IsLast(itemParent))
                        {
                            //  
                            prefix += "   ";
                        }
                        else
                        {
                            // │
                            prefix += " " + DrawingHelper.GetVerticalBorder(BorderStyle.Thin) +
                                      " ";
                        }
                    }
                }
            }



            if (!item.HasChildren)
            {
                prefix += DrawingHelper.GetHorizontalBorder(BorderStyle.Thin) +
                          DrawingHelper.GetHorizontalBorder(BorderStyle.Thin) +
                          DrawingHelper.GetHorizontalBorder(BorderStyle.Thin) +
                          " ";
            }
            else
            {
                if (item.IsOpen)
                    prefix += "[-] ";
                else
                    prefix += "[+] ";
            }


            ConsoleCharBuffer subBuff = new ConsoleCharBuffer(Width, 1);

            if (Focused && item == FocusedItem)
                subBuff.DrawString(this, (prefix + item.Text).PadRight(Width, ' '), true, 0, 0, ActiveBackgroundColor, ActiveForegroundColor);
            else
                subBuff.DrawString(this, (prefix + item.Text).PadRight(Width, ' '), true, 0, 0, BackgroundColor, ForegroundColor);

            buffer.Merge(subBuff, 0, yOffset, true);

            yOffset++;
            if (item.IsOpen)
            {
                foreach (MenuItem child in item.Children)
                {
                    yOffset = ItemRender(buffer, yOffset, child);
                }
            }

            return yOffset;
        }
    }
}
