using System;
using CLIForms.Buffer;
using CLIForms.Components.Containers;

namespace CLIForms.Components.Globals
{
    public class Screen : Container
    {
        public ConsoleColor BackgroundColor = ConsoleColor.Blue;

        public Screen(int width, int height) : base(null, width, height)
        {
        }

        public Screen(Engine engine) : base(null, engine.Width, engine.Height)
        {
        }

        public Screen() : base(null, Engine.Instance.Width, Engine.Instance.Height)
        {
        }


        public override bool Dirty
        {
            get => base.Dirty;
            set
            {
                base.Dirty = value;
                Engine.Instance.SignalDirty(this);
                

            }
        }

        protected override ConsoleCharBuffer RenderContainer()
        {
            return new ConsoleCharBuffer(Width, Height).Clear(new ConsoleChar(owner: this, focussable: false, ch: ' ', background: BackgroundColor));
        }


    }
}
