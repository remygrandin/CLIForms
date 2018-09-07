
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
    }
}
