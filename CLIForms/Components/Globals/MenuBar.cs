using System;
using System.Collections.Generic;
using System.Linq;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Extentions;
using CLIForms.Interfaces;

namespace CLIForms.Components.Globals
{
    public class MenuBar : DisplayObject, IInterractive, IAcceptGlobalInput
    {
        public ConsoleColor? BackgroundColor = ConsoleColor.Gray;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public ConsoleColor? TitleBackgroundColor = ConsoleColor.DarkGreen;
        public ConsoleColor TitleForegroundColor = ConsoleColor.White;

        public ConsoleColor? ActiveBackgroundColor = ConsoleColor.Green;
        public ConsoleColor ActiveForegroundColor = ConsoleColor.Blue;


        public ConsoleColor? FocusedBackgroundColor = ConsoleColor.Green;
        public ConsoleColor FocusedForegroundColor = ConsoleColor.Blue;

        public MenuItem RootNode;

        private MenuItem _focusedNode = null;

        public MenuItem FocusedNode
        {
            get => _focusedNode;
            set
            {
                if (_focusedNode != value)
                {
                    _focusedNode = value;
                    Dirty = true;
                }
            }
        }

        private bool _focused = false;
        public bool Focused
        {
            get => _focused;
            set
            {
                if (_focused != value)
                {
                    _focused = value;
                    Dirty = true;
                }
            }
        }



        public MenuBar(Container parent, MenuItem rootNode) : base(parent)
        {
            RootNode = rootNode;
        }

        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;



            DisplayObject rootScreenCandidate = this.Parents<DisplayObject>(item => item.Parent).LastOrDefault();

            if (!(rootScreenCandidate is Screen))
            {
                _dirty = false;
                return new ConsoleCharBuffer(0, 0);
            }

            Screen screen = (Screen)rootScreenCandidate;



            if (screen.Height == 0)
            {
                _dirty = false;
                return new ConsoleCharBuffer(0, 0);

            }

            ConsoleCharBuffer baseBuffer = new ConsoleCharBuffer(screen.Width, screen.Height);

            baseBuffer.DrawString(this, new String(' ', screen.Width), false, 0, 0, BackgroundColor, ForegroundColor);

            baseBuffer.DrawString(this, RootNode.Text, false, 0, 0, TitleBackgroundColor, TitleForegroundColor);

            foreach (MenuItem item in RootNode.Children.Flatten(item => item.Children).Where(item => item != null))
            {
                item.XPos = 0;
                item.YPos = 0;
                item.IsOpen = false;
            }

            int xOffset = RootNode.Text.Length + 1;
            foreach (MenuItem item in RootNode.Children)
            {
                ConsoleColor? Backgrnd = BackgroundColor;
                ConsoleColor Forgrnd = ForegroundColor;

                if (item.isActive)
                {
                    Backgrnd = item.ActiveBackgroundColor;
                    Forgrnd = item.ActiveForegroundColor;
                }

                if (Focused && FocusedNode.Parents(node => node.Parent).Contains(item))
                {
                    Backgrnd = item.FocusedBackgroundColor;
                    Forgrnd = item.FocusedForegroundColor;
                }

                item.XPos = xOffset;
                item.YPos = 0;

                string displayText = item.Text;

                if (item.Children.Any())
                {
                    displayText += " " + '▼';
                    if (Focused)
                    {
                        if (FocusedNode.Parents(node => node.Parent).Contains(item))
                        {
                            item.IsOpen = true;

                            RenderSubMenu(baseBuffer, item, item.XPos, item.YPos + 1);
                        }
                    }

                }

                baseBuffer.DrawString(this, displayText, true, xOffset, 0, Backgrnd, Forgrnd);
                baseBuffer.DrawString(this, " ", false, xOffset + displayText.Length, 0, BackgroundColor, ForegroundColor);
                item.XPos = xOffset;
                item.YPos = 0;

                xOffset += displayText.Length + 1;
            }






            displayBuffer = baseBuffer;

            _dirty = false;

            return baseBuffer;
        }

