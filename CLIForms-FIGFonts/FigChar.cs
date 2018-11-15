
namespace CLIForms_FIGFonts
{
    public class FIGChar
    {
        public int Code;
        public string Name;
        public FIGBuffer Buffer;

        public FIGChar(int code, string name, FIGBuffer buffer)
        {
            Code = code;
            Name = name;
            Buffer = buffer;
        }
    }
}
