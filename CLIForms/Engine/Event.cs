
using CLIForms.Components;

namespace CLIForms.Engine
{
    public enum EventPhase
    {
        Capture,
        Target,
        Bubbling
    }

    public class Event
    {
        public EventPhase EventPhase { get; protected set; }
        public bool Cancellable { get; protected set; }
        public DisplayObject CurrentTarget { get; protected set; }
        public DisplayObject Target { get; protected set; }

        internal bool _stopPropagation = false;
        internal bool _stopImmediatePropagation = false;

        public void StopPropagation()
        {
            _stopPropagation = true;
        }

        public void StopImmediatePropagation()
        {
            _stopPropagation = true;
            _stopImmediatePropagation = true;
        }
    }
}
