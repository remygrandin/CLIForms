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

        public List<IFocusable> VisibleFocussableObjects = new List<IFocusable>();
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

                VisibleFocussableObjects = engineBuffer.data.Flatten().Where(item => item.Focussable)
                    .Select(item => item.Owner).Distinct()
                    .Where(item => item.Parents(itm => itm.Parent).All(itm => item.Disabled == false))
                    .Select(item => (IFocusable) item).ToList();

                AllObjects = new List<DisplayObject>(ActiveScreen.GetAllChildren());
                AllObjects.Add(ActiveScreen);

                if (FocusedObject != null && !VisibleFocussableObjects.Contains(FocusedObject))
                {
                    TransferFocus(FocusedObject,
                        (IFocusable) VisibleFocussableObjects.Select(item => (DisplayObject) item)
                            .OrderBy(item => item.DisplayY).ThenBy(item => item.DisplayX).First(), null);
                }

                Console.SetWindowPosition(0, 0);
                Console.SetCursorPosition(0, 0);
            }
        }

        public IFocusable FocusedObject;

        public void Start()
        {
            Console.CursorVisible = false;

            if (FocusedObject == null)
            {
                ForceDraw();

                TransferFocus(FocusedObject, (IFocusable)VisibleFocussableObjects.Cast<DisplayObject>().OrderBy(item => item.DisplayY).ThenBy(item => item.DisplayX).FirstOrDefault(), null);

            }

            


            while (true)
            {
                ForceDraw();

                ConsoleKeyInfo k = Console.ReadKey(true);
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

                if(breakAndContinue)
                    continue;

                // Then we give a chance to focussed object to intercept the key
                if (FocusedObject != null && FocusedObject is IAcceptInput)
                {
                    if (((IAcceptInput)FocusedObject).KeyPressed(k))
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

        private void MoveUp(ConsoleKeyInfo responsibleKey)
        {
            IEnumerable<PositionedConsoleChar> positionnedChars = engineBuffer.dataPositioned.Where(item => item.Focussable);

            int minYActive = positionnedChars.Where(item => item.Owner == FocusedObject).Min(item => item.Y);

            IEnumerable<PositionedConsoleChar> candidatesFocused = positionnedChars.Where(item => item.Owner == FocusedObject && item.Y == minYActive);

            IEnumerable<PositionedConsoleChar> candidatesNotFocused = positionnedChars.Where(item => item.Owner != FocusedObject && item.Y < minYActive);

            if (!candidatesNotFocused.Any())
            {
                return;
            }
            // Target, Distance
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

            double minDistance = candidates.SelectMany(item => item.Value).Min();

            IEnumerable<KeyValuePair<DisplayObject, List<double>>> candidatesShortList = candidates.Where(item => item.Value.Contains(minDistance));

            if (candidatesShortList.Count() == 1)
            {
                TransferFocus(FocusedObject, (IFocusable)candidatesShortList.First().Key, responsibleKey);
            }
            else
            {
                int maxDistancesCount = candidatesShortList.Select(item => item.Value.Count).Max();

                candidatesShortList = candidatesShortList.Where(item => item.Value.Count == maxDistancesCount);


                if (candidatesShortList.Count() == 1)
                {
                    TransferFocus(FocusedObject, (IFocusable)candidatesShortList.First().Key, responsibleKey);
                }
                else// If multiple candidate are at a tie, select the lefter one
                {
                    TransferFocus(FocusedObject, (IFocusable)candidatesShortList.OrderBy(item => item.Key.DisplayX).First().Key, responsibleKey);
                }
            }

        }

        private void MoveDown(ConsoleKeyInfo responsibleKey)
        {
            IEnumerable<PositionedConsoleChar> positionnedChars = engineBuffer.dataPositioned.Where(item => item.Focussable);

            int maxYActive = positionnedChars.Where(item => item.Owner == FocusedObject).Max(item => item.Y);

            IEnumerable<PositionedConsoleChar> candidatesFocused = positionnedChars.Where(item => item.Owner == FocusedObject && item.Y == maxYActive);

            IEnumerable<PositionedConsoleChar> candidatesNotFocused = positionnedChars.Where(item => item.Owner != FocusedObject && item.Y > maxYActive);

            if (!candidatesNotFocused.Any())
            {
                return;
            }
            // Target, Distance
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

            double minDistance = candidates.SelectMany(item => item.Value).Min();

            IEnumerable<KeyValuePair<DisplayObject, List<double>>> candidatesShortList = candidates.Where(item => item.Value.Contains(minDistance));

            if (candidatesShortList.Count() == 1)
            {
                TransferFocus(FocusedObject, (IFocusable)candidatesShortList.First().Key, responsibleKey);
            }
            else
            {
                int maxDistancesCount = candidatesShortList.Select(item => item.Value.Count).Max();

                candidatesShortList = candidatesShortList.Where(item => item.Value.Count == maxDistancesCount);

                
                if (candidatesShortList.Count() == 1)
                {
                    TransferFocus(FocusedObject, (IFocusable)candidatesShortList.First().Key, responsibleKey);
                }
                else// If multiple candidate are at a tie, select the lefter one
                {
                    TransferFocus(FocusedObject, (IFocusable)candidatesShortList.OrderBy(item => item.Key.DisplayX).First().Key, responsibleKey);
                }
            }

        }

        private void MoveLeft(ConsoleKeyInfo responsibleKey)
        {
            IEnumerable<PositionedConsoleChar> positionnedChars = engineBuffer.dataPositioned.Where(item => item.Focussable);

            int minXActive = positionnedChars.Where(item => item.Owner == FocusedObject).Min(item => item.X);

            IEnumerable<PositionedConsoleChar> candidatesFocused = positionnedChars.Where(item => item.Owner == FocusedObject && item.X == minXActive);

            IEnumerable<PositionedConsoleChar> candidatesNotFocused = positionnedChars.Where(item => item.Owner != FocusedObject && item.X < minXActive);

            if (!candidatesNotFocused.Any())
            {
                return;
            }
            // Target, Distance
            Dictionary<DisplayObject, List<double>> candidates = new Dictionary<DisplayObject, List<double>>();

            foreach (PositionedConsoleChar focusedChar in candidatesFocused)
            {
                foreach (PositionedConsoleChar notFocusedChar in candidatesNotFocused)
                {
                    double distance = Math.Pow(focusedChar.X - notFocusedChar.X, 2) +
                                      Math.Pow(focusedChar.Y - notFocusedChar.Y, 2);

                    // Direct line bonus
                    if (focusedChar.Y == notFocusedChar.Y)
                        distance /= 2;

                    if (!candidates.ContainsKey(notFocusedChar.Owner))
                    {
                        candidates.Add(notFocusedChar.Owner, new List<double>());
                    }
                    candidates[notFocusedChar.Owner].Add(distance);

                }
            }

            double minDistance = candidates.SelectMany(item => item.Value).Min();

            IEnumerable<KeyValuePair<DisplayObject, List<double>>> candidatesShortList = candidates.Where(item => item.Value.Contains(minDistance));

            if (candidatesShortList.Count() == 1)
            {
                TransferFocus(FocusedObject, (IFocusable)candidatesShortList.First().Key, responsibleKey);
            }
            else
            {
                int maxDistancesCount = candidatesShortList.Select(item => item.Value.Count).Max();

                candidatesShortList = candidatesShortList.Where(item => item.Value.Count == maxDistancesCount);


                if (candidatesShortList.Count() == 1)
                {
                    TransferFocus(FocusedObject, (IFocusable)candidatesShortList.First().Key, responsibleKey);
                }
                else// If multiple candidate are at a tie, select the upper one
                {
                    TransferFocus(FocusedObject, (IFocusable)candidatesShortList.OrderBy(item => item.Key.DisplayY).First().Key, responsibleKey);
                }
            }

        }

        private void MoveRight(ConsoleKeyInfo responsibleKey)
        {
            IEnumerable<PositionedConsoleChar> positionnedChars = engineBuffer.dataPositioned.Where(item => item.Focussable);

            int maxXActive = positionnedChars.Where(item => item.Owner == FocusedObject).Max(item => item.X);

            IEnumerable<PositionedConsoleChar> candidatesFocused = positionnedChars.Where(item => item.Owner == FocusedObject && item.X == maxXActive);

            IEnumerable<PositionedConsoleChar> candidatesNotFocused = positionnedChars.Where(item => item.Owner != FocusedObject && item.X > maxXActive);

            if (!candidatesNotFocused.Any())
            {
                return;
            }
            // Target, Distance
            Dictionary<DisplayObject, List<double>> candidates = new Dictionary<DisplayObject, List<double>>();

            foreach (PositionedConsoleChar focusedChar in candidatesFocused)
            {
                foreach (PositionedConsoleChar notFocusedChar in candidatesNotFocused)
                {
                    double distance = Math.Pow(focusedChar.X - notFocusedChar.X, 2) +
                                      Math.Pow(focusedChar.Y - notFocusedChar.Y, 2);

                    // Direct line bonus
                    if (focusedChar.Y == notFocusedChar.Y)
                        distance /= 2;

                    if (!candidates.ContainsKey(notFocusedChar.Owner))
                    {
                        candidates.Add(notFocusedChar.Owner, new List<double>());
                    }
                    candidates[notFocusedChar.Owner].Add(distance);

                }
            }

            double minDistance = candidates.SelectMany(item => item.Value).Min();

            IEnumerable<KeyValuePair<DisplayObject, List<double>>> candidatesShortList = candidates.Where(item => item.Value.Contains(minDistance));

            if (candidatesShortList.Count() == 1)
            {
                TransferFocus(FocusedObject, (IFocusable)candidatesShortList.First().Key, responsibleKey);
            }
            else
            {
                int maxDistancesCount = candidatesShortList.Select(item => item.Value.Count).Max();

                candidatesShortList = candidatesShortList.Where(item => item.Value.Count == maxDistancesCount);


                if (candidatesShortList.Count() == 1)
                {
                    TransferFocus(FocusedObject, (IFocusable)candidatesShortList.First().Key, responsibleKey);
                }
                else// If multiple candidate are at a tie, select the upper one
                {
                    TransferFocus(FocusedObject, (IFocusable)candidatesShortList.OrderBy(item => item.Key.DisplayY).First().Key, responsibleKey);
                }
            }

        }

        private void CycleFocus(int direction, ConsoleKeyInfo responsibleKey)
        {
            int focusedIndex = VisibleFocussableObjects.IndexOf(FocusedObject);

            int nextFocusedIndex = focusedIndex + direction;

            if (nextFocusedIndex >= VisibleFocussableObjects.Count)
                nextFocusedIndex = 0;
            else if(nextFocusedIndex < 0)
                nextFocusedIndex = VisibleFocussableObjects.Count - 1;


            TransferFocus(FocusedObject, VisibleFocussableObjects[nextFocusedIndex], responsibleKey);
        }

        private void TransferFocus(IFocusable oldDP, IFocusable newDP, ConsoleKeyInfo? responsibleKey)
        {
            if (oldDP != null)
            {
                oldDP.FireFocusOut(responsibleKey);
                oldDP.Focused = false;
            }

            if (newDP != null)
            {
                newDP.FireFocusIn(responsibleKey);
                newDP.Focused = true;
            }

            FocusedObject = newDP;
        }


    }
}
