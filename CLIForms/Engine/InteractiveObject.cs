using System;
using CLIForms.Components.Containers;
using CLIForms.Engine.Events;

namespace CLIForms.Engine
{
    public abstract class InteractiveObject : DisplayObject
    {
        public InteractiveObject(Container parent) : base(parent)
        {
        }

        // Keyboard

        public event KeyboardEvent KeyDown;
        public event KeyboardEvent KeyUp;

        internal Events.KeyboardEvent FireKeyDown(Events.KeyboardEvent evt)
        {
            if (KeyDown != null)
            {
                Delegate[] handlers = (Delegate[])KeyDown.GetInvocationList();
                foreach (KeyboardEvent evtHandler in handlers)
                {
                    if (evt._stopImmediatePropagation)
                        return evt;

                    evtHandler.Invoke(evt);
                }
                return evt;
            }
            else
            {
                return evt;
            }
        }

        internal Events.KeyboardEvent FireKeyUp(Events.KeyboardEvent evt)
        {
            if (KeyUp != null)
            {
                Delegate[] handlers = (Delegate[])KeyUp.GetInvocationList();
                foreach (KeyboardEvent evtHandler in handlers)
                {
                    if (evt._stopImmediatePropagation)
                        return evt;

                    evtHandler.Invoke(evt);
                }
                return evt;
            }
            else
            {
                return evt;
            }
        }

        public event KeyboardEvent KeyDownCapture;
        public event KeyboardEvent KeyUpCapture;

        internal Events.KeyboardEvent FireKeyDownCapture(Events.KeyboardEvent evt)
        {
            if (KeyDownCapture != null)
            {
                Delegate[] handlers = (Delegate[])KeyDownCapture.GetInvocationList();
                foreach (KeyboardEvent evtHandler in handlers)
                {
                    if (evt._stopImmediatePropagation)
                        return evt;

                    evtHandler.Invoke(evt);
                }
                return evt;
            }
            else
            {
                return evt;
            }
        }

        internal Events.KeyboardEvent FireKeyUpCapture(Events.KeyboardEvent evt)
        {
            if (KeyUpCapture != null)
            {
                Delegate[] handlers = (Delegate[])KeyUpCapture.GetInvocationList();
                foreach (KeyboardEvent evtHandler in handlers)
                {
                    if (evt._stopImmediatePropagation)
                        return evt;

                    evtHandler.Invoke(evt);
                }
                return evt;
            }
            else
            {
                return evt;
            }
        }

        // Focus

        protected bool _focused = false;
        public virtual bool Focused
        {
            get => _focused;
            set
            {
                _focused = value;
                Dirty = true;
            }
        }

        public event FocusEvent FocusIn;
        public event FocusEvent FocusOut;

        internal Event FireFocusIn(Events.FocusEvent evt, Direction vector)
        {
            if (FocusIn != null)
            {
                Delegate[] handlers = (Delegate[]) FocusIn.GetInvocationList();
                foreach (FocusEvent evtHandler in handlers)
                {
                    if (evt._stopImmediatePropagation)
                        return evt;

                    evtHandler.Invoke(evt, vector);
                }
                return evt;
            }
            else
            {
                return evt;
            }
        }

        internal Event FireFocusOut(Events.FocusEvent evt, Direction vector)
        {
            if (FocusOut != null)
            {
                Delegate[] handlers = (Delegate[])FocusOut.GetInvocationList();
                foreach (FocusEvent evtHandler in handlers)
                {
                    if (evt._stopImmediatePropagation)
                        return evt;

                    evtHandler.Invoke(evt, vector);
                }
                return evt;
            }
            else
            {
                return evt;
            }
        }

        // Activate
        public event ActivateEvent Activated;
        public event ActivateEvent ActivatedCapture;

        internal Event FireActivated(Events.Event evt)
        {
            if (Activated != null)
            {
                ActivateEvent[] handlers = (ActivateEvent[])Activated.GetInvocationList();
                foreach (ActivateEvent evtHandler in handlers)
                {
                    if (evt._stopImmediatePropagation)
                        return evt;

                    evtHandler.Invoke(evt);
                }
                return evt;
            }
            else
            {
                return evt;
            }
        }
        internal Event FireActivatedCapture(Events.Event evt)
        {
            if (ActivatedCapture != null)
            {
                ActivateEvent[] handlers = (ActivateEvent[])ActivatedCapture.GetInvocationList();
                foreach (ActivateEvent evtHandler in handlers)
                {
                    if (evt._stopImmediatePropagation)
                        return evt;

                    evtHandler.Invoke(evt);
                }
                return evt;
            }
            else
            {
                return evt;
            }
        }
    }
}
