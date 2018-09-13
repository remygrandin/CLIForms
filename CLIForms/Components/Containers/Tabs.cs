using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CLIForms.Buffer;
using CLIForms.Interfaces;
using CLIForms.Styles;

namespace CLIForms.Components.Containers
{
    public class Tabs : Container, IFocusable, IAcceptInput
    {
        public ConsoleColor? BackgroudColor = ConsoleColor.Gray;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public ConsoleColor? TitleBackgroudColor = ConsoleColor.DarkGray;
        public ConsoleColor TitleForegroundColor = ConsoleColor.Black;

        public ConsoleColor? TitleActiveBackgroudColor = ConsoleColor.Green;
        public ConsoleColor TitleActiveForegroundColor = ConsoleColor.White;

        public ConsoleColor? TitleFocusedBackgroudColor = ConsoleColor.White;
        public ConsoleColor TitleFocusedForegroundColor = ConsoleColor.Black;

        public BorderStyle Border = BorderStyle.Thick;

        public ShadowStyle Shadow = ShadowStyle.Light;

        private ObservableCollection<string> TabsTitles = new ObservableCollection<string>();
        public string _activeTab;
        public string ActiveTab
        {
            get
            {
                return _activeTab;
            }
            set
            {
                if (_activeTab != value)
                {
                    if (value != null && !TabsTitles.Contains(value))
                        throw new Exception("Invalid TabName");

                    _activeTab = value;
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
                _focused = value;
                Dirty = true;
            }
        }

        public string FocussedTab { get; internal set; }

        private Dictionary<string, List<DisplayObject>> TabsChildren = new Dictionary<string, List<DisplayObject>>();

        public event FocusEventHandler FocusIn;
        public event FocusEventHandler FocusOut;
        public event KeyPressEventHandler Keypress;

        public Tabs(Container parent, IEnumerable<string> titles, int width = 30, int height = 12) : base(width, height)
        {
            Parent = parent;
            parent.AddChild(this);

            TabsTitles.CollectionChanged += Titles_CollectionChanged;

            foreach (string title in titles)
            {
                TabsTitles.Add(title);
            }
        }

        private void Titles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Dirty = true;
        }

        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer baseBuffer = RenderContainer();

            ConsoleCharBuffer componentsBuffer = new ConsoleCharBuffer(Width - 2, Height - 2);

            if (ActiveTab != null)
            {
                foreach (DisplayObject child in TabsChildren[ActiveTab].Where(item => item.Visible))
                {
                    componentsBuffer = componentsBuffer.Merge(child.Render(), child.X, child.Y);
                }
            }

            baseBuffer.Merge(componentsBuffer, 1, 1);

            displayBuffer = baseBuffer;

            _dirty = false;

            return baseBuffer;
        }

        private List<List<string>> OrderTabForDisplay()
        {
            List<string> titlesPool = TabsTitles.ToList();

            List<string> subPool = new List<string>();
            List<List<string>> subPools = new List<List<string>>();


            for (int i = 0; i < titlesPool.Count; i++)
            {
                subPool.Add(titlesPool[i]);

                if (i == titlesPool.Count - 1 || subPool.Sum(item => item.Length) + subPool.Count - 1 + 2 > Width)
                {
                    subPool.RemoveAt(subPool.Count - 1);
                    subPools.Add(subPool);
                    subPool = new List<string>();

                    if (i != titlesPool.Count - 1)
                        i--;
                }
            }

            subPools.Reverse();

            return subPools;
        }

