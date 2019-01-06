using System;

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

        public KeyboardEvent(EventPhase eventPhase, 
            DisplayObject target, 
            DisplayObject currentTarget,

            Char charCode,
            Byte asciiCode,

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
        }
    }
}
