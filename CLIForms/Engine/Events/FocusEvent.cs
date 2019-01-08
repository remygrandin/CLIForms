namespace CLIForms.Engine.Events
{
    public enum FocusEventType
    {
        In,
        Out
    }

    public class FocusEvent : Event
    {
        public FocusEvent(FocusEventType eventType, EventPhase eventPhase, DisplayObject target, DisplayObject currentTarget, bool bubbles = true, bool cancelable = false) : base(eventPhase, target, currentTarget, bubbles, cancelable)
        {
            EventType = eventType;
        }

        private FocusEventType EventType;
    }
}
