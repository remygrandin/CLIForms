using System;
using CLIForms.Extentions;

namespace CLIForms.Engine.Events
{
    public class MouseEvent : Event
    {
        public bool AltKey { get; private set; }
        public bool ControlKey { get; private set; }
        public bool ShiftKey { get; private set; }

        public bool ButtonDown { get; private set; }


        public MouseEvent(EventPhase eventPhase, 
            DisplayObject target, 
            DisplayObject currentTarget,

            bool buttonDown = true,
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

            ButtonDown = buttonDown;
        }

        public MouseEvent(
            MouseEvent mouseEvent,
            EventPhase eventPhase,
            DisplayObject target,
            DisplayObject currentTarget)
            : base(eventPhase, target, currentTarget, mouseEvent.Bubbles, mouseEvent.Cancelable)
        {
            this.AltKey = mouseEvent.AltKey;
            this.ControlKey = mouseEvent.ControlKey;
            this.ShiftKey = mouseEvent.ShiftKey;

            this.ButtonDown = mouseEvent.ButtonDown;

        }

        public override string ToString()
        {
            return "[MouseEvent " +
                   ": ButtonDown=" + this.ButtonDown +
                   ", AltKey=" + this.AltKey +
                   ", ControlKey=" + this.ControlKey +
                   ", ShiftKey=" + this.ShiftKey +
                   "]";
        }
    }
}
