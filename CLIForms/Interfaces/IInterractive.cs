using System;

namespace CLIForms.Interfaces
{
    public interface IInterractive
    {
        // return true to cancel the event propagation (fe : if the component has handled the key press)
        bool KeyPressed(ConsoleKeyInfo key);

        bool Focused { get; set; }

        event FocusEventHandler FocusIn;
        event FocusEventHandler FocusOut;

        void FocusedIn(ConsoleKeyInfo? key);
        void FocusedOut(ConsoleKeyInfo? key);
    }
}
