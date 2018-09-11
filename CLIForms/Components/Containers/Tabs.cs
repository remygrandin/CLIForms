using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CLIForms.Buffer;
using CLIForms.Extentions;
using CLIForms.Styles;

namespace CLIForms.Components.Containers
{
    public class Tabs : Container
    {
        public ConsoleColor? BackgroudColor = ConsoleColor.Gray;
        public ConsoleColor ForegroundColor = ConsoleColor.Black;

        public ConsoleColor? TitleBackgroudColor = ConsoleColor.DarkGray;
        public ConsoleColor TitleForegroundColor = ConsoleColor.Black;

        public ConsoleColor? TitleSelectedBackgroudColor = ConsoleColor.Green;
        public ConsoleColor TitleSelectedForegroundColor = ConsoleColor.White;

        public ConsoleColor? TitleCursorBackgroudColor = ConsoleColor.White;
        public ConsoleColor TitleCursorForegroundColor = ConsoleColor.Black;

        public BorderStyle Border = BorderStyle.Thick;

        public ShadowStyle Shadow = ShadowStyle.Light;

        private ObservableCollection<string> Titles = new ObservableCollection<string>();


        public Tabs(Container parent, IEnumerable<string> titles, int width = 30, int height = 12) : base(width, height)
        {
            Parent = parent;
            parent.AddChild(this);

            Titles.CollectionChanged += Titles_CollectionChanged;

            foreach (string title in titles)
            {
                Titles.Add(title);
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

            foreach (DisplayObject child in Children.Where(item => item.Visible))
            {
                componentsBuffer = componentsBuffer.Merge(child.Render(), child.X, child.Y);


            }

            baseBuffer.Merge(componentsBuffer, 1, 1);

            displayBuffer = baseBuffer;

            _dirty = false;

            return baseBuffer;
        }

        protected override ConsoleCharBuffer RenderContainer()
        {
            ConsoleCharBuffer buffer = new ConsoleCharBuffer(Width + 1, Height + 1);

            List<string> titlesPool = Titles.ToList();

            List<string> subPool = new List<string>();
            List<string[]> subPools = new List<string[]>();

            for (int i = 0; i < titlesPool.Count; i++)
            {
                subPool.Add(titlesPool[i]);

                if (i == titlesPool.Count - 1 || subPool.Sum(item => item.Length) + subPool.Count - 1 + 2 > Width)
                {
                    subPool.RemoveAt(subPool.Count - 1);
                    subPools.Add(subPool.ToArray());
                    subPool = new List<string>();

                    if (i != titlesPool.Count - 1)
                        i--;
                }
            }

            subPools.Reverse();


            for (var i = 0; i < subPools.Count; i++)
            {
                string[] pool = subPools[i];



                string header = DrawingHelper.GetTopLeftCornerBorder(Border)
                                + String.Join(DrawingHelper.GetBottomTJunctionBorder(Border), pool.Select(item => new string(DrawingHelper.GetHorizontalBorder(Border)[0], item.Length)))
                                + DrawingHelper.GetTopRightCornerBorder(Border);

                buffer.DrawString(this, header, false, 0, i * 2, BackgroudColor, ForegroundColor);

                buffer.DrawString(this, DrawingHelper.GetVerticalBorder(Border), false, 0, i * 2 + 1, BackgroudColor, ForegroundColor);

                int pos = 1;
                foreach (string str in pool)
                {
                    buffer.DrawString(this, str, true, pos, i * 2 + 1, TitleBackgroudColor, TitleForegroundColor);

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
            if (subPools.Last().Sum(item => item.Length) + subPools.Last().Length + 2 == Width)
                buffer.data[subPools.Last().Sum(item => item.Length) + subPools.Last().Length + 2, subPools.Count * 2]
                    .Char = DrawingHelper.GetLeftTJunctionBorder(Border)[0];
            
            return buffer;
        }
    }
}
