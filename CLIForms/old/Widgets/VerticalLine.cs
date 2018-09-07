using System;
using CLIForms.Styles;

namespace CLIForms.Widgets
{
    public class VerticalLine : Widget
    {
        internal VerticalLine() { }
        public VerticalLine(Widget parent)
            : base(parent)
        {
            Background = parent.Background;
            Foreground = ConsoleColor.Black;
            Border = BorderStyle.Thin;
        }

        internal override void Render()
        {
            if (Border != BorderStyle.None)
            {
                string b = (Border == BorderStyle.Thick) ? "║" : "│";

                for (var i = 0; i < Height; i++)
                {
                    ConsoleHelper.DrawText(DisplayLeft, DisplayTop + i, Foreground, Background, b);
                }
            }
        }
    }
}
