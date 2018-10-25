using System;
using System.Collections.Generic;
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

                System.Console.SetBufferSize(Width, Height);
            }
        }

        public int Height
        {
            get { return System.Console.WindowHeight; }
            set
            {
                System.Console.WindowHeight = value;

                System.Console.SetBufferSize(Width, Height);
            }
        }

        public void Display(ConsoleCharBuffer buffer)
        {
            for (int x = 0; x < buffer.Width; x++)
            for (int y = 0; y < buffer.Height; y++)
            {
                System.Console.SetCursorPosition(x, y);
                System.Console.BackgroundColor = buffer.data[x, y].Background ?? ConsoleColor.Black;
                System.Console.ForegroundColor = buffer.data[x, y].Foreground;
                System.Console.Write(buffer.data[x, y].Char);
            }

            System.Console.SetWindowPosition(0, 0);
            System.Console.SetCursorPosition(0, 0);
        }

        public void Display(List<PositionedConsoleChar> chars)
        {

            var result = chars.GroupBy(item =>
                new
                {
                    item.Y,
                    item.Background,
                    item.Foreground
                });

            foreach (var lineChars in result)
            {
                var groupped = lineChars.GroupAdjacentBy((x1, x2) => x1.X + 1 == x2.X);

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


            System.Console.SetWindowPosition(0, 0);
            System.Console.SetCursorPosition(0, 0);
        }

        private ConsoleColor _initBackgroundColor;
        private ConsoleColor _initForegroundColor;

        private int _initXPos;
        private int _initYPos;

        public void Init()
        {
            _initXPos = System.Console.CursorLeft;
            _initYPos = System.Console.CursorTop;

            _initBackgroundColor = System.Console.BackgroundColor;
            _initForegroundColor = System.Console.ForegroundColor;

            System.Console.OutputEncoding = System.Text.Encoding.Unicode;
            System.Console.CursorVisible = false;

            System.Console.SetWindowPosition(0, 0);
            System.Console.SetCursorPosition(0, 0);
        }


        public void End()
        {
            throw new NotImplementedException();
        }
    }
}
