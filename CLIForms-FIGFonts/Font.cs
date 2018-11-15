using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLIForms_FIGFonts
{
    public class Font
    {
        internal static Tuple<int, string>[] BaseCharCodes = new Tuple<int, string>[]
        {
            new Tuple<int, string>(32, "SPACE"), new Tuple<int, string>(33, "!"), new Tuple<int, string>(34, "\""),
            new Tuple<int, string>(35, "#"), new Tuple<int, string>(36, "$"), new Tuple<int, string>(37, "%"),
            new Tuple<int, string>(38, "&"), new Tuple<int, string>(39, "'"), new Tuple<int, string>(40, "("),
            new Tuple<int, string>(41, ")"), new Tuple<int, string>(42, "*"), new Tuple<int, string>(43, "+"),
            new Tuple<int, string>(44, ","), new Tuple<int, string>(45, "-"), new Tuple<int, string>(46, "."),
            new Tuple<int, string>(47, "/"), new Tuple<int, string>(48, "0"), new Tuple<int, string>(49, "1"),
            new Tuple<int, string>(50, "2"), new Tuple<int, string>(51, "3"), new Tuple<int, string>(52, "4"),
            new Tuple<int, string>(53, "5"), new Tuple<int, string>(54, "6"), new Tuple<int, string>(55, "7"),
            new Tuple<int, string>(56, "8"), new Tuple<int, string>(57, "9"), new Tuple<int, string>(58, ":"),
            new Tuple<int, string>(59, ";"), new Tuple<int, string>(60, "<"), new Tuple<int, string>(61, "="),
            new Tuple<int, string>(62, ">"), new Tuple<int, string>(63, "?"), new Tuple<int, string>(64, "@"),
            new Tuple<int, string>(65, "A"), new Tuple<int, string>(66, "B"), new Tuple<int, string>(67, "C"),
            new Tuple<int, string>(68, "D"), new Tuple<int, string>(69, "E"), new Tuple<int, string>(70, "F"),
            new Tuple<int, string>(71, "G"), new Tuple<int, string>(72, "H"), new Tuple<int, string>(73, "I"),
            new Tuple<int, string>(74, "J"), new Tuple<int, string>(75, "K"), new Tuple<int, string>(76, "L"),
            new Tuple<int, string>(77, "M"), new Tuple<int, string>(78, "N"), new Tuple<int, string>(79, "O"),
            new Tuple<int, string>(80, "P"), new Tuple<int, string>(81, "Q"), new Tuple<int, string>(82, "R"),
            new Tuple<int, string>(83, "S"), new Tuple<int, string>(84, "T"), new Tuple<int, string>(85, "U"),
            new Tuple<int, string>(86, "V"), new Tuple<int, string>(87, "W"), new Tuple<int, string>(88, "X"),
            new Tuple<int, string>(89, "Y"), new Tuple<int, string>(90, "Z"), new Tuple<int, string>(91, "["),
            new Tuple<int, string>(92, "\\"), new Tuple<int, string>(93, "]"), new Tuple<int, string>(94, "^"),
            new Tuple<int, string>(95, "_"), new Tuple<int, string>(96, "`"), new Tuple<int, string>(97, "a"),
            new Tuple<int, string>(98, "b"), new Tuple<int, string>(99, "c"), new Tuple<int, string>(100, "d"),
            new Tuple<int, string>(101, "e"), new Tuple<int, string>(102, "f"), new Tuple<int, string>(103, "g"),
            new Tuple<int, string>(104, "h"), new Tuple<int, string>(105, "i"), new Tuple<int, string>(106, "j"),
            new Tuple<int, string>(107, "k"), new Tuple<int, string>(108, "l"), new Tuple<int, string>(109, "m"),
            new Tuple<int, string>(110, "n"), new Tuple<int, string>(111, "o"), new Tuple<int, string>(112, "p"),
            new Tuple<int, string>(113, "q"), new Tuple<int, string>(114, "r"), new Tuple<int, string>(115, "s"),
            new Tuple<int, string>(116, "t"), new Tuple<int, string>(117, "u"), new Tuple<int, string>(118, "v"),
            new Tuple<int, string>(119, "w"), new Tuple<int, string>(120, "x"), new Tuple<int, string>(121, "y"),
            new Tuple<int, string>(122, "z"), new Tuple<int, string>(123, "{"), new Tuple<int, string>(124, "|"),
            new Tuple<int, string>(125, "}"), new Tuple<int, string>(126, "~"),


            new Tuple<int, string>(196, "LATIN CAPITAL LETTER A WITH DIAERESIS"),
            new Tuple<int, string>(214, "LATIN CAPITAL LETTER O WITH DIAERESIS"),
            new Tuple<int, string>(220, "LATIN CAPITAL LETTER U WITH DIAERESIS"),
            new Tuple<int, string>(228, "LATIN SMALL LETTER A WITH DIAERESIS"),
            new Tuple<int, string>(246, "LATIN SMALL LETTER O WITH DIAERESIS"),
            new Tuple<int, string>(252, "LATIN SMALL LETTER U WITH DIAERESIS"),
            new Tuple<int, string>(223, "LATIN SMALL LETTER SHARP S")
        };

        internal char Hardblank;
        internal int CharHeight;
        internal int Baseline;

        internal int MaxCharWidth;

        internal int OldLayout;

        internal int CommentLinesCount;
        public string[] CommentLines;

        internal int PrintDirection;

        internal FullLayoutEnum FullLayout;

        internal int CodetagCount;

        internal Dictionary<int, FIGChar> Chars = new Dictionary<int, FIGChar>();

        internal void LoadChar(int code, string name, IList<string> charData)
        {
            FIGSubChar[,] charTable = new FIGSubChar[charData.Max(item => item.Length), charData.Count];

            for (int y = 0; y < charData.Count; y++)
            {
                for (int x = 0; x < charData[y].Length; x++)
                {
                    charTable[x, y] = new FIGSubChar(charData[y][x], x, y);
                }
            }

            if (Chars.ContainsKey(code))
                Chars.Remove(code);

            Chars.Add(code, new FIGChar(code, name, new FIGBuffer(charTable)));
        }

        internal string GetCharString(int code)
        {
            if (!Chars.ContainsKey(code))
            {
                if (Chars.ContainsKey(0))
                    code = 0;
                else
                    return "Unknown Char & no default found";
            }

            FIGSubChar[,] charTable = Chars[code].Buffer.data;

            int xDim = charTable.GetLength(0);
            int yDim = charTable.GetLength(1);

            List<string> lines = new List<string>();
            for (int y = 0; y < yDim; y++)
            {
                StringBuilder line = new StringBuilder(xDim);
                for (int x = 0; x < xDim; x++)
                {
                    line.Append(charTable[x, y].Char);
                }
                lines.Add(line.ToString());
            }

            return String.Join("\r\n", lines);
        }

        public FIGBuffer Render(string text)
        {
            FIGBuffer output = new FIGBuffer(0, this.CharHeight);

            foreach (char c in text)
            {
                FIGChar fChar = Chars[Convert.ToInt32(c)];

                

                if (!FullLayout.HasFlag(FullLayoutEnum.Horz_Smush) &&
                    FullLayout.HasFlag(FullLayoutEnum.Horz_Fitting)) // full width
                {
                    output.Width += fChar.Buffer.Width;

                    

                }
                else
                {
                    IList<int> leftMask = output.GetRightSpaceMask();
                    IList<int> rightMask = fChar.Buffer.GetLeftSpaceMask();

                    IList<int> minDist = GetMinDist(leftMask, rightMask);

                    if (FullLayout.HasFlag(FullLayoutEnum.Horz_Smush))
                    {

                    }
                    else if (FullLayout.HasFlag(FullLayoutEnum.Horz_Fitting))
                    {

                    }

                }

            }

            output.Replace(Hardblank, ' ');

            return output;
        }

        private IList<int> GetMinDist(IList<int> firstList, IList<int> secondList)
        {
            List<int> addedLists = new List<int>();

            for (int i = 0; i < firstList.Count; i++)
            {
                addedLists.Add(firstList[i] + secondList[i]);
            }

            return addedLists;
        }
    }
}
