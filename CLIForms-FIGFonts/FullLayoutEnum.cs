using System;

namespace CLIForms_FIGFonts
{
    [Flags]
    public enum FullLayoutEnum : Int16
    {
        Horz_Smush_R1 = 1,
        Horz_Smush_R2 = 2,
        Horz_Smush_R3 = 4,
        Horz_Smush_R4 = 8,
        Horz_Smush_R5 = 16,
        Horz_Smush_R6 = 32,

        Horz_Fitting = 64,
        Horz_Smush = 128,


        Vert_Smush_R1 = 256,
        Vert_Smush_R2 = 512,
        Vert_Smush_R3 = 1024,
        Vert_Smush_R4 = 2048,
        Vert_Smush_R5 = 4096,
  
        Vert_Kerning = 8192,
        Vert_Smush = 16384
    }
}
