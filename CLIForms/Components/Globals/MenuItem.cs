using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Extentions;
using CLIForms.Interfaces;

namespace CLIForms.Components.Globals
{
    public class MenuItem
    {
        public ConsoleColor? BackgroundColor = ConsoleColor.Gray;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        
        public ConsoleColor? ActiveBackgroundColor = ConsoleColor.Green;
        public ConsoleColor ActiveForegroundColor = ConsoleColor.Blue;


        public ConsoleColor? FocusedBackgroundColor = ConsoleColor.Green;
        public ConsoleColor FocusedForegroundColor = ConsoleColor.Blue;

        public string Text;

        public char? HotChar;

        public List<MenuItem> Children = new List<MenuItem>();

        public bool isActive = false;
        internal bool IsOpen = false;
        internal int XPos = 0;
        internal int YPos = 0;

        private static int NextId = 0;

        public string Id = (NextId++).ToString();

        public MenuItem(string text)
        {
            Text = text;
        }


        public MenuItem(string text, char? hotChar, bool inheritStyle = true, params MenuItem[] children)
        {
            Text = text;

            HotChar = hotChar;

            Children = children?.ToList() ?? new List<MenuItem>();

            if (inheritStyle && Children != null)
            {
                foreach (MenuItem menuItem in Children)
                {
                    menuItem.BackgroundColor = this.BackgroundColor;
                    menuItem.ForegroundColor = this.ForegroundColor;

                    menuItem.ActiveBackgroundColor = this.ActiveBackgroundColor;
                    menuItem.ActiveForegroundColor = this.ActiveForegroundColor;
                    
                    menuItem.FocusedBackgroundColor = this.FocusedBackgroundColor;
                    menuItem.FocusedForegroundColor = this.FocusedForegroundColor;
                }
            }
        }


        public MenuItem(string text, params MenuItem[] children)
        {
            Text = text;
            
            Children = children?.ToList() ?? new List<MenuItem>();

            if (Children != null)
            {
                foreach (MenuItem menuItem in Children)
                {
                    menuItem.BackgroundColor = this.BackgroundColor;
                    menuItem.ForegroundColor = this.ForegroundColor;

                    menuItem.ActiveBackgroundColor = this.ActiveBackgroundColor;
                    menuItem.ActiveForegroundColor = this.ActiveForegroundColor;

                    menuItem.FocusedBackgroundColor = this.FocusedBackgroundColor;
                    menuItem.FocusedForegroundColor = this.FocusedForegroundColor;
                }
            }
        }
    }
}
