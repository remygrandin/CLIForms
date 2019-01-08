using System;
using System.Collections.Generic;
using System.Linq;
using CLIForms.Buffer;
using CLIForms.Engine;
using CLIForms.Engine.Events;
using CLIForms.Styles;

namespace CLIForms.Components.Containers
{
    public class Tabs : Container
    {
        public ConsoleColor? BackgroundColor = ConsoleColor.Gray;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public ConsoleColor? TitleBackgroundColor = ConsoleColor.DarkGray;
        public ConsoleColor TitleForegroundColor = ConsoleColor.Black;

        public ConsoleColor? TitleActiveBackgroundColor = ConsoleColor.Green;
        public ConsoleColor TitleActiveForegroundColor = ConsoleColor.White;

        public ConsoleColor? TitleFocusedBackgroundColor = ConsoleColor.White;
        public ConsoleColor TitleFocusedForegroundColor = ConsoleColor.Black;

        public BorderStyle Border = BorderStyle.Thick;

        public ShadowStyle Shadow = ShadowStyle.Light;

        private List<string> TabsList = new List<string>();
        private int? _activeTab;
        public int? ActiveTab
        {
            get
            {
                return _activeTab;
            }
            set
            {
                if (_activeTab != value)
                {
                    if (value != null && (value < 0 || value > TabsList.Count))
                        throw new Exception("Invalid Tab");

                    _activeTab = value;
                    Dirty = true;
                }
            }
        }

        public string ActiveTabName
        {
            get
            {
                if (_activeTab == null)
                    return null;
                return TabsList[_activeTab.Value];
            }
            set
            {
                if (value == null)
                {
                    _activeTab = null;
                    Dirty = true;
                }
                else
                {
                    int index = TabsList.IndexOf(value);

                    if (index == -1)
                        throw new Exception("Invalid Tab");

                    if (_activeTab != index)
                    {
                        _activeTab = index;
                        Dirty = true;
                    }
                }
            }
        }

        private int _focusedTab;
        public int FocusedTab
        {
            get => _focusedTab;
            set
            {
                if (_focusedTab != value)
                {
                    _focusedTab = value;
                    Dirty = true;
                }
            }
        }

        private List<List<DisplayObject>> TabsChildren = new List<List<DisplayObject>>();

        public Tabs(Container parent, IEnumerable<string> tabs, int width = 30, int height = 12) : base(parent, width, height)
        {
            if (!tabs.Any())
                throw new Exception("Tabs list can't be empty");

            foreach (string tab in tabs)
            {
                AddTab(tab);
            }

            FocusedTab = 0;
            ActiveTab = 0;

            FocusIn += Tabs_FocusIn;
            KeyDown += Tabs_KeyDown;
        }

        public void AddTab(string name, int? pos = null)
        {
            if (pos == null)
            {
                TabsList.Add(name);
                TabsChildren.Add(new List<DisplayObject>());
            }
            else
            {
                int checkedPos = pos.Value;
                if (pos.Value < 0 || pos.Value > TabsList.Count)
                    throw new Exception("Tab Pos is outside of range");

                if (checkedPos <= _activeTab)
                {
                    TabsList.Insert(checkedPos, name);
                    TabsChildren.Insert(checkedPos, new List<DisplayObject>());
                    _activeTab++;
                }
                else
                {
                    TabsList.Insert(checkedPos, name);
                    TabsChildren.Insert(checkedPos, new List<DisplayObject>());
                }
            }

            Dirty = true;
        }

        public override IEnumerable<DisplayObject> GetSiblings(DisplayObject child)
        {
            List<DisplayObject> siblings = TabsChildren.FirstOrDefault(item => item.Contains(child));

            if (siblings == null)
                return null;

            return siblings.ToList();

        }

        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer baseBuffer = RenderContainer();

            List<List<Tuple<int, string>>> subPools = OrderTabForDisplay();

            ConsoleCharBuffer componentsBuffer = new ConsoleCharBuffer(Width - 2, Height - subPools.Count * 2 - 2);

