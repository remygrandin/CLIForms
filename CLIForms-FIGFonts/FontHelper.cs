using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace CLIForms_FIGFonts
{
    public static class FontFactory
    {
        private static readonly ResourceSet Fonts = FontRepository.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);

        public static IEnumerable<string> GetAvailableFonts()
        {
            return Fonts.Cast<DictionaryEntry>().Select(item => item.Key.ToString());
        }

        public static Font GetFont(string fontName)
        {
            if(Fonts.Cast<DictionaryEntry>().All(item => item.Key.ToString() != fontName))
                throw new ArgumentException("Unknown Font");

            MemoryStream fileStream = new MemoryStream((byte[])Fonts.GetObject(fontName));

            StreamReader streamReader = new StreamReader(fileStream);

            LoadFontFromTextFile(streamReader.ReadToEnd());

            return new Font();
        }

        private static object lockobj = new object();

        private static Font LoadFontFromTextFile(string fontFileContent)
        {
            lock (lockobj)
            {
                string[] lines = new string[0];

                if (fontFileContent.Contains("\r\n"))
                    lines = Regex.Split(fontFileContent, "\r\n", RegexOptions.Multiline | RegexOptions.Multiline);
                else
                    lines = Regex.Split(fontFileContent, "(\r)|(\n)", RegexOptions.Multiline | RegexOptions.Multiline);

                string line0 = lines[0];

                if (!line0.StartsWith("flf2a"))
                    throw new Exception("Font file is no starting with flf2a");

                line0 = line0.Substring(5);

                string[] headerParameters = line0.Split(' ');

                if (headerParameters.Length < 7)
                    throw new Exception("Font Header decoding problem : parameter count mismatch");

                Font font = new Font();

                // Hardblank Char
                if (headerParameters[0].Length != 1)
                    throw new Exception("Font Header decoding problem : hardblank is more than 1 char");

                font.Hardblank = headerParameters[0][0];

                // Char Height
                if (!int.TryParse(headerParameters[1], out font.CharHeight))
                    throw new Exception("Font Header decoding problem : char height is not an integer");

                // Baseline
                if (!int.TryParse(headerParameters[2], out font.Baseline))
                    throw new Exception("Font Header decoding problem : baseline is not an integer");

                if (font.Baseline < -1 || font.Baseline > font.CharHeight)
                    throw new Exception("Font Header decoding problem : baseline is < 1 or greater than char height");

                // max char width
                if (!int.TryParse(headerParameters[3], out font.MaxCharWidth))
                    throw new Exception("Font Header decoding problem : max char width is not an integer");

                if (font.MaxCharWidth < 0)
                    throw new Exception("Font Header decoding problem : max char width is < 0");

                // old layout
                if (!int.TryParse(headerParameters[4], out font.OldLayout))
                    throw new Exception("Font Header decoding problem : old layout is not an integer");

                // comment lines count
                if (!int.TryParse(headerParameters[5], out font.CommentLinesCount))
                    throw new Exception("Font Header decoding problem : comment lines count is not an integer");

                if (font.CommentLinesCount < 0)
                    throw new Exception("Font Header decoding problem : comment lines count is < 0");

                // Print direction
                if (!int.TryParse(headerParameters[6], out font.PrintDirection))
                    throw new Exception("Font Header decoding problem : print direction is not an integer");

                if (font.PrintDirection != 0 && font.PrintDirection != 1)
                    throw new Exception("Font Header decoding problem : print direction is not 0 or 1");

                if (headerParameters.Length >= 8)
                {
                    // full layout
                    if (!int.TryParse(headerParameters[7], out font.FullLayout))
                        throw new Exception("Font Header decoding problem : full layout is not an integer");

                }

                if (headerParameters.Length >= 9)
                {
                    // Code Tagged Count
                    if (!int.TryParse(headerParameters[8], out font.CodetagCount))
                        throw new Exception("Font Header decoding problem : codetag count is not an integer");

                    if (font.CodetagCount < 0)
                        throw new Exception("Font Header decoding problem : print direction is not 0 or 1");
                }

                // Extracting comments lines
                font.CommentLines = lines.Skip(1).Take(font.CommentLinesCount).ToArray();


                bool baseExtract = true;
                int baseExtractOffset = 0;
                int lineOffset = font.CommentLinesCount + 1;

                while (lineOffset < lines.Length)
                {
                    if (baseExtract)
                    {
                        List<string> charLines = new List<string>();

                        for (int i = lineOffset; i < lineOffset + font.CharHeight; i++)
                        {
                            string line = lines[i];

                            char lineLastChar = line.ToCharArray().Last();

                            while (lineLastChar == line.ToCharArray().Last())
                            {
                                line = line.Substring(0, line.Length - 1);
                            }

                            charLines.Add(line);
                        }

                        font.LoadChar(Font.BaseCharCodes[baseExtractOffset].Item1, Font.BaseCharCodes[baseExtractOffset].Item2, charLines);

                        lineOffset += font.CharHeight;
                        baseExtractOffset++;

                        if (baseExtractOffset >= Font.BaseCharCodes.Length)
                            baseExtract = false;
                    }
                    else
                    {
                        string charCodeStr = lines[lineOffset].Split(' ').First();
                        string charName = String.Join(" ", lines[lineOffset].Split().Skip(1));

                        int charCode;
                        bool toNegative = false;
                        if (charCodeStr.StartsWith("-"))
                        {
                            toNegative = true;
                            charCodeStr = charCodeStr.Substring(1);
                        }

                        if (charCodeStr.ToLower().StartsWith("0x")) // Octal
                        {
                            charCode = Convert.ToInt32(charCodeStr, 16);
                        }
                        else if (charCodeStr.StartsWith("0"))
                        {
                            charCode = Convert.ToInt32(charCodeStr, 8);
                        }
                        else
                        {
                            charCode = Convert.ToInt32(charCodeStr, 10);
                        }

                        if(toNegative)
                        {
                            charCode = charCode * -1;
                        }

                        List<string> charLines = new List<string>();

                        lineOffset++;

                        for (int i = lineOffset; i < lineOffset + font.CharHeight; i++)
                        {
                            string line = lines[i];

                            char lineLastChar = line.ToCharArray().Last();

                            while (lineLastChar == line.ToCharArray().Last())
                            {
                                line = line.Substring(0, line.Length - 1);
                            }

                            charLines.Add(line);
                        }

                        font.LoadChar(charCode, charName, charLines);

                        lineOffset += font.CharHeight;

                    }



                }

                string a = font.GetCharString(96);

                foreach (KeyValuePair<int, FigChar> figChar in font.Chars)
                {
                    Debug.WriteLine("char : " + figChar.Value.Code + "," + figChar.Value.Name);

                    for (int y = 0; y < figChar.Value.Data.GetLength(1); y++)
                    {
                        string str = "";
                        for (int x = 0; x < figChar.Value.Data.GetLength(0); x++)
                        {
                            str += figChar.Value.Data[x, y];

                        }
                        Debug.WriteLine(str);
                    }
                }


                return font;
            }
        }

    }
}
