using System;

namespace CLIForms
{
    internal static class ConsoleHelper
    {
        internal static void ResetConsoleWindow()
        {
            Console.SetWindowPosition(0, 0);
            Console.SetCursorPosition(0, 0);
        }

        internal static void DrawText(int x, int y, ConsoleColor fg, ConsoleColor bg, string format, params object[] args)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
            Console.Write(format, args);
        }

        internal static void DrawText(int x, int y, ConsoleColor fg, string format, params object[] args)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = fg;
            Console.Write(format, args);
        }

        internal static void DrawRectShade(int x, int y, int w, int h, ConsoleColor bg, ConsoleColor fg, char ch)
        {
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;

            var l = new String(ch, w);

            for (var i = 0; i < h - 1; i++)
            {
                Console.SetCursorPosition(x + w - 1, y + i);
                Console.Write(ch);
            }
            Console.SetCursorPosition(x, y + h - 1);
            Console.Write(l);

            ResetConsoleWindow();
        }

        internal delegate void DrawBoxMethod(int x, int y, int w, int h, ConsoleColor c);

        internal static void DrawNothing(int x, int y, int w, int h, ConsoleColor c){ }

        internal static void DrawRectSolid(int x, int y, int w, int h, ConsoleColor c)
        {
            Console.BackgroundColor = c;
            var l = new String(' ', w);
            for (var i = 0; i < h; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(l);
            }
            ResetConsoleWindow();
        }

        internal static void DrawBlockOutline(int x, int y, int w, int h, ConsoleColor c)
        {
            Console.ForegroundColor = c;

            Console.SetCursorPosition(x, y);
            Console.Write("▄");
            Console.Write(new String('█', w - 1));
            Console.Write("▄");

            for (int i = 1; i < h - 1; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write("█");
                Console.SetCursorPosition(x + w, y + i);
                Console.Write("█");
            }

            Console.SetCursorPosition(x, y + h - 1);
            Console.Write("▀");
            Console.Write(new String('█', w - 1));
            Console.Write("▀");
        }

        internal static void DrawSingleOutline(int x, int y, int w, int h, ConsoleColor c)
        {
            Console.ForegroundColor = c;

            Console.SetCursorPosition(x, y);
            Console.Write("┌");
            Console.Write(new String('─', w - 1));
            Console.Write("┐");

            for (int i = 1; i < h - 1; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write("│");
                Console.SetCursorPosition(x + w, y + i);
                Console.Write("│");
            }

            Console.SetCursorPosition(x, y + h - 1);
            Console.Write("└");
            Console.Write(new String('─', w - 1));
            Console.Write("┘");
        }

        internal static void DrawDoubleOutline(int x, int y, int w, int h, ConsoleColor c)
        {
            Console.ForegroundColor = c;

            Console.SetCursorPosition(x, y);
            Console.Write("╔");
            Console.Write(new String('═', w - 1));
            Console.Write("╗");

            for (int i = 1; i < h - 1; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write("║");
                Console.SetCursorPosition(x + w, y + i);
                Console.Write("║");
            }

            Console.SetCursorPosition(x, y + h - 1);
            Console.Write("╚");
            Console.Write(new String('═', w - 1));
            Console.Write("╝");
        }

        private static int borderStyleToPos(BorderStyle style)
        {
            switch (style)
            {
                case BorderStyle.Block:
                    return 0;
                case BorderStyle.Thick:
                    return 1;
                case BorderStyle.Thin:
                    return 2;
                case BorderStyle.None:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }

        internal static string GetTopLeftCornerBorder(BorderStyle style) => "▄╔┌ "[borderStyleToPos(style)].ToString();
        internal static string GetTopRightCornerBorder(BorderStyle style) => "▄╗┐ "[borderStyleToPos(style)].ToString();
        internal static string GetBottomLeftCornerBorder(BorderStyle style) => "▀╚└ "[borderStyleToPos(style)].ToString();
        internal static string GetBottomRightCornerBorder(BorderStyle style) => "▀╝┘ "[borderStyleToPos(style)].ToString();


        internal static string GetHorizontalBorder(BorderStyle style) => "█═─ "[borderStyleToPos(style)].ToString();
        internal static string GetVerticalBorder(BorderStyle style) => "█║│ "[borderStyleToPos(style)].ToString();

        internal static string GetTopTJunctionBorder(BorderStyle style) => "█╩┴ "[borderStyleToPos(style)].ToString();
        internal static string GetBotomTJunctionBorder(BorderStyle style) => "█╦┬ "[borderStyleToPos(style)].ToString();
        internal static string GetLeftTJunctionBorder(BorderStyle style) => "█╣┤ "[borderStyleToPos(style)].ToString();
        internal static string GetRightTJunctionBorder(BorderStyle style) => "█╠├ "[borderStyleToPos(style)].ToString();
        internal static string GetCrossJunctionBorder(BorderStyle style) => "█╬┼ "[borderStyleToPos(style)].ToString();
    }
}
