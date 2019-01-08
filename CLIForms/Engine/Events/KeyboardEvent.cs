using System;
using CLIForms.Extentions;

namespace CLIForms.Engine.Events
{
    public class KeyboardEvent : Event
    {
        public bool AltKey { get; private set; }
        public bool ControlKey { get; private set; }
        public bool ShiftKey { get; private set; }

        public bool KeyDown { get; private set; }

        public Char CharCode { get; private set; }
        public Byte AsciiCode { get; private set; }
        public VirtualKey VirtualKeyCode { get; private set; }

        public KeyboardEvent(EventPhase eventPhase, 
            DisplayObject target, 
            DisplayObject currentTarget,

            Char charCode,
            Byte asciiCode,
            VirtualKey virtualKeyCode,


            bool keyDown = true,
            bool bubbles = true,
            bool cancelable = false,
            bool altKey = false, 
            bool controlKey = false,
            bool shiftKey = false) 
            : base(eventPhase, target, currentTarget, bubbles, cancelable)
        {
            AltKey = altKey;
            ControlKey = controlKey;
            ShiftKey = shiftKey;

            KeyDown = keyDown;

            CharCode = charCode;
            AsciiCode = asciiCode;
            VirtualKeyCode = virtualKeyCode;
        }

        public KeyboardEvent(
            KeyboardEvent keyboardEvent,
            EventPhase eventPhase,
            DisplayObject target,
            DisplayObject currentTarget)
            : base(eventPhase, target, currentTarget, keyboardEvent.Bubbles, keyboardEvent.Cancelable)
        {
            this.AltKey = keyboardEvent.AltKey;
            this.ControlKey = keyboardEvent.ControlKey;
            this.ShiftKey = keyboardEvent.ShiftKey;

            this.KeyDown = keyboardEvent.KeyDown;

            this.CharCode = keyboardEvent.CharCode;
            this.AsciiCode = keyboardEvent.AsciiCode;
            this.VirtualKeyCode = keyboardEvent.VirtualKeyCode;
        }

        public override string ToString()
        {
            return "[KeyboardEvent " +
                   ": Keydown=" + this.KeyDown +
                   ", CharCode=" + (this.CharCode.IsPrintable() ? this.CharCode.ToString() : "unprintable") +
                   ", AsciiCode=" + this.AsciiCode +
                   ", VirtualKeyCode=" + Enum.GetName(typeof(VirtualKey), VirtualKeyCode) +
                   ", AltKey=" + this.AltKey +
                   ", ControlKey=" + this.ControlKey +
                   ", ShiftKey=" + this.ShiftKey +
                   "]";
        }
    }
}
