namespace CLIForms.Components
{
    public abstract class DisplayObject
    {
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
                    return Parent.DisplayX + X;
            }
        }

        public int DisplayY
        {
            get
            {
                if (Parent == null)
                    return Y;
                else
                    return Parent.DisplayY + Y;
            }
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
                    _parent.AddChild(this);
                    Dirty = true;
                }
            }
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

        public abstract ConsoleCharBuffer Render();
    }
}
