using System;

namespace CLIForms.Widgets
{
    public class HorizontalLine : Widget
    {
        internal HorizontalLine() { }
        public HorizontalLine(Widget parent)
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
                char b = (Border == BorderStyle.Thick) ? '═' : '─';
                ConsoleHelper.DrawText(DisplayLeft, DisplayTop, Foreground, Background, new String(b, Width));
            }
        }
    }
}
