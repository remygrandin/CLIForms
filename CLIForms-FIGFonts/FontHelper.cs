using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

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

        private static Font LoadFontFromTextFile(string fontFileContent)
        {
            string[] lines = fontFileContent.Split('\n');

            string line0 = lines[0];

            if (!line0.StartsWith("flf2a"))
                throw new Exception("Font file is no starting with flf2a");

            line0 = line0.Substring(5);

            string[] headerParameters = line0.Split(' ');

            if (headerParameters.Length != 9)
                throw new Exception("Font Header decoding problem : parameter count mismatch");

            Font font = new Font();

            // Hardblank Char
            if (headerParameters[0].Length != 1)
                throw new Exception("Font Header decoding problem : hardblank is more than 1 char");

            font.Hardblank = headerParameters[0][0];

            // Char Height
            if(!int.TryParse(headerParameters[1], out font.CharHeight))
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

            // max char width
            if (!int.TryParse(headerParameters[5], out font.CommentLinesCount))
                throw new Exception("Font Header decoding problem : comment lines count is not an integer");

            if (font.CommentLinesCount < 0)
                throw new Exception("Font Header decoding problem : comment lines count is < 0");




            return new Font();
        }

    }
}
