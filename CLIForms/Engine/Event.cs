
using CLIForms.Components;

namespace CLIForms.Engine
{
    public enum EventPhase
    {
        Capture,
        Target,
        Bubble
    }

    public class Event
    {
        public EventPhase Phase;
        public bool Cancellable;
        public DisplayObject CurrentTarget;
        public DisplayObject Target;

    }
}
