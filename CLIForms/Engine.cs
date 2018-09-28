using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using CLIForms.Buffer;
using CLIForms.Components;
using CLIForms.Components.Globals;
using CLIForms.Extentions;
using CLIForms.Interfaces;

namespace CLIForms
{
    public sealed class Engine
    {
        private static readonly Engine instance = new Engine();

        public static Engine Instance
        {
            get
            {
                return instance;
            }
        }



        private ConsoleCharBuffer engineBuffer;
        public Engine(int width = 80, int height = 30)
        {

            Console.WindowWidth = width;
            Console.WindowHeight = height;

            engineBuffer = new ConsoleCharBuffer(width, height);

            DebounceDirty.AutoReset = false;
            DebounceDirty.Elapsed += DebounceDirty_Elapsed;
        }


        public int Width
        {
            get => Console.WindowWidth;
            set
            {
                Console.WindowWidth = value;
                Console.SetBufferSize(Width, Height);

                engineBuffer = new ConsoleCharBuffer(Width, Height);
                ActiveScreen.Width = Width;
            }
        }

        public int Height
        {
            get => Console.WindowHeight;
            set
            {
                Console.WindowHeight = value;
                Console.SetBufferSize(Width, Height);
                engineBuffer = new ConsoleCharBuffer(Width, Height);

                engineBuffer = new ConsoleCharBuffer(Width, Height);
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

        public bool Draw = true;

        private Timer DebounceDirty = new Timer(10);

        public void SignalDirty(Screen screen)
        {
            if (ActiveScreen == screen)
            {
                DebounceDirty.Stop();
                DebounceDirty.Start();
            }
        }


        private void DebounceDirty_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Draw)
            {
                ForceDraw();
            }
        }

        private object _drawLock = new object();

        public void ForceDraw()
        {
            lock (_drawLock)
            {
                ConsoleCharBuffer screenbuffer = ActiveScreen.Render();

                List<PositionedConsoleChar> diff = engineBuffer.Diff(screenbuffer);

                ConsoleCharBuffer.Display(diff);

                engineBuffer = screenbuffer;

                VisibleFocusableChars = engineBuffer.dataPositioned.Where(item => item.Focussable).ToList();
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

                Console.SetWindowPosition(0, 0);
                Console.SetCursorPosition(0, 0);
            }
        }

        public IInterractive FocusedObject;

        public bool DebugEnabled = false;

        public void Start()
        {

            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.CursorVisible = false;


            if (FocusedObject == null)
            {
                ForceDraw();

                TransferFocus(FocusedObject, (IInterractive)VisibleFocusableChars.OrderBy(item => Math.Pow(item.Y, 2) + Math.Pow(item.X, 2)).FirstOrDefault()?.Owner, null);

            }




            while (true)
            {


                ConsoleKeyInfo k = Console.ReadKey(true);


                if (DebugEnabled)
                {
                    switch (k.Key)
                    {
                        case ConsoleKey.F12:
                            {
                                var oldBuffer = engineBuffer;
                                engineBuffer = new ConsoleCharBuffer(oldBuffer.Width, oldBuffer.Height);

                                for (int x = 0; x < engineBuffer.Width; x++)
                                    for (int y = 0; y < engineBuffer.Height; y++)
                                    {
                                        if (!oldBuffer.data[x, y].Focussable)
                                            engineBuffer.data[x, y] = new ConsoleChar(oldBuffer.data[x, y].Owner, oldBuffer.data[x, y].Char, oldBuffer.data[x, y].Focussable, ConsoleColor.Blue, ConsoleColor.Black);
                                        else
                                            engineBuffer.data[x, y] = new ConsoleChar(oldBuffer.data[x, y].Owner, oldBuffer.data[x, y].Char, oldBuffer.data[x, y].Focussable, ConsoleColor.DarkMagenta, ConsoleColor.Black);
                                    }

                                ConsoleCharBuffer.Display(engineBuffer);

                                continue;
                            }
                    }
                }

                ForceDraw();
                bool breakAndContinue = false;

                // First we give a chance to the global listeners to intercept the key (true to break)
                foreach (IAcceptGlobalInput dp in AllObjects.Where(item => item is IAcceptGlobalInput))
                {
                    if (dp.FireGlobalKeypress(k))
                    {
                        breakAndContinue = true;
                        break;
                    }
                }

                if (breakAndContinue)
                    continue;

                // Then we give a chance to focussed object to intercept the key
                if (FocusedObject != null && FocusedObject is IInterractive)
                {
                    if (((IInterractive)FocusedObject).KeyPressed(k))
                    {
                        continue;
                    }
                }


                // Finally we try to use it as a moving mechanism

                switch (k.Key)
                {
                    case ConsoleKey.Tab:
                        CycleFocus(k.Modifiers.HasFlag(ConsoleModifiers.Shift) ? -1 : 1, k);
                        break;
                    case ConsoleKey.UpArrow:
                        MoveUp(k);
                        break;
                    case ConsoleKey.DownArrow:
                        MoveDown(k);
                        break;
                    case ConsoleKey.LeftArrow:
                        MoveLeft(k);
                        break;
                    case ConsoleKey.RightArrow:
                        MoveRight(k);
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

        private void MoveUp(ConsoleKeyInfo responsibleKey)
        {
            IEnumerable<PositionedConsoleChar> positionedChars = engineBuffer.dataPositioned.Where(item => item.Focussable);

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

            if(candidates.Count == 0)
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
            IEnumerable<PositionedConsoleChar> positionedChars = engineBuffer.dataPositioned.Where(item => item.Focussable);

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
            IEnumerable<PositionedConsoleChar> positionedChars = engineBuffer.dataPositioned.Where(item => item.Focussable);

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
            IEnumerable<PositionedConsoleChar> positionedChars = engineBuffer.dataPositioned.Where(item => item.Focussable);

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