        protected override ConsoleCharBuffer RenderContainer()
        {
            ConsoleCharBuffer buffer = new ConsoleCharBuffer(Width + 1, Height + 1);

            List<List<string>> subPools = OrderTabForDisplay();


            for (var i = 0; i < subPools.Count; i++)
            {
                List<string> pool = subPools[i];
                               
                string header = DrawingHelper.GetTopLeftCornerBorder(Border)
                                + String.Join(DrawingHelper.GetBottomTJunctionBorder(Border), pool.Select(item => new string(DrawingHelper.GetHorizontalBorder(Border)[0], item.Length)))
                                + DrawingHelper.GetTopRightCornerBorder(Border);

                buffer.DrawString(this, header, false, 0, i * 2, BackgroudColor, ForegroundColor);

                buffer.DrawString(this, DrawingHelper.GetVerticalBorder(Border), false, 0, i * 2 + 1, BackgroudColor, ForegroundColor);

                int pos = 1;
                foreach (string str in pool)
                {
                    ConsoleColor? Backgrnd = TitleBackgroudColor;
                    ConsoleColor Forgrnd = TitleForegroundColor;

                    if (str == ActiveTab)
                    {
                        Backgrnd = TitleActiveBackgroudColor;
                        Forgrnd = TitleActiveForegroundColor;
                    }

                    if(str == FocussedTab)
                    {
                        Backgrnd = TitleFocusedBackgroudColor;
                        Forgrnd = TitleFocusedForegroundColor;
                    }


                    buffer.DrawString(this, str, true, pos, i * 2 + 1, Backgrnd, Forgrnd);

                    buffer.DrawString(this, DrawingHelper.GetVerticalBorder(Border), false, pos + str.Length, i * 2 + 1, BackgroudColor, ForegroundColor);

                    pos += str.Length + 1;
                }
            }

            // container
            DrawingHelper.DrawBlockFull(buffer, this, false, 0, subPools.Count * 2, Width, Height - subPools.Count * 2, BackgroudColor, ForegroundColor, Border, Shadow);

            // tabsFooter
            string footer = DrawingHelper.GetRightTJunctionBorder(Border)
                            + String.Join(DrawingHelper.GetTopTJunctionBorder(Border), subPools.Last().Select(item => new string(DrawingHelper.GetHorizontalBorder(Border)[0], item.Length)))
                            + DrawingHelper.GetTopTJunctionBorder(Border);

            buffer.DrawString(this, footer, false, 0, subPools.Count * 2, BackgroudColor, ForegroundColor);

            // Edge case : tabs make full length
            if (subPools.Last().Sum(item => item.Length) + subPools.Last().Count + 2 == Width)
                buffer.data[subPools.Last().Sum(item => item.Length) + subPools.Last().Count + 2, subPools.Count * 2]
                    .Char = DrawingHelper.GetLeftTJunctionBorder(Border)[0];

            return buffer;
        }

        public void AddChild(DisplayObject child, string tabName)
        {
            if(!TabsTitles.Contains(tabName))
                throw new Exception("Invalid TabName");

            if (!TabsChildren.ContainsKey(tabName))
                TabsChildren.Add(tabName, new List<DisplayObject>());
            
            if (!TabsChildren[tabName].Contains(child))
                TabsChildren[tabName].Add(child);

            Dirty = true;
        }

        public override void AddChild(DisplayObject child)
        {
            AddChild(child, ActiveTab);
        }

        public void AddChild(DisplayObject child, int tabPos)
        {
            if (tabPos < 0 || tabPos > TabsTitles.Count)
                throw new Exception("Invalid TabPos");

            AddChild(child, TabsTitles[tabPos]);
        }

        public override void RemoveChild(DisplayObject child)
        {
            foreach (KeyValuePair<string, List<DisplayObject>> tabs in TabsChildren)
            {
                if (tabs.Value.Contains(child))
                    tabs.Value.Remove(child);
            }
        }

        public void FireFocusIn(ConsoleKeyInfo? key)
        {
            return;
        }

        public void FireFocusOut(ConsoleKeyInfo? key)
        {
            return;
        }

        public bool FireKeypress(ConsoleKeyInfo key)
        {
            List<List<string>> subPools = OrderTabForDisplay();

            List<string> activePool = subPools.FirstOrDefault(item => item.Contains(ActiveTab));

            int activePoolIndex = subPools.IndexOf(activePool);
            int activeIndex = activePool.IndexOf(ActiveTab);

            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (activeIndex == 0)
                        return false;
                    ActiveTab = activePool[activeIndex - 1];
                    return true;
                    break;
                case ConsoleKey.RightArrow:
                    if (activeIndex == activePool.Count - 1)
                        return false;
                    ActiveTab = activePool[activeIndex + 1];
                    return true;
                    break;
                case ConsoleKey.UpArrow:
                    {
                        if (activePoolIndex == 0)
                            return false;

                        List<string> newPool = subPools[activePoolIndex - 1];

                        if (newPool.Count - 1 > activeIndex)
                            activeIndex = newPool.Count - 1;

                        ActiveTab = newPool[activeIndex];
                        return true;
                        break;
                    }
                case ConsoleKey.DownArrow:
                    {
                        if (activePoolIndex == subPools.Count - 1)
                            return false;

                        List<string> newPool = subPools[activePoolIndex + 1];

                        if (newPool.Count - 1 > activeIndex)
                            activeIndex = newPool.Count - 1;

                        ActiveTab = newPool[activeIndex];
                        break;
                    }
            }

            return false;
        }
    }
}
