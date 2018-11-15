using System;

namespace CLIForms_FIGFonts
{
    public class FIGSubChar : ICloneable
    {
        public char Char = ' ';
        public int X = -1;
        public int Y = -1;

        public FIGSubChar(char ch)
        {
            Char = ch;
        }

        public FIGSubChar(char ch, int x, int y)
        {
            Char = ch;
            X = x;
            Y = y;
        }


        public override string ToString()
        {
            return "C=" + Char.ToString() + " | X=" + X.ToString() + " | Y=" + Y.ToString();
        }

        public override bool Equals(Object obj)
        {
            return obj is FIGSubChar && this == (FIGSubChar)obj;
        }
        public override int GetHashCode()
        {
            return Char.GetHashCode();
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(FIGSubChar x, FIGSubChar y)
        {
            if (object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null))
                return true;

            if (object.ReferenceEquals(x, null) && !object.ReferenceEquals(y, null))
                return false;

            if (!object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null))
                return false;

            return x.Char == y.Char;
        }

        public static bool operator !=(FIGSubChar x, FIGSubChar y)
        {
            return !(x == y);
        }
    }
}
