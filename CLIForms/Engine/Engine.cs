using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using CLIForms.Buffer;
using CLIForms.Components.Globals;
using CLIForms.Console;
using CLIForms.Engine.Events;
using CLIForms.Extentions;
using CLIForms.Interfaces;

namespace CLIForms.Engine
{
    public sealed class Engine
    {
        public static Engine Instance { get; } = new Engine();

        private IConsole _currentConsole = null;

        private ConsoleCharBuffer _engineBuffer;
        public Engine(int width = 80, int height = 30)
        {
            _currentConsole = new WindowsConsole();


            _currentConsole.Width = width;
            _currentConsole.Height = height;

            _engineBuffer = new ConsoleCharBuffer(width, height);

            _debounceDirty.AutoReset = false;
            _debounceDirty.Elapsed += DebounceDirty_Elapsed;
        }


        public int Width
        {
            get => _currentConsole.Width;
            set
            {
                _currentConsole.Width = value;

                _engineBuffer = new ConsoleCharBuffer(Width, Height);
                ActiveScreen.Width = Width;
            }
        }

        public int Height
        {
            get => _currentConsole.Height;
            set
            {
                _currentConsole.Height = value;

                _engineBuffer = new ConsoleCharBuffer(Width, Height);
                ActiveScreen.Height = Height;
            }
        }

        private Screen _activeScreen;
        public Screen ActiveScreen
        {
            get { return _activeScreen; }
            set
            {
                _activeScreen = value;
                ForceDraw();
            }
        }

        public List<InteractiveObject> VisibleFocusableObjects = new List<InteractiveObject>();
        public List<PositionedConsoleChar> VisibleFocusableChars = new List<PositionedConsoleChar>();
        public List<DisplayObject> AllObjects = new List<DisplayObject>();

        private Timer _debounceDirty = new Timer(50);

        public void SignalDirty(Screen screen)
        {
            if (ActiveScreen == screen)
            {
                _debounceDirty.Stop();
                _debounceDirty.Start();
            }
        }


