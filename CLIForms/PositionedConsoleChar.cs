namespace CLIForms
{
    public class PositionedConsoleChar : ConsoleChar
    {
        public int X;
        public int Y;

        public PositionedConsoleChar(ConsoleChar consoleChar, int x, int y) : base(consoleChar.Owner,consoleChar.Char,consoleChar.Background,consoleChar.Foreground)
        {
            this.Background = consoleChar.Background;
            this.Foreground = consoleChar.Foreground;
            this.Char = consoleChar.Char;
            this.X = x;
            this.Y = y;
        }
    }
}
