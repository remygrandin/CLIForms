using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Ionic.Zip;

namespace CLIForms_FIGFonts
{
    public static class FontFactory
    {
        static FontFactory()
        {
            Stream fontzip = Assembly.GetExecutingAssembly().GetManifestResourceStream("CLIForms_FIGFonts.fonts.zip");

            using (ZipFile zip = ZipFile.Read(fontzip))
            {
                foreach (string entry in zip.EntryFileNames)
                {
                    if (entry.EndsWith(".flf"))
                    {
                        FontList.Add(entry);
                    }
                }
            }
        }

        public static string GetFullName(string fontName)
        {
            string foundName = null;

            // First try fullname
            foundName = FontList.FirstOrDefault(item => item == fontName);

            // Then the ful name with flf extension
            if (foundName == null)
                foundName = FontList.FirstOrDefault(item => item.Contains("/" + fontName + ".flf"));

            // Then with flf extension
            if (foundName == null)
                foundName = FontList.FirstOrDefault(item => item.Contains(fontName + ".flf"));

            // Then any match
            if (foundName == null)
                foundName = FontList.FirstOrDefault(item => item.Contains(fontName));

            return foundName;
        }


        public static List<string> FontList = new List<string>();

        private static Dictionary<string, Font> FontCache = new Dictionary<string, Font>();

        public static Font GetFont(string fontName)
        {
            string fontFullName = GetFullName(fontName);


            if (fontFullName == null)
                throw new ArgumentException("Unknown Font");



            lock (fontLoadLockObj)
            {
                if (FontCache.ContainsKey(fontFullName))
                    return FontCache[fontFullName];

                Stream fontzip = Assembly.GetExecutingAssembly().GetManifestResourceStream("CLIForms_FIGFonts.fonts.zip");

                using (ZipFile zip = ZipFile.Read(fontzip))
                {
                    MemoryStream fileStream = new MemoryStream();

                    zip[fontFullName].Extract(fileStream);

                    fileStream.Position = 0;

                    StreamReader streamReader = new StreamReader(fileStream);

                    Font loadedFont = LoadFontFromTextFile(streamReader.ReadToEnd());

                    FontCache.Add(fontFullName, loadedFont);

                    return loadedFont;
                }
            }
        }

        private static object fontLoadLockObj = new object();

        private static Font LoadFontFromTextFile(string fontFileContent)
        {
            string[] lines = new string[0];

            if (fontFileContent.Contains("\r\n"))
                lines = Regex.Split(fontFileContent, "\\r\\n", RegexOptions.Multiline);
            else if (fontFileContent.Contains("\n"))
                lines = Regex.Split(fontFileContent, "\n", RegexOptions.Multiline);
            else
                lines = Regex.Split(fontFileContent, "\r", RegexOptions.Multiline);

            string line0 = lines[0];

            if (!line0.StartsWith("flf2a"))
                throw new Exception("Font file is no starting with flf2a");

            line0 = line0.Substring(5);

            string[] headerParameters = line0.Split(' ');

            if (headerParameters.Length < 6)
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
            if (headerParameters.Length >= 7)
            {
                if (!int.TryParse(headerParameters[6], out font.PrintDirection))
                    throw new Exception("Font Header decoding problem : print direction is not an integer");

                if (font.PrintDirection != 0 && font.PrintDirection != 1)
                    throw new Exception("Font Header decoding problem : print direction is not 0 or 1");
            }

            if (headerParameters.Length >= 8)
            {
                // full layout
                if (!Enum.TryParse<FullLayoutEnum>(headerParameters[7], out font.FullLayout))
                    throw new Exception("Font Header decoding problem : full layout is not a valid value");

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
                    if (String.IsNullOrWhiteSpace(lines[lineOffset]) && lineOffset == lines.Length - 1)
                        break;

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

                    if (toNegative)
                    {
                        charCode = charCode * -1;
                    }

                    if (charCode == -1)
                        throw new Exception("Error parsing at line " + lineOffset + " : char code can't be -1 per FIG specifications");

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

            return font;

        }

    }
}