        private void RenderSubMenu(ConsoleCharBuffer buffer, MenuItem node, int xPos, int yPos)
        {
            if (node.Children == null || node.Children.Count == 0)
                return;

            int maxWidth = node.Children.Select(item => (item.Children != null && item.Children.Any()) ? item.Text.Length + 2 : item.Text.Length).Max() + 2;

            int yOffset = 0;

            foreach (MenuItem child in node.Children)
            {
                child.XPos = xPos;
                child.YPos = yPos + yOffset;

                ConsoleColor? Backgrnd = child.BackgroundColor;
                ConsoleColor Forgrnd = child.ForegroundColor;

                if (child.isActive)
                {
                    Backgrnd = child.ActiveBackgroundColor;
                    Forgrnd = child.ActiveForegroundColor;
                }

                if (Focused && FocusedNode.Parents(item => item.Parent).Contains(child))
                {
                    Backgrnd = child.FocusedBackgroundColor;
                    Forgrnd = child.FocusedForegroundColor;
                }

                string nodeText = " " + child.Text;

                if (child.Children != null && child.Children.Any())
                {

                    if (buffer.Width - (xPos + maxWidth) < 10)
                    {
                        nodeText += new string(' ', maxWidth - nodeText.Length - 2) + "◄ ";

                        if (FocusedNode.Parents(item => item.Parent).Contains(child))
                        {
                            child.IsOpen = true;

                            int childrenMaxWidth = node.Children.Select(item => (item.Children != null && item.Children.Any()) ? item.Text.Length + 2 : item.Text.Length).Max();

                            RenderSubMenu(buffer, child, child.XPos - childrenMaxWidth, child.YPos);
                        }

                        FocusedNode.RightSubMenu = false;
                    }
                    else
                    {
                        nodeText += new string(' ', maxWidth - nodeText.Length - 2) + "► ";

                        if (FocusedNode.Parents(item => item.Parent).Contains(child))
                        {
                            child.IsOpen = true;

                            RenderSubMenu(buffer, child, child.XPos + maxWidth, child.YPos);
                        }

                        FocusedNode.RightSubMenu = true;
                    }
                }

                nodeText = nodeText.PadRight(maxWidth, ' ');

                buffer.DrawString(this, nodeText, true, xPos, yPos + yOffset, Backgrnd, Forgrnd);

                yOffset++;
            }


        }

        public event FocusEventHandler FocusIn;
        public event FocusEventHandler FocusOut;


        public void FocusedIn(ConsoleKeyInfo? key)
        {
            FocusedNode = RootNode.Children.FirstOrDefault();
            Dirty = true;
            if (FocusIn != null)
                foreach (FocusEventHandler handler in FocusIn.GetInvocationList())
                {
                    if (handler?.Invoke(this) == true)
                        return;
                }
        }

        public void FocusedOut(ConsoleKeyInfo? key)
        {
            if (FocusOut != null)
                foreach (FocusEventHandler handler in FocusOut.GetInvocationList())
                {
                    if (handler?.Invoke(this) == true)
                        return;
                }
        }

        public bool FireGlobalKeypress(ConsoleKeyInfo key)
        {
            return false;
        }

        public bool KeyPressed(ConsoleKeyInfo key)
        {
            // Main bar
            if (RootNode.Children.Contains(FocusedNode))
            {

                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                        {
                            int index = RootNode.Children.IndexOf(FocusedNode);

                            if (index == 0)
                                return false;

                            FocusedNode = RootNode.Children[index - 1];
                            return true;
                        }

                    case ConsoleKey.RightArrow:
                        {
                            int index = RootNode.Children.IndexOf(FocusedNode);

                            if (index == RootNode.Children.Count - 1)
                                return false;

                            FocusedNode = RootNode.Children[index + 1];
                            return true;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            if (FocusedNode.Children == null || RootNode.Children.Count == 0)
                                return false;

                            FocusedNode = FocusedNode.Children.First();
                            return true;
                        }
                }
            }
            else
            {
                switch (key.Key)
                {
                    case ConsoleKey.DownArrow:
                        {
                            int index = FocusedNode.Parent.Children.IndexOf(FocusedNode);

                            if (index == FocusedNode.Parent.Children.Count - 1)
                                return false;

                            FocusedNode = FocusedNode.Parent.Children[index + 1];
                            return true;
                        }
                    case ConsoleKey.UpArrow:
                        {
                            int index = FocusedNode.Parent.Children.IndexOf(FocusedNode);

                            if (index == 0)
                            {
                                if (RootNode.Children.Contains(FocusedNode.Parent))
                                {
                                    FocusedNode = FocusedNode.Parent;
                                    return true;
                                }
                                return false;
                            }


                            FocusedNode = FocusedNode.Parent.Children[index - 1];
                            return true;
                        }

                    case ConsoleKey.LeftArrow:
                        {
                            if (FocusedNode.Children == null || FocusedNode.Children.Count == 0)
                            {
                                if (RootNode.Children.Contains(FocusedNode.Parent))
                                {
                                    return false;
                                }

                                FocusedNode = FocusedNode.Parent;
                                return true;
                            }
                            else
                            {
                                if (FocusedNode.RightSubMenu)
                                {
                                    if (RootNode.Children.Contains(FocusedNode.Parent))
                                    {
                                        return false;
                                    }

                                    FocusedNode = FocusedNode.Parent;
                                    return true;
                                }

                                FocusedNode = FocusedNode.Children.First();
                                return true;
                            }
                            
                        }
                    case ConsoleKey.RightArrow:
                    {
                        if (FocusedNode.Children == null || FocusedNode.Children.Count == 0)
                        {
                            if (RootNode.Children.Contains(FocusedNode.Parent))
                            {
                                return false;
                            }

                            FocusedNode = FocusedNode.Parent;
                            return true;
                        }
                        else
                        {
                            if (!FocusedNode.RightSubMenu)
                            {
                                if (RootNode.Children.Contains(FocusedNode.Parent))
                                {
                                    return false;
                                }

                                FocusedNode = FocusedNode.Parent;
                                return true;
                            }

                            FocusedNode = FocusedNode.Children.First();
                            return true;
                        }

                    }
                }
            }


            return false;
        }
    }
}
