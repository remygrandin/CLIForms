﻿using System;
using CLIForms.Buffer;

namespace CLIForms.Components.Containers
{
    public class Screen : Container
    {
        public ConsoleColor BackgroudColor = ConsoleColor.Blue;

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
                Engine.Instance.SignalDirty(this);
                base.Dirty = value;

            }
        }

        protected override ConsoleCharBuffer RenderContainer()
        {
            return new ConsoleCharBuffer(Width, Height).Clear(new ConsoleChar(owner: this, focussable: false, ch: ' ', background: BackgroudColor));
        }


    }
}
