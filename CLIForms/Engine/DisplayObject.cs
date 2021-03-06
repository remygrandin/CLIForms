﻿using System.Collections.Generic;
using CLIForms.Buffer;
using CLIForms.Components.Containers;
using CLIForms.Extentions;

namespace CLIForms.Engine
{
    public abstract class DisplayObject
    {
        public DisplayObject(Container parent) {
            Parent = parent;
        }

        protected int _x;

        public int X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                    Dirty = true;
                }
            }
        }
        protected int _y;

        public int Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    Dirty = true;
                }
            }
        }

        protected ConsoleCharBuffer displayBuffer;

        public int DisplayX
        {
            get
            {
                if (Parent == null)
                    return X;
                else
                    return Parent.DisplayX + GetXOffset(this) + X;
            }
        }

        internal virtual int GetXOffset(DisplayObject child) // used in case of scrolling
        {
            return 0;
        }

        public int DisplayY
        {
            get
            {
                if (Parent == null)
                    return Y;
                else
                    return Parent.DisplayY + GetYOffset(this) + Y;
            }
        }

        internal virtual int GetYOffset(DisplayObject child) // used in case of scrolling
        {
            return 0;
        }

        private Container _parent;
        public Container Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != value)
                {
                    _parent?.RemoveChild(this);

                    _parent = value;
                    _parent?.AddChild(this);
                    Dirty = true;
                }
            }
        }

        public IEnumerable<DisplayObject> Parents{
            get { return this.Parents(item => item.Parent); }
        }

        private bool _disabled = false;
        public bool Disabled
        {
            get { return _disabled; }
            set
            {
                if (_disabled != value)
                {
                    _disabled = value;
                    Dirty = true;
                }
            }
        }


        protected bool _dirty = false;

        public virtual bool Dirty
        {
            get => _dirty;
            set
            {
                if (_dirty != value)
                {
                    _dirty = value;

                    if (_dirty)
                    {
                        if (Parent != null) Parent.Dirty = true;
                    }
                }
            }
        }

        private bool _visible = true;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    Dirty = true;
                }
            }
        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

        public abstract ConsoleCharBuffer Render();
    }
}
