using System;

namespace CLIForms.Widgets
{
    public class StatusBar : Widget
    {
        internal StatusBar() {}
        public StatusBar(Widget parent) : base(parent) 
        {
            Background = ConsoleColor.Gray;
            Foreground = ConsoleColor.Black;
        }

        private string _textLeft = "";
        public string TextLeft
        {
            get { return _textLeft; }
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
