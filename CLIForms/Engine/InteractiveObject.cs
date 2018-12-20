using System;
using CLIForms.Components.Containers;
using CLIForms.Interfaces;

namespace CLIForms.Components
{
    public abstract class InteractiveObject : DisplayObject
    {
        public InteractiveObject(Container parent) : base(parent)
        {

        }

        public virtual bool KeyPressed(ConsoleKeyInfo key);

        public virtual bool Focused { get; set; }

        public virtual event FocusEventHandler FocusIn;
        public virtual event FocusEventHandler FocusOut;

        public virtual void FocusedIn(ConsoleKeyInfo? key);
        public virtual void FocusedOut(ConsoleKeyInfo? key);

    }
}