            if (_activeTab != null)
            {
                foreach (DisplayObject child in TabsChildren[_activeTab.Value].Where(item => item.Visible))
                {
                    componentsBuffer = componentsBuffer.Merge(child.Render(), child.X, child.Y);
                }

            }



            baseBuffer.Merge(componentsBuffer, 1, subPools.Count * 2 + 1);

            displayBuffer = baseBuffer;

            _dirty = false;

            return baseBuffer;
        }

        private List<List<Tuple<int, string>>> OrderTabForDisplay()
        {
            List<string> titlesPool = TabsList.ToList();

            List<Tuple<int, string>> subPool = new List<Tuple<int, string>>();
            List<List<Tuple<int, string>>> subPools = new List<List<Tuple<int, string>>>();


            for (int i = 0; i < titlesPool.Count; i++)
            {
                subPool.Add(new Tuple<int, string>(i, titlesPool[i]));

                if (i == titlesPool.Count - 1 || subPool.Sum(item => item.Item2.Length) + subPool.Count - 1 + 2 > Width)
                {
                    if (i != titlesPool.Count - 1)
                    {
                        subPool.RemoveAt(subPool.Count - 1);
                    }

                    subPools.Add(subPool);
                    subPool = new List<Tuple<int, string>>();

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

            List<List<Tuple<int, string>>> subPools = OrderTabForDisplay();


            for (var i = 0; i < subPools.Count; i++)
            {
                List<Tuple<int, string>> pool = subPools[i];

                string header = DrawingHelper.GetTopLeftCornerBorder(Border)
                                + String.Join(DrawingHelper.GetBottomTJunctionBorder(Border), pool.Select(item => new string(DrawingHelper.GetHorizontalBorder(Border)[0], item.Item2.Length)))
                                + DrawingHelper.GetTopRightCornerBorder(Border);

                buffer.DrawString(this, header, false, 0, i * 2, BackgroundColor, ForegroundColor);

                buffer.DrawString(this, DrawingHelper.GetVerticalBorder(Border), false, 0, i * 2 + 1, BackgroundColor, ForegroundColor);

                int pos = 1;
                foreach (Tuple<int, string> tpl in pool)
                {
                    ConsoleColor? Backgrnd = TitleBackgroundColor;
                    ConsoleColor Forgrnd = TitleForegroundColor;

                    if (tpl.Item1 == ActiveTab)
                    {
                        Backgrnd = TitleActiveBackgroundColor;
                        Forgrnd = TitleActiveForegroundColor;
                    }

                    if (Focused && tpl.Item1 == FocusedTab)
                    {
                        Backgrnd = TitleFocusedBackgroundColor;
                        Forgrnd = TitleFocusedForegroundColor;
                    }


                    buffer.DrawString(this, tpl.Item2, true, pos, i * 2 + 1, Backgrnd, Forgrnd);

                    buffer.DrawString(this, DrawingHelper.GetVerticalBorder(Border), false, pos + tpl.Item2.Length, i * 2 + 1, BackgroundColor, ForegroundColor);

                    pos += tpl.Item2.Length + 1;
                }
            }

            // container
            DrawingHelper.DrawBlockFull(buffer, this, false, 0, subPools.Count * 2, Width, Height - subPools.Count * 2, BackgroundColor, ForegroundColor, Border, Shadow);

            // tabsFooter
            string footer = DrawingHelper.GetRightTJunctionBorder(Border)
                            + String.Join(DrawingHelper.GetTopTJunctionBorder(Border), subPools.Last().Select(item => new string(DrawingHelper.GetHorizontalBorder(Border)[0], item.Item2.Length)))
                            + DrawingHelper.GetTopTJunctionBorder(Border);

            buffer.DrawString(this, footer, false, 0, subPools.Count * 2, BackgroundColor, ForegroundColor);

            // Edge case : tabs make full length
            if (subPools.Last().Sum(item => item.Item2.Length) + subPools.Last().Count + 2 == Width)
                buffer.data[subPools.Last().Sum(item => item.Item2.Length) + subPools.Last().Count + 2, subPools.Count * 2].Char = DrawingHelper.GetLeftTJunctionBorder(Border)[0];

            return buffer;
        }

        public void AddChild(DisplayObject child, int tabPos)
        {
            if (tabPos < 0 || tabPos > TabsList.Count)
                throw new Exception("Invalid TabPos");

            if (!TabsChildren.Any(item => item.Contains(child)))
            {
                TabsChildren[tabPos].Add(child);

                if (child.Parent != this)
                    child.Parent = this;
            }

            Dirty = true;
        }

        public void AddChild(DisplayObject child, string tabName)
        {
            if (!TabsList.Contains(tabName))
                throw new Exception("Invalid TabName");

            AddChild(child, TabsList.IndexOf(tabName));
        }

        public override void AddChild(DisplayObject child)
        {
            if (_activeTab == null)
                throw new Exception("No Active Tab");

            AddChild(child, _activeTab.Value);
        }

        public override void RemoveChild(DisplayObject child)
        {
            foreach (List<DisplayObject> tabs in TabsChildren)
            {
                if (tabs.Contains(child))
                {
                    tabs.Remove(child);
                    if (child.Parent == this)
                        child.Parent = null;
                }
            }

            Dirty = true;
        }

        public void Tabs_FocusIn(Engine.Events.FocusEvent evt, Direction vector)
        {
            List<List<Tuple<int, string>>> subPools = OrderTabForDisplay();

            if (vector == Direction.Unknown || vector == Direction.None)
            {
                FocusedTab = subPools.First().First().Item1;
                return;
            }

            switch (vector)
            {
                case Direction.Down:
                    FocusedTab = subPools.First().First().Item1;
                    break;
                case Direction.Up:
                    FocusedTab = subPools.Last().First().Item1;
                    break;
                case Direction.Left:
                    FocusedTab = subPools.First().Last().Item1;
                    break;
                case Direction.Right:
                    FocusedTab = subPools.First().First().Item1;
                    break;
            }
        }

        public void Tabs_KeyDown(Engine.Events.KeyboardEvent evt)
        {
            if(!Focused || evt.Target != this)
                return;

            List<List<Tuple<int, string>>> subPools = OrderTabForDisplay();

            List<Tuple<int, string>> focusedPool = subPools.FirstOrDefault(item => item.Any(sitem => sitem.Item1 == FocusedTab));

            int focusedPoolIndex = subPools.IndexOf(focusedPool);
            Tuple<int, string> focusedTpl = focusedPool.FirstOrDefault(item => item.Item1 == FocusedTab);
            int focusedIndex = focusedPool.IndexOf(focusedTpl);

            switch (evt.VirtualKeyCode)
            {
                case VirtualKey.Left:
                    if (focusedIndex == 0)
                        return;
                    FocusedTab = focusedPool[focusedIndex - 1].Item1;

                    evt.StopPropagation();
                    evt.PreventDefault();
                    return;
                case VirtualKey.Right:
                    if (focusedIndex == focusedPool.Count - 1)
                        return;
                    FocusedTab = focusedPool[focusedIndex + 1].Item1;

                    evt.StopPropagation();
                    evt.PreventDefault();
                    return;
                case VirtualKey.Up:
                    {
                        if (focusedPoolIndex == 0)
                            return;

                        List<Tuple<int, string>> newPool = subPools[focusedPoolIndex - 1];

                        if (newPool.Count - 1 < focusedIndex)
                            focusedIndex = newPool.Count - 1;

                        FocusedTab = newPool[focusedIndex].Item1;

                        evt.StopPropagation();
                        evt.PreventDefault();
                        return;
                    }
                case VirtualKey.Down:
                    {
                        if (focusedPoolIndex == subPools.Count - 1)
                            return;

                        List<Tuple<int, string>> newPool = subPools[focusedPoolIndex + 1];

                        if (newPool.Count - 1 < focusedIndex)
                            focusedIndex = newPool.Count - 1;

                        FocusedTab = newPool[focusedIndex].Item1;

                        evt.StopPropagation();
                        evt.PreventDefault();
                        return;
                    }
                case VirtualKey.Space:
                case VirtualKey.Enter:
                    {
                        ActiveTab = FocusedTab;

                        evt.StopPropagation();
                        evt.PreventDefault();
                        return;
                    }
            }

            return;
        }
    }
}
