using System.Collections.Generic;
using System.Linq;
using CLIForms.Buffer;

namespace CLIForms.Components.Containers
{
    public class Container : DisplayObject
    {
        internal List<DisplayObject> Children = new List<DisplayObject>();

        public Container(int width, int height)
        {
            _width = width;
            _height = height;
        }

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

        public virtual IEnumerable<DisplayObject> GetAllChildren()
        {
            List<DisplayObject> dpList = new List<DisplayObject>();

            foreach (DisplayObject child in Children)
            {
                dpList.Add(child);
                if (child is Container container)
                {
                    dpList.AddRange(container.GetAllChildren());
                }
            }

            return dpList;
        }


        public virtual void AddChild(DisplayObject child)
        {
            if (!Children.Contains(child))
                Children.Add(child);

            Dirty = true;
        }

        public virtual void RemoveChild(DisplayObject child)
        {
            if (Children.Contains(child))
                Children.Remove(child);

            Dirty = true;
        }

        public override ConsoleCharBuffer Render()
        {
            if (!_dirty && displayBuffer != null)
                return displayBuffer;

            ConsoleCharBuffer baseBuffer = RenderContainer();

            foreach (DisplayObject child in Children.Where(item => item.Visible))
            {
                baseBuffer = baseBuffer.Merge(child.Render(), child.X, child.Y);
            }

            displayBuffer = baseBuffer;

            _dirty = false;

            return baseBuffer;
        }

        protected virtual ConsoleCharBuffer RenderContainer()
        {
            return new ConsoleCharBuffer(Width, Height);
        }
    }
}
