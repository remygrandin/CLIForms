using System;
using CLIForms.Components;

namespace CLIForms.Interfaces
{
    

    public interface IFocusable
    {
        bool Focused { get; set; }

        event FocusEventHandler FocusIn;
        event FocusEventHandler FocusOut;

        void FireFocusIn(ConsoleKeyInfo? key);
        void FireFocusOut(ConsoleKeyInfo? key);
    }
}
