using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using CLIForms.Buffer;
using CLIForms.Components;
using CLIForms.Components.Globals;
using CLIForms.Console;
using CLIForms.Extentions;
using CLIForms.Interfaces;

namespace CLIForms
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

        public List<IInterractive> VisibleFocusableObjects = new List<IInterractive>();
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
                .Select(item => (IInterractive)item).ToList();

            AllObjects = new List<DisplayObject>(ActiveScreen.GetAllChildren());
            AllObjects.Add(ActiveScreen);

            if (FocusedObject != null && !VisibleFocusableObjects.Contains(FocusedObject))
            {
                TransferFocus(FocusedObject,
                    (IInterractive)VisibleFocusableChars.OrderBy(item => Math.Pow(item.Y, 2) + Math.Pow(item.X, 2)).FirstOrDefault()?.Owner, null);
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
                .Select(item => (IInterractive)item).ToList();

            AllObjects = new List<DisplayObject>(ActiveScreen.GetAllChildren());
            AllObjects.Add(ActiveScreen);

            if (FocusedObject != null && !VisibleFocusableObjects.Contains(FocusedObject))
            {
                TransferFocus(FocusedObject,
                    (IInterractive)VisibleFocusableChars.OrderBy(item => Math.Pow(item.Y, 2) + Math.Pow(item.X, 2)).FirstOrDefault()?.Owner, null);
            }
        }

        public IInterractive FocusedObject;

        public bool DebugEnabled = false;

        public void Start()
        {
            _currentConsole.Init();

            if (FocusedObject == null)
            {
                ForceDraw();

                TransferFocus(FocusedObject, (IInterractive)VisibleFocusableChars.OrderBy(item => Math.Pow(item.Y, 2) + Math.Pow(item.X, 2)).FirstOrDefault()?.Owner, null);

            }

            ForceDraw();

            _currentConsole.KeyPressed += CurrentConsole_KeyPressed;
            _currentConsole.StartCaptureKeyboard();
        }

        private void CurrentConsole_KeyPressed(ConsoleKeyInfo key)
        {
            if (DebugEnabled)
            {
                switch (key.Key)
                {
                    case ConsoleKey.F12:
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
            foreach (IAcceptGlobalInput dp in AllObjects.Where(item => item is IAcceptGlobalInput))
            {
                if (dp.FireGlobalKeypress(key))
                {
                    breakAndContinue = true;
                    break;
                }
            }

            if (breakAndContinue)
                return;

            // Then we give a chance to focused object to intercept the key
            if (FocusedObject != null && FocusedObject is IInterractive)
            {
                if (((IInterractive)FocusedObject).KeyPressed(key))
                {
                    return;
                }
            }


            // Finally we try to use it as a moving mechanism

            switch (key.Key)
            {
                case ConsoleKey.Tab:
                    CycleFocus(key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? -1 : 1, key);
                    break;
                case ConsoleKey.UpArrow:
                    MoveUp(key);
                    break;
                case ConsoleKey.DownArrow:
                    MoveDown(key);
                    break;
                case ConsoleKey.LeftArrow:
                    MoveLeft(key);
                    break;
                case ConsoleKey.RightArrow:
                    MoveRight(key);
                    break;
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

        private void MoveUp(ConsoleKeyInfo responsibleKey)
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
                TransferFocus(FocusedObject, (IInterractive)candidatesShortList.First().Key, responsibleKey);
            }
            else
            {
                int maxDistancesCount = candidatesShortList.Select(item => item.Value.Count).Max();

                candidatesShortList = candidatesShortList.Where(item => item.Value.Count == maxDistancesCount);


                if (candidatesShortList.Count() == 1)
                {
                    TransferFocus(FocusedObject, (IInterractive)candidatesShortList.First().Key, responsibleKey);
                }
                else// If multiple candidate are at a tie, select the lefter one
                {
                    TransferFocus(FocusedObject, (IInterractive)candidatesShortList.OrderBy(item => item.Key.DisplayX).First().Key, responsibleKey);
                }
            }

        }

        private void MoveDown(ConsoleKeyInfo responsibleKey)
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
                TransferFocus(FocusedObject, (IInterractive)candidatesShortList.First().Key, responsibleKey);
            }
            else
            {
                int maxDistancesCount = candidatesShortList.Select(item => item.Value.Count).Max();

                candidatesShortList = candidatesShortList.Where(item => item.Value.Count == maxDistancesCount);


                if (candidatesShortList.Count() == 1)
                {
                    TransferFocus(FocusedObject, (IInterractive)candidatesShortList.First().Key, responsibleKey);
                }
                else// If multiple candidate are at a tie, select the lefter one
                {
                    TransferFocus(FocusedObject, (IInterractive)candidatesShortList.OrderBy(item => item.Key.DisplayX).First().Key, responsibleKey);
                }
            }

        }

        private void MoveLeft(ConsoleKeyInfo responsibleKey)
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
                TransferFocus(FocusedObject, (IInterractive)candidatesShortList.First().Key, responsibleKey);
            }
            else
            {
                int maxDistancesCount = candidatesShortList.Select(item => item.Value.Count).Max();

                candidatesShortList = candidatesShortList.Where(item => item.Value.Count == maxDistancesCount);


                if (candidatesShortList.Count() == 1)
                {
                    TransferFocus(FocusedObject, (IInterractive)candidatesShortList.First().Key, responsibleKey);
                }
                else// If multiple candidate are at a tie, select the upper one
                {
                    TransferFocus(FocusedObject, (IInterractive)candidatesShortList.OrderBy(item => item.Key.DisplayY).First().Key, responsibleKey);
                }
            }

        }

        private void MoveRight(ConsoleKeyInfo responsibleKey)
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
                TransferFocus(FocusedObject, (IInterractive)candidatesShortList.First().Key, responsibleKey);
            }
            else
            {
                int maxDistancesCount = candidatesShortList.Select(item => item.Value.Count).Max();

                candidatesShortList = candidatesShortList.Where(item => item.Value.Count == maxDistancesCount);


                if (candidatesShortList.Count() == 1)
                {
                    TransferFocus(FocusedObject, (IInterractive)candidatesShortList.First().Key, responsibleKey);
                }
                else// If multiple candidate are at a tie, select the upper one
                {
                    TransferFocus(FocusedObject, (IInterractive)candidatesShortList.OrderBy(item => item.Key.DisplayY).First().Key, responsibleKey);
                }
            }

        }

        private void CycleFocus(int direction, ConsoleKeyInfo responsibleKey)
        {
            int focusedIndex = VisibleFocusableObjects.IndexOf(FocusedObject);

            int nextFocusedIndex = focusedIndex + direction;

            if (nextFocusedIndex >= VisibleFocusableObjects.Count)
                nextFocusedIndex = 0;
            else if (nextFocusedIndex < 0)
                nextFocusedIndex = VisibleFocusableObjects.Count - 1;


            TransferFocus(FocusedObject, VisibleFocusableObjects[nextFocusedIndex], responsibleKey);
        }

        private void TransferFocus(IInterractive oldDP, IInterractive newDP, ConsoleKeyInfo? responsibleKey)
        {
            if (oldDP != null)
            {
                oldDP.FocusedOut(responsibleKey);
                oldDP.Focused = false;
            }

            if (newDP != null)
            {
                newDP.FocusedIn(responsibleKey);
                newDP.Focused = true;
            }

            FocusedObject = newDP;
        }


    }
}
