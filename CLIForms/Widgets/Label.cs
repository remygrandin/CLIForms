using System;

namespace CLIForms.Widgets
{
    public class Label : Widget
    {
        internal Label() {}
        public Label(Widget parent) : base(parent) 
        {
            Background = parent.Background;
            Foreground = ConsoleColor.White;
        }

        internal override void Render()
        {
            var lines = Text.Split(new string[]{"\n"}, StringSplitOptions.None);

            for (var i = 0; i < lines.Length; i++)
            {
                ConsoleHelper.DrawText(DisplayLeft, DisplayTop + i, Foreground, Background, lines[i]);
            }
        }
    }
}
