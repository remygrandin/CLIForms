using System;

namespace CLIForms.Components
{
    public class Screen : Container
    {
        public ConsoleColor BackgroudColor = ConsoleColor.Blue;

        public Screen(int width, int height) : base(width, height)
        {
        }

        public Screen(Engine engine) : base(engine.Width, engine.Height)
        {
        }

        public Screen() : base(Engine.Instance.Width, Engine.Instance.Height)
        {
        }


        public override bool Dirty
        {
            get => base.Dirty;
            set
            {
                Engine.Instance.SignalDirty(this);
                base.Dirty = value;

            }
        }

        protected override ConsoleCharBuffer RenderContainer()
        {
            return new ConsoleCharBuffer(Width, Height).Clear(new ConsoleChar(owner:this,ch:' ',background: BackgroudColor));
        }


    }
}
