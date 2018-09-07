using System;
using CLIForms.Components;
using CLIForms.Styles;

namespace CLIForms
{
    public static class DrawingHelper
    {
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

        private static int shadowStyleToPos(ShadowStyle style)
        {
            switch (style)
            {
                case ShadowStyle.None:
                    return 0;
                case ShadowStyle.Light:
                    return 1;
                case ShadowStyle.Medium:
                    return 2;
                case ShadowStyle.Dark:
                    return 3;
                case ShadowStyle.Block:
                    return 4;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }

        internal static string GetShadow(ShadowStyle style) => " ░▒▓█"[shadowStyleToPos(style)].ToString();

        internal static string GetTopLeftCornerBorder(BorderStyle style) => "▄╔┌ "[borderStyleToPos(style)].ToString();
        internal static string GetTopRightCornerBorder(BorderStyle style) => "▄╗┐ "[borderStyleToPos(style)].ToString();
        internal static string GetBottomLeftCornerBorder(BorderStyle style) => "▀╚└ "[borderStyleToPos(style)].ToString();
        internal static string GetBottomRightCornerBorder(BorderStyle style) => "▀╝┘ "[borderStyleToPos(style)].ToString();


        internal static string GetHorizontalBorder(BorderStyle style) => "█═─ "[borderStyleToPos(style)].ToString();
        internal static string GetVerticalBorder(BorderStyle style) => "█║│ "[borderStyleToPos(style)].ToString();

        internal static string GetTopTJunctionBorder(BorderStyle style) => "█╩┴ "[borderStyleToPos(style)].ToString();
        internal static string GetBottomTJunctionBorder(BorderStyle style) => "█╦┬ "[borderStyleToPos(style)].ToString();
        internal static string GetLeftTJunctionBorder(BorderStyle style) => "█╣┤ "[borderStyleToPos(style)].ToString();
        internal static string GetRightTJunctionBorder(BorderStyle style) => "█╠├ "[borderStyleToPos(style)].ToString();
        internal static string GetCrossJunctionBorder(BorderStyle style) => "█╬┼ "[borderStyleToPos(style)].ToString();

        internal static ConsoleCharBuffer DrawBlockOutline(ConsoleCharBuffer buffer,DisplayObject owner, int x, int y, int w, int h, ConsoleColor backgroundColor, ConsoleColor foregroundColor, BorderStyle style, ShadowStyle shadow)
        {
            // Border
            buffer.DrawString(owner, GetTopLeftCornerBorder(style) +
                              new string(GetHorizontalBorder(style)[0], w - 2) +
                              GetTopRightCornerBorder(style), x, y, backgroundColor, foregroundColor);

            for (int i = y + 1; i < y + h - 1; i++)
            {
                buffer.DrawString(owner, GetVerticalBorder(style), x, i, backgroundColor, foregroundColor);

                buffer.DrawString(owner, GetVerticalBorder(style), x + w, i, backgroundColor, foregroundColor);
            }

            buffer.DrawString(owner, GetBottomLeftCornerBorder(style) +
                              new string(GetHorizontalBorder(style)[0], w - 2) +
                              GetBottomRightCornerBorder(style), x, y + h - 1, backgroundColor, foregroundColor);

            if (shadow != ShadowStyle.None)
            {
                for (int i = y + 1; i < y + h - 1; i++)
                {
                    buffer.DrawString(owner, GetShadow(shadow), x + w + 1, i, null, foregroundColor);
                }

                buffer.DrawString(owner, new string(GetShadow(shadow)[0], w), x + 1, y + h, null, foregroundColor);

            }

            return buffer;
        }

        internal static ConsoleCharBuffer DrawBlockFull(ConsoleCharBuffer buffer, DisplayObject owner, int x, int y, int w, int h, ConsoleColor? backgroundColor, ConsoleColor foregroundColor, BorderStyle style, ShadowStyle shadow)
        {
            // Border
            buffer.DrawString(owner, GetTopLeftCornerBorder(style) +
                              new string(GetHorizontalBorder(style)[0], w - 2) +
                              GetTopRightCornerBorder(style), x, y, backgroundColor, foregroundColor);

            for (int i = y + 1; i < y + h - 1; i++)
            {
                buffer.DrawString(owner, GetVerticalBorder(style) + new string(' ', w - 2) + GetVerticalBorder(style), x, i, backgroundColor, foregroundColor);
            }

            buffer.DrawString(owner, GetBottomLeftCornerBorder(style) +
                              new string(GetHorizontalBorder(style)[0], w - 2) +
                              GetBottomRightCornerBorder(style), x, y + h - 1, backgroundColor, foregroundColor);

            if (shadow != ShadowStyle.None)
            {
                for (int i = y + 1; i < y + h ; i++)
                {
                    buffer.DrawString(owner, GetShadow(shadow), x + w, i, null, foregroundColor);
                }

                buffer.DrawString(owner, new string(GetShadow(shadow)[0], w), x + 1, y + h, null, foregroundColor);

            }

            return buffer;
        }
    }
}
