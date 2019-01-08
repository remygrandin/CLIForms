namespace CLIForms.Engine.Events
{
    public enum EventPhase
    {
        /// <summary>
        /// Used by the root emitter
        /// </summary>
        Root,
        Capture,
        Target,
        Bubbling
    }

    public class Event
    {
        public EventPhase EventPhase { get; internal set; }
        public bool Bubbles { get; internal set; }
        public bool Cancelable { get; internal set; }
        public DisplayObject CurrentTarget { get; internal set; }
        public DisplayObject Target { get; internal set; }

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

        internal bool _canceled = false;

        public void PreventDefault()
        {
            _canceled = true;
        }
    }
}
