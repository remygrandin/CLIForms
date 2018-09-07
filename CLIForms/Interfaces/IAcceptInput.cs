using System;

namespace CLIForms.Interfaces
{
    interface IAcceptInput
    {
        bool FireKeypress(ConsoleKeyInfo key);

        event FocusEventHandler Keypress;
    }
}
