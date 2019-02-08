using CLIForms.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CLIForms.Components.Globals
{
    public class MenuItem
    {
        public ConsoleColor? BackgroundColor = ConsoleColor.DarkGray;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;


        public ConsoleColor? ActiveBackgroundColor = ConsoleColor.White;
        public ConsoleColor ActiveForegroundColor = ConsoleColor.Black;


        public ConsoleColor? FocusedBackgroundColor = ConsoleColor.Green;
        public ConsoleColor FocusedForegroundColor = ConsoleColor.Blue;

        public string Text;

        public char? HotChar;

        internal bool RightSubMenu = true;

        protected List<MenuItem> _children = new List<MenuItem>();
        public virtual List<MenuItem> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        public virtual bool HasChildren => Children != null && Children.Count != 0;

        public bool isActive = false;
        internal bool IsOpen;

        public int Depth => this.Parents.Count() - 1;
        internal int XPos = 0;
        internal int YPos = 0;

        private static int NextId = 0;

        public string Id = (NextId++).ToString();

        public MenuItem(string text)
        {
            Text = text;
        }

        public MenuItem(MenuItem parent, string text)
        {
            Text = text;
            _parent = parent;
        }

        private MenuItem _parent;
        public MenuItem Parent
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    _parent?.Children.Remove(this);

                    _parent = value;

                    _parent.Children.Add(this);

                }
            }
        }

        public List<MenuItem> FlattOpenChildren
        {
            get
            {
                List<MenuItem> items = new List<MenuItem>();

                items.Add(this);

                if (HasChildren && IsOpen)
                {
                    foreach (MenuItem menuItem in Children)
                    {
                        items.AddRange(menuItem.FlattOpenChildren);
                    }

                }

                return items;
            }
        }

        public IEnumerable<MenuItem> Parents
        {
            get { return this.Parents(item => item.Parent); }
        }



        public MenuItem(string text, char? hotChar, bool inheritStyle, params MenuItem[] children)
        {
            Text = text;

            HotChar = hotChar;

            Children = new List<MenuItem>();

            foreach (MenuItem item in children?.ToList() ?? new List<MenuItem>())
            {
                item.Parent = this;
            }


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

            Children = new List<MenuItem>();

            foreach (MenuItem item in children?.ToList() ?? new List<MenuItem>())
            {
                item.Parent = this;
            }

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
