using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLIForms_FIGFonts
{
    public class FigChar
    {
        public int Code;
        public string Name;
        public char[,] Data;

        public FigChar(int code, string name, char[,] data)
        {
            Code = code;
            Name = name;
            Data = data;
        }
    }
}
