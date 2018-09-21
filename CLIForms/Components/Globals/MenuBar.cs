using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Extentions;
using CLIForms.Interfaces;

namespace CLIForms.Components.Globals
{
    public class MenuBar : DisplayObject, IFocusable, IAcceptGlobalInput, IAcceptInput
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

                if (Focused && item == FocusedNode)
                {
                    Backgrnd = item.FocusedBackgroundColor;
                    Forgrnd = item.FocusedForegroundColor;
                }

                string displayText = item.Text;

                if (item.Children.Any())
                {
                    displayText += " V";

                    if (item == FocusedNode)
                    {


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

        private void renderSubMenu(ConsoleCharBuffer buffer, MenuItem node, int xPos, int yPos)
        {


        }

        public event FocusEventHandler FocusIn;
        public event FocusEventHandler FocusOut;
        public void FireFocusIn(ConsoleKeyInfo? key)
        {
            FocusedNode = RootNode.Children.FirstOrDefault();
            Dirty = true;
        }

        public void FireFocusOut(ConsoleKeyInfo? key)
        {
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
                }
            }


            return false;
        }
    }
}
