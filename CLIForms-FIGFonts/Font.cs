using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLIForms_FIGFonts
{
    public class Font
    {
        internal static int[] BaseCharcodes = new[]
        {
            32, 33, 34, 35, 36,
            37, 38, 39, 40, 41,
            42, 43, 44, 45, 46,
            47, 48, 49, 50, 51,
            52, 53, 54, 55, 56,
            57, 58, 59, 60, 61,
            62, 63, 64, 65, 66,
            67, 68, 69, 70, 71,
            72, 73, 74, 75, 76,
            77, 78, 79, 80, 81,
            82, 83, 84, 85, 86,
            87, 88, 89, 90, 91,
            92, 93, 94, 95, 96,
            97, 98, 99, 100, 101,
            102, 103, 104, 105, 106,
            107, 108, 109, 110, 111,
            112, 113, 114, 115, 116,
            117, 118, 119, 120, 121,
            122, 123, 124, 125, 126,
            196, 214, 220, 228, 246,
            252, 223
        };

        internal char Hardblank;
        internal int CharHeight;
        internal int Baseline;

        internal int MaxCharWidth;

        internal int OldLayout;

        internal int CommentLinesCount;
        public string[] CommentLines;

        internal int PrintDirection;

        internal int FullLayout;

        internal int CodetagCount;

        internal Dictionary<int,char[,]> Chars = new Dictionary<int, char[,]>();

        internal void LoadChar(int code, IList<string> charData)
        {
            char[,] charTable = new char[charData.Max(item => item.Length), charData.Count];

            for (int y = 0; y < charData.Count; y++)
            {
                for (int x = 0; x < charData[y].Length; x++)
                {
                    charTable[x, y] = charData[y][x];
                }
            }

            if (Chars.ContainsKey(code))
                Chars.Remove(code);

            Chars.Add(code, charTable);
        }

        internal string GetCharString(int code)
        {
            if (!Chars.ContainsKey(code))
                return "invalidChar";

            char[,] charTable = Chars[code];

            int xDim = charTable.GetLength(0);
            int yDim = charTable.GetLength(1);

            List<string> lines = new List<string>();
            for (int y = 0; y < yDim; y++)
            {
                StringBuilder line = new StringBuilder(xDim);
                for (int x = 0; x < xDim; x++)
                {
                    line.Append(charTable[x, y]);
                }
                lines.Add(line.ToString());
            }

            return String.Join("\r\n", lines);
        }
    }
}
