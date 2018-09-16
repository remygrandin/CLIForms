using System;
using CLIForms.Components;
using CLIForms.Styles;

namespace CLIForms.Buffer
{
    public static class DrawingHelper
    {
        private static int BorderStyleToPos(BorderStyle style)
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

        private static int ShadowStyleToPos(ShadowStyle style)
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

        internal static string GetShadow(ShadowStyle style) => " ░▒▓█"[ShadowStyleToPos(style)].ToString();

        internal static string GetTopLeftCornerBorder(BorderStyle style) => "▄╔┌ "[BorderStyleToPos(style)].ToString();
        internal static string GetTopRightCornerBorder(BorderStyle style) => "▄╗┐ "[BorderStyleToPos(style)].ToString();
        internal static string GetBottomLeftCornerBorder(BorderStyle style) => "▀╚└ "[BorderStyleToPos(style)].ToString();
        internal static string GetBottomRightCornerBorder(BorderStyle style) => "▀╝┘ "[BorderStyleToPos(style)].ToString();


        internal static string GetHorizontalBorder(BorderStyle style) => "█═─ "[BorderStyleToPos(style)].ToString();
        internal static string GetVerticalBorder(BorderStyle style) => "█║│ "[BorderStyleToPos(style)].ToString();

        internal static string GetTopTJunctionBorder(BorderStyle style) => "█╩┴ "[BorderStyleToPos(style)].ToString();
        internal static string GetBottomTJunctionBorder(BorderStyle style) => "█╦┬ "[BorderStyleToPos(style)].ToString();
        internal static string GetLeftTJunctionBorder(BorderStyle style) => "█╣┤ "[BorderStyleToPos(style)].ToString();
        internal static string GetRightTJunctionBorder(BorderStyle style) => "█╠├ "[BorderStyleToPos(style)].ToString();
        internal static string GetCrossJunctionBorder(BorderStyle style) => "█╬┼ "[BorderStyleToPos(style)].ToString();

        internal static ConsoleCharBuffer DrawBlockOutline(ConsoleCharBuffer buffer, DisplayObject owner, bool focussable, int x, int y, int w, int h, ConsoleColor backgroundColor, ConsoleColor foregroundColor, BorderStyle style, ShadowStyle shadow)
        {
            // Border
            buffer.DrawString(owner, GetTopLeftCornerBorder(style) +
                              new string(GetHorizontalBorder(style)[0], w - 2) +
                              GetTopRightCornerBorder(style), focussable, x, y, backgroundColor, foregroundColor);

            for (int i = y + 1; i < y + h - 1; i++)
            {
                buffer.DrawString(owner, GetVerticalBorder(style), focussable, x, i, backgroundColor, foregroundColor);

                buffer.DrawString(owner, GetVerticalBorder(style), focussable, x + w, i, backgroundColor, foregroundColor);
            }

            buffer.DrawString(owner, GetBottomLeftCornerBorder(style) +
                              new string(GetHorizontalBorder(style)[0], w - 2) +
                              GetBottomRightCornerBorder(style), focussable, x, y + h - 1, backgroundColor, foregroundColor);

            if (shadow != ShadowStyle.None)
            {
                for (int i = y + 1; i < y + h - 1; i++)
                {
                    buffer.DrawString(owner, GetShadow(shadow), focussable, x + w + 1, i, null, foregroundColor);
                }

                buffer.DrawString(owner, new string(GetShadow(shadow)[0], w), focussable, x + 1, y + h, null, foregroundColor);

            }

            return buffer;
        }

        internal static ConsoleCharBuffer DrawBlockFull(ConsoleCharBuffer buffer, DisplayObject owner, bool focusable, int x, int y, int w, int h, ConsoleColor? backgroundColor, ConsoleColor foregroundColor, BorderStyle style, ShadowStyle shadow)
        {
            // Border
            buffer.DrawString(owner, GetTopLeftCornerBorder(style) +
                              new string(GetHorizontalBorder(style)[0], w - 2) +
                              GetTopRightCornerBorder(style), focusable, x, y, backgroundColor, foregroundColor);

            for (int i = y + 1; i < y + h - 1; i++)
            {
                buffer.DrawString(owner, GetVerticalBorder(style) + new string(' ', w - 2) + GetVerticalBorder(style), focusable, x, i, backgroundColor, foregroundColor);
            }

            buffer.DrawString(owner, GetBottomLeftCornerBorder(style) +
                              new string(GetHorizontalBorder(style)[0], w - 2) +
                              GetBottomRightCornerBorder(style), focusable, x, y + h - 1, backgroundColor, foregroundColor);

            if (shadow != ShadowStyle.None)
            {
                for (int i = y + 1; i < y + h ; i++)
                {
                    buffer.DrawString(owner, GetShadow(shadow), focusable, x + w, i, null, foregroundColor);
                }

                buffer.DrawString(owner, new string(GetShadow(shadow)[0], w), focusable, x + 1, y + h, null, foregroundColor);

            }

            return buffer;
        }
    }
}
