
namespace CLIForms.Extentions
{
    public static class StringExtentions
    {
        public static string Truncate(this string str, int length, bool ellipsis = true)
        {
            if (str == null || str.Length <= length || str.Length == 0)
                return str;


            if (ellipsis)
            {
                if (length > 3)
                    return str.Substring(0, length - 3) + "...";
                else if (length == 3)
                    return str.Substring(0, length - 2) + "..";
                else if (length == 2)
                    return str.Substring(0, length - 1) + ".";
                else if (length == 1)
                    return ".";
            }

            return str.Substring(0, length);
        }

        public static bool IsPrintable(this char chr)
        {
            string printableChar = "abcdefghijklmnopqrstuvwxyz" +
                                   "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                                   "1234567890" +
                                   "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~" +
                                   "ÇüéâäàåçêëèïîìÄÅÉæÆôöòûùÿÖÜø£Ø×ƒáíóúñÑªº¿®¬½¼¡«»" +
                                   "░▒▓│┤ÁÂÀ©╣║╗╝¢¥┐└┴┬├─┼ãÃ╚╔╩╦╠═╬¤" +
                                   "ðÐÊËÈıÍÎÏ┘┌█▄¦Ì▀ÓßÔÒõÕµþÞÚÛÙýÝ¯´";

            return printableChar.Contains(chr.ToString());
        }

    }
}
