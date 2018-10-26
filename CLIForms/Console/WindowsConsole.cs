using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLIForms.Buffer;
using CLIForms.Extentions;

namespace CLIForms.Console
{
    public class WindowsConsole : IConsole
    {

        public int Width
        {
            get { return System.Console.WindowWidth; }
            set
            {
                System.Console.WindowWidth = value;

                ReApplySizing();
            }
        }

        public int Height
        {
            get { return System.Console.WindowHeight; }
            set
            {
                System.Console.WindowHeight = value;

                ReApplySizing();
            }
        }

        private void ReApplySizing()
        {
            System.Console.SetBufferSize(Width, Height + 1);
        }

        private bool _draw = false;

        public bool Draw
        {
            get => _draw;
            set => _draw = value;
        }

        /*
        public void Display(ConsoleCharBuffer buffer)
        {
            if (_draw)
            {
                for (int y = 0; y < buffer.Height; y++)
                    for (int x = 0; x < buffer.Width; x++)
                    {
                        System.Console.SetCursorPosition(x, y);
                        System.Console.BackgroundColor = buffer.data[x, y].Background ?? ConsoleColor.Black;
                        System.Console.ForegroundColor = buffer.data[x, y].Foreground;
                        System.Console.Write(buffer.data[x, y].Char);
                    }

                System.Console.SetWindowPosition(0, 0);
                System.Console.SetCursorPosition(0, 0);
            }
        }
        */
        public void Display(ConsoleCharBuffer buffer)
        {
            Display(buffer.dataPositioned);
        }

        private object _drawLock = new object();

        public void Display(IEnumerable<PositionedConsoleChar> chars)
        {
            if (_draw)
            {
                lock (_drawLock)
                {

                    System.Console.SetCursorPosition(0, 0);
                    System.Console.SetWindowPosition(0, 0);

                    var result = chars.GroupBy(item =>
                            item.Y
                        );

                    foreach (var lineChars in result)
                    {
                        var groupped = lineChars.GroupAdjacentBy((x1, x2) => x1.X + 1 == x2.X && x1.Background == x2.Background && x1.Foreground == x2.Foreground);

                        foreach (var subGroup in groupped)
                        {
                            var orderedSubGroup = subGroup.OrderBy(item => item.X);

                            var firstChar = orderedSubGroup.First();
                            string groupStr = new string(orderedSubGroup.Select(item => item.Char).ToArray());

                            System.Console.SetCursorPosition(firstChar.X, firstChar.Y);
                            System.Console.BackgroundColor = firstChar.Background ?? ConsoleColor.Black;
                            System.Console.ForegroundColor = firstChar.Foreground;
                            System.Console.Write(groupStr);
                        }

                    }


                    System.Console.SetCursorPosition(0, 0);
                    System.Console.SetWindowPosition(0, 0);

                    System.Console.BackgroundColor = _initBackgroundColor;
                    System.Console.ForegroundColor = _initForegroundColor;

                    //ReApplySizing();
                }

                
            }
        }

        public event KeyEventHandler KeyPressed;

        private ConsoleColor _initBackgroundColor;
        private ConsoleColor _initForegroundColor;

        private int _initXPos;
        private int _initYPos;

        private Encoding _initOutputEncoding;
        private bool _initCursorVisible;

        public void Init()
        {

            _initXPos = System.Console.CursorLeft;
            _initYPos = System.Console.CursorTop;

            _initBackgroundColor = System.Console.BackgroundColor;
            _initForegroundColor = System.Console.ForegroundColor;

            _initOutputEncoding = System.Console.OutputEncoding;
            _initCursorVisible = System.Console.CursorVisible;

            System.Console.OutputEncoding = System.Text.Encoding.Unicode;
            System.Console.CursorVisible = false;

            System.Console.SetWindowPosition(0, 0);
            System.Console.SetCursorPosition(0, 0);

            _draw = true;
        }


        public void End()
        {
            _draw = false;

            System.Console.CursorLeft = _initXPos;
            System.Console.CursorTop = _initYPos;

            System.Console.BackgroundColor = _initBackgroundColor;
            System.Console.ForegroundColor = _initForegroundColor;

            System.Console.OutputEncoding = _initOutputEncoding;
            System.Console.CursorVisible = _initCursorVisible;
        }

        private Task _captureKeyboardTask;


        public void StartCaptureKeyboard()
        {
            while (true)
            {
                ConsoleKeyInfo key = System.Console.ReadKey(true);

                KeyPressed?.Invoke(key);
            }
        }
    }
}
