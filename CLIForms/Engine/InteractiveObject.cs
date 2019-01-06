using CLIForms.Components.Containers;

namespace CLIForms.Engine
{
    public abstract class InteractiveObject : DisplayObject
    {
        public InteractiveObject(Container parent) : base(parent)
        {
        }

        public virtual event KeyboardEvent KeyDown;
        public virtual event KeyboardEvent KeyUp;

        public virtual bool Focused { get; internal set; }

        public virtual event FocusEvent FocusIn;
        public virtual event FocusEvent FocusOut;

    }
}