        private void DebounceDirty_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_currentConsole.Draw)
            {
                StandardDraw();
            }
        }

        public void ForceDraw()
        {
            ConsoleCharBuffer screenbuffer = ActiveScreen.Render();

            _currentConsole.Display(screenbuffer);

            _engineBuffer = screenbuffer;

            VisibleFocusableChars = _engineBuffer.dataPositioned.Where(item => item.Focussable).ToList();
            VisibleFocusableObjects = VisibleFocusableChars.Select(item => item.Owner).Distinct()
                .Where(item => item.Parents(itm => itm.Parent).All(itm => item.Disabled == false))
                .Select(item => (InteractiveObject)item).ToList();

            AllObjects = new List<DisplayObject>(ActiveScreen.GetAllChildren());
            AllObjects.Add(ActiveScreen);

            if (FocusedObject != null && !VisibleFocusableObjects.Contains(FocusedObject))
            {
                TransferFocus(FocusedObject,
                    (InteractiveObject)VisibleFocusableChars.OrderBy(item => Math.Pow(item.Y, 2) + Math.Pow(item.X, 2)).FirstOrDefault()?.Owner, Direction.None);
            }

        }

        public void StandardDraw()
        {

            ConsoleCharBuffer screenbuffer = ActiveScreen.Render();

            List<PositionedConsoleChar> diff = _engineBuffer.Diff(screenbuffer);

            _currentConsole.Display(diff);

            _engineBuffer = screenbuffer;

            VisibleFocusableChars = _engineBuffer.dataPositioned.Where(item => item.Focussable).ToList();
            VisibleFocusableObjects = VisibleFocusableChars.Select(item => item.Owner).Distinct()
                .Where(item => item.Parents(itm => itm.Parent).All(itm => item.Disabled == false))
                .Select(item => (InteractiveObject)item).ToList();

            AllObjects = new List<DisplayObject>(ActiveScreen.GetAllChildren());
            AllObjects.Add(ActiveScreen);

            if (FocusedObject != null && !VisibleFocusableObjects.Contains(FocusedObject))
            {
                TransferFocus(FocusedObject,
                    (InteractiveObject)VisibleFocusableChars.OrderBy(item => Math.Pow(item.Y, 2) + Math.Pow(item.X, 2)).FirstOrDefault()?.Owner, Direction.None);
            }
        }

        public InteractiveObject FocusedObject;

        public bool DebugEnabled = false;

        public void Start()
        {
            _currentConsole.Init();

            if (FocusedObject == null)
            {
                ForceDraw();

                TransferFocus(FocusedObject, (InteractiveObject)VisibleFocusableChars.OrderBy(item => Math.Pow(item.Y, 2) + Math.Pow(item.X, 2)).FirstOrDefault()?.Owner, Direction.None);

            }

            ForceDraw();

            _currentConsole.KeyboardEvent += CurrentConsole_KeyEvent;
            _currentConsole.StartCapture();
        }

        private void CurrentConsole_KeyEvent(Events.KeyboardEvent evt)
        {
            if (DebugEnabled)
            {
                Debug.WriteLine(evt.ToString());

                switch (evt.VirtualKeyCode)
                {
                    case VirtualKey.F12:
                        {
                            var oldBuffer = _engineBuffer;
                            _engineBuffer = new ConsoleCharBuffer(oldBuffer.Width, oldBuffer.Height);

                            for (int x = 0; x < _engineBuffer.Width; x++)
                                for (int y = 0; y < _engineBuffer.Height; y++)
                                {
                                    if (!oldBuffer.data[x, y].Focussable)
                                        _engineBuffer.data[x, y] = new ConsoleChar(oldBuffer.data[x, y].Owner, oldBuffer.data[x, y].Char, oldBuffer.data[x, y].Focussable, ConsoleColor.Blue, ConsoleColor.Black);
                                    else
                                        _engineBuffer.data[x, y] = new ConsoleChar(oldBuffer.data[x, y].Owner, oldBuffer.data[x, y].Char, oldBuffer.data[x, y].Focussable, ConsoleColor.DarkMagenta, ConsoleColor.Black);
                                }

                            _currentConsole.Display(_engineBuffer);

                            return;
                        }
                }
            }

            ForceDraw();
            bool breakAndContinue = false;

            // First we give a chance to the global listeners to intercept the key (true to break)
            Events.KeyboardEvent globalEvt = new Events.KeyboardEvent(evt, EventPhase.Target, null, null);
            foreach (IAcceptGlobalInput dp in AllObjects.OfType<IAcceptGlobalInput>())
            {
                globalEvt = new Events.KeyboardEvent(evt, EventPhase.Target, (DisplayObject)dp, (DisplayObject)dp);
                globalEvt.Cancelable = true;

                dp.FireGlobalKeypress(globalEvt);

                if (globalEvt._stopImmediatePropagation)
                {
                    breakAndContinue = true;
                    break;
                }

                if (globalEvt._stopPropagation)
                {
                    breakAndContinue = true;
                }
            }

            if (breakAndContinue)
                return;

            if (globalEvt != null && globalEvt._canceled)
                return;

            // Then we give a chance to focused object to intercept the key
            if (globalEvt.KeyDown)
                globalEvt = TriggerEvent(evt, FocusedObject, (lEvt, obj) => obj.FireKeyDownCapture(lEvt), (lEvt, obj) => obj.FireKeyDown(lEvt), FocusedObject.Parents.Cast<InteractiveObject>());
            else
                globalEvt = TriggerEvent(evt, FocusedObject, (lEvt, obj) => obj.FireKeyUpCapture(lEvt), (lEvt, obj) => obj.FireKeyUp(lEvt), FocusedObject.Parents.Cast<InteractiveObject>());



            if (globalEvt._canceled)
                return;

            // Finally we try to use it as a moving mechanism

            if (evt.KeyDown)
            {
                switch (evt.VirtualKeyCode)
                {
                    case VirtualKey.Tab:
                        CycleFocus(evt.ShiftKey ? -1 : 1);
                        break;
                    case VirtualKey.Up:
                        MoveUp();
                        break;
                    case VirtualKey.Down:
                        MoveDown();
                        break;
                    case VirtualKey.Left:
                        MoveLeft();
                        break;
                    case VirtualKey.Right:
                        MoveRight();
                        break;
                }
            }

        }

        private Dictionary<DisplayObject, List<double>> ComputeDistances(IEnumerable<PositionedConsoleChar> candidatesFocused, IEnumerable<PositionedConsoleChar> candidatesNotFocused)
        {
            Dictionary<DisplayObject, List<double>> candidates = new Dictionary<DisplayObject, List<double>>();

            foreach (PositionedConsoleChar focusedChar in candidatesFocused)
            {
                foreach (PositionedConsoleChar notFocusedChar in candidatesNotFocused)
                {
                    double distance = Math.Pow(focusedChar.X - notFocusedChar.X, 2) +
                                      Math.Pow(focusedChar.Y - notFocusedChar.Y, 2);

                    // Direct line bonus
                    if (focusedChar.X == notFocusedChar.X)
                        distance /= 2;

                    if (!candidates.ContainsKey(notFocusedChar.Owner))
                    {
                        candidates.Add(notFocusedChar.Owner, new List<double>());
                    }
                    candidates[notFocusedChar.Owner].Add(distance);

                }
            }

            return candidates;
        }


        public Event TriggerActivated(InteractiveObject masterIo)
        {
            Event evt = new Event(EventPhase.Capture, masterIo, null, true, false);

            return TriggerEvent(evt, masterIo, (lEvt, obj) => obj.FireActivatedCapture(lEvt), (lEvt, obj) => obj.FireActivated(lEvt), masterIo.Parents.Cast<InteractiveObject>());
        }


        public T1 TriggerEvent<T1, T2>(T1 evt, T2 target, Func<T1, T2, T1> captureMethod, Func<T1, T2, T1> bubbleMethod, IEnumerable<T2> parentList)
        {
            if (evt == null)
                return evt;


            bool breakAndContinue = false;

            // == capture phase ==
            IEnumerable<T2> objectParentList = parentList.Skip(1).Reverse();

            Event evtCasted = evt as Event;
            evtCasted.EventPhase = EventPhase.Capture;
            evtCasted.Target = target as DisplayObject;

            foreach (T2 io in objectParentList)
            {
                evtCasted.CurrentTarget = io as DisplayObject;

                captureMethod.Invoke(evt, io);

                if (evtCasted._stopImmediatePropagation)
                {
                    breakAndContinue = true;
                    break;
                }

                if (evtCasted._stopPropagation)
                {
                    breakAndContinue = true;
                }
            }

            if (breakAndContinue)
                return evt;

            evtCasted.CurrentTarget = target as DisplayObject;
            evtCasted.EventPhase = EventPhase.Target;

            captureMethod.Invoke(evt, target);

            if (evtCasted._stopImmediatePropagation)
                return evt;

            if (evtCasted._stopPropagation)
                return evt;


            // == bubble phase ==

            evtCasted.CurrentTarget = target as DisplayObject;
            evtCasted.EventPhase = EventPhase.Target;

            bubbleMethod.Invoke(evt, target);

            if (evtCasted._stopImmediatePropagation)
                return evt;

            if (evtCasted._stopPropagation)
                return evt;

            objectParentList = objectParentList.Reverse();

            foreach (T2 io in objectParentList)
            {
                evtCasted.CurrentTarget = io as DisplayObject;

                bubbleMethod.Invoke(evt, io);

                if (evtCasted._stopImmediatePropagation)
                {
                    breakAndContinue = true;
                    break;
                }

                if (evtCasted._stopPropagation)
                {
                    breakAndContinue = true;
                }
            }

            if (breakAndContinue)
                return evt;

            return evt;
        }

        private void MoveUp()
        {
            IEnumerable<PositionedConsoleChar> positionedChars = _engineBuffer.dataPositioned.Where(item => item.Focussable);

            int minYActive = positionedChars.Where(item => item.Owner == FocusedObject).Min(item => item.Y);

            IEnumerable<PositionedConsoleChar> candidatesFocused = positionedChars.Where(item => item.Owner == FocusedObject && item.Y == minYActive);

            IEnumerable<PositionedConsoleChar> candidatesNotFocused = positionedChars.Where(item => item.Owner != FocusedObject && item.Y < minYActive);

            if (!candidatesNotFocused.Any())
            {
                return;
            }
            // Target, Distance

            // direct up Candidate Test then full up candidate test
            int minXActive = positionedChars.Where(item => item.Owner == FocusedObject).Min(item => item.X);
            int maxXActive = positionedChars.Where(item => item.Owner == FocusedObject).Max(item => item.X);

            Dictionary<DisplayObject, List<double>> candidates = ComputeDistances(candidatesFocused,
                                                                                  candidatesNotFocused.Where(item => item.X <= maxXActive && item.X >= minXActive));

            if (candidates.Count == 0)
                candidates = ComputeDistances(candidatesFocused, candidatesNotFocused);

            double minDistance = candidates.SelectMany(item => item.Value).Min();

            IEnumerable<KeyValuePair<DisplayObject, List<double>>> candidatesShortList = candidates.Where(item => item.Value.Contains(minDistance));

            if (candidatesShortList.Count() == 1)
            {
                TransferFocus(FocusedObject, (InteractiveObject)candidatesShortList.First().Key, Direction.Up);
            }
            else
            {
                int maxDistancesCount = candidatesShortList.Select(item => item.Value.Count).Max();

                candidatesShortList = candidatesShortList.Where(item => item.Value.Count == maxDistancesCount);


                if (candidatesShortList.Count() == 1)
                {
                    TransferFocus(FocusedObject, (InteractiveObject)candidatesShortList.First().Key, Direction.Up);
                }
                else// If multiple candidate are at a tie, select the lefter one
                {
                    TransferFocus(FocusedObject, (InteractiveObject)candidatesShortList.OrderBy(item => item.Key.DisplayX).First().Key, Direction.Up);
                }
            }

        }

        private void MoveDown()
        {
            IEnumerable<PositionedConsoleChar> positionedChars = _engineBuffer.dataPositioned.Where(item => item.Focussable);

            int maxYActive = positionedChars.Where(item => item.Owner == FocusedObject).Max(item => item.Y);

            IEnumerable<PositionedConsoleChar> candidatesFocused = positionedChars.Where(item => item.Owner == FocusedObject && item.Y == maxYActive);

            IEnumerable<PositionedConsoleChar> candidatesNotFocused = positionedChars.Where(item => item.Owner != FocusedObject && item.Y > maxYActive);

            if (!candidatesNotFocused.Any())
            {
                return;
            }
            // Target, Distance

            // direct up Candidate Test then full up candidate test
            int minXActive = positionedChars.Where(item => item.Owner == FocusedObject).Min(item => item.X);
            int maxXActive = positionedChars.Where(item => item.Owner == FocusedObject).Max(item => item.X);

            Dictionary<DisplayObject, List<double>> candidates = ComputeDistances(candidatesFocused,
                candidatesNotFocused.Where(item => item.X <= maxXActive && item.X >= minXActive));

            if (candidates.Count == 0)
                candidates = ComputeDistances(candidatesFocused, candidatesNotFocused);


            double minDistance = candidates.SelectMany(item => item.Value).Min();

            IEnumerable<KeyValuePair<DisplayObject, List<double>>> candidatesShortList = candidates.Where(item => item.Value.Contains(minDistance));

            if (candidatesShortList.Count() == 1)
            {
                TransferFocus(FocusedObject, (InteractiveObject)candidatesShortList.First().Key, Direction.Down);
            }
            else
            {
                int maxDistancesCount = candidatesShortList.Select(item => item.Value.Count).Max();

                candidatesShortList = candidatesShortList.Where(item => item.Value.Count == maxDistancesCount);


                if (candidatesShortList.Count() == 1)
                {
                    TransferFocus(FocusedObject, (InteractiveObject)candidatesShortList.First().Key, Direction.Down);
                }
                else// If multiple candidate are at a tie, select the lefter one
                {
                    TransferFocus(FocusedObject, (InteractiveObject)candidatesShortList.OrderBy(item => item.Key.DisplayX).First().Key, Direction.Down);
                }
            }

        }

        private void MoveLeft()
        {
            IEnumerable<PositionedConsoleChar> positionedChars = _engineBuffer.dataPositioned.Where(item => item.Focussable);

            int minXActive = positionedChars.Where(item => item.Owner == FocusedObject).Min(item => item.X);

            IEnumerable<PositionedConsoleChar> candidatesFocused = positionedChars.Where(item => item.Owner == FocusedObject && item.X == minXActive);

            IEnumerable<PositionedConsoleChar> candidatesNotFocused = positionedChars.Where(item => item.Owner != FocusedObject && item.X < minXActive);

            if (!candidatesNotFocused.Any())
            {
                return;
            }
            // Target, Distance

            // direct up Candidate Test then full up candidate test
            int minYActive = positionedChars.Where(item => item.Owner == FocusedObject).Min(item => item.Y);
            int maxYActive = positionedChars.Where(item => item.Owner == FocusedObject).Max(item => item.Y);

            Dictionary<DisplayObject, List<double>> candidates = ComputeDistances(candidatesFocused,
                candidatesNotFocused.Where(item => item.Y <= maxYActive && item.Y >= minYActive));

            if (candidates.Count == 0)
                candidates = ComputeDistances(candidatesFocused, candidatesNotFocused);


            double minDistance = candidates.SelectMany(item => item.Value).Min();

            IEnumerable<KeyValuePair<DisplayObject, List<double>>> candidatesShortList = candidates.Where(item => item.Value.Contains(minDistance));

            if (candidatesShortList.Count() == 1)
            {
                TransferFocus(FocusedObject, (InteractiveObject)candidatesShortList.First().Key, Direction.Left);
            }
            else
            {
                int maxDistancesCount = candidatesShortList.Select(item => item.Value.Count).Max();

                candidatesShortList = candidatesShortList.Where(item => item.Value.Count == maxDistancesCount);


                if (candidatesShortList.Count() == 1)
                {
                    TransferFocus(FocusedObject, (InteractiveObject)candidatesShortList.First().Key, Direction.Left);
                }
                else// If multiple candidate are at a tie, select the upper one
                {
                    TransferFocus(FocusedObject, (InteractiveObject)candidatesShortList.OrderBy(item => item.Key.DisplayY).First().Key, Direction.Left);
                }
            }

        }

        private void MoveRight()
        {
            IEnumerable<PositionedConsoleChar> positionedChars = _engineBuffer.dataPositioned.Where(item => item.Focussable);

            int maxXActive = positionedChars.Where(item => item.Owner == FocusedObject).Max(item => item.X);

            IEnumerable<PositionedConsoleChar> candidatesFocused = positionedChars.Where(item => item.Owner == FocusedObject && item.X == maxXActive);

            IEnumerable<PositionedConsoleChar> candidatesNotFocused = positionedChars.Where(item => item.Owner != FocusedObject && item.X > maxXActive);

            if (!candidatesNotFocused.Any())
            {
                return;
            }
            // Target, Distance

            // direct up Candidate Test then full up candidate test
            int minYActive = positionedChars.Where(item => item.Owner == FocusedObject).Min(item => item.Y);
            int maxYActive = positionedChars.Where(item => item.Owner == FocusedObject).Max(item => item.Y);

            Dictionary<DisplayObject, List<double>> candidates = ComputeDistances(candidatesFocused,
                candidatesNotFocused.Where(item => item.Y <= maxYActive && item.Y >= minYActive));

            if (candidates.Count == 0)
                candidates = ComputeDistances(candidatesFocused, candidatesNotFocused);

            double minDistance = candidates.SelectMany(item => item.Value).Min();

            IEnumerable<KeyValuePair<DisplayObject, List<double>>> candidatesShortList = candidates.Where(item => item.Value.Contains(minDistance));

            if (candidatesShortList.Count() == 1)
            {
                TransferFocus(FocusedObject, (InteractiveObject)candidatesShortList.First().Key, Direction.Right);
            }
            else
            {
                int maxDistancesCount = candidatesShortList.Select(item => item.Value.Count).Max();

                candidatesShortList = candidatesShortList.Where(item => item.Value.Count == maxDistancesCount);


                if (candidatesShortList.Count() == 1)
                {
                    TransferFocus(FocusedObject, (InteractiveObject)candidatesShortList.First().Key, Direction.Right);
                }
                else// If multiple candidate are at a tie, select the upper one
                {
                    TransferFocus(FocusedObject, (InteractiveObject)candidatesShortList.OrderBy(item => item.Key.DisplayY).First().Key, Direction.Right);
                }
            }

        }

        private void CycleFocus(int direction)
        {
            int focusedIndex = VisibleFocusableObjects.IndexOf(FocusedObject);

            int nextFocusedIndex = focusedIndex + direction;

            if (nextFocusedIndex >= VisibleFocusableObjects.Count)
                nextFocusedIndex = 0;
            else if (nextFocusedIndex < 0)
                nextFocusedIndex = VisibleFocusableObjects.Count - 1;


            TransferFocus(FocusedObject, VisibleFocusableObjects[nextFocusedIndex], Direction.None);
        }

        private void TransferFocus(InteractiveObject oldDP, InteractiveObject newDP, Direction vector)
        {
            if (oldDP != null)
            {
                Events.FocusEvent evt = new Events.FocusEvent(FocusEventType.Out, EventPhase.Target, oldDP, oldDP, false, false);
                oldDP.FireFocusOut(evt, vector);
                oldDP.Focused = false;
            }

            if (newDP != null)
            {
                Events.FocusEvent evt = new Events.FocusEvent(FocusEventType.In, EventPhase.Target, oldDP, oldDP, false, false);
                newDP.FireFocusIn(evt, vector);
                newDP.Focused = true;
            }

            FocusedObject = newDP;
        }


    }
}
