namespace CLIForms.Engine.Events
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
        public bool Bubbles { get; protected set; }
        public bool Cancelable { get; protected set; }
        public DisplayObject CurrentTarget { get; internal set; }
        public DisplayObject Target { get; protected set; }

        public Event(EventPhase eventPhase, DisplayObject target, DisplayObject currentTarget, bool bubbles = true, bool cancelable = false)
        {
            EventPhase = eventPhase;
            Target = target;
            CurrentTarget = currentTarget;
            Bubbles = bubbles;
            Cancelable = cancelable;
        }

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
