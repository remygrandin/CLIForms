using System;
using CLIForms.Components;

namespace CLIForms.Buffer
{
    public class ConsoleChar
    {
        public ConsoleColor? Background;
        public ConsoleColor Foreground;
        public char Char;
        public DisplayObject Owner;
        public bool Focussable;

        public ConsoleChar(ConsoleChar consoleChar)
        {
            Char = consoleChar.Char;
            Background = consoleChar.Background;
            Foreground = consoleChar.Foreground;
            Owner = consoleChar.Owner;
            Focussable = consoleChar.Focussable;
        }

        public ConsoleChar(DisplayObject owner, char ch, bool focussable, ConsoleColor? background = null, ConsoleColor foreground = ConsoleColor.White)
        {
            Owner = owner;
            Char = ch;
            Focussable = focussable;
            Background = background;
            Foreground = foreground;
        }

        public override string ToString()
        {
            return "C=" + Char.ToString() + ", F=" + Focussable.ToString() + ", O=" + Owner.ToString();
        }

        public override bool Equals(Object obj)
        {
            return obj is ConsoleChar && this == (ConsoleChar)obj;
        }
        public override int GetHashCode()
        {
            return Background.GetHashCode() ^ Foreground.GetHashCode() ^ Char.GetHashCode();
        }
        public static bool operator ==(ConsoleChar x, ConsoleChar y)
        {
            if (object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null))
                return true;

            if (object.ReferenceEquals(x, null) && !object.ReferenceEquals(y, null))
                return false;

            if (!object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null))
                return false;

            return x.Background == y.Background && x.Foreground == y.Foreground && x.Char == y.Char;
        }
        public static bool operator !=(ConsoleChar x, ConsoleChar y)
        {
            return !(x == y);
        }

        public ConsoleChar Merge(ConsoleChar masterChar)
        {
            if (masterChar == null)
                return this;

            if (masterChar.Background != null)
                return masterChar;

            ConsoleChar newChar = new ConsoleChar(masterChar)
            { Background = this.Background };


            return newChar;
        }
    }
}
