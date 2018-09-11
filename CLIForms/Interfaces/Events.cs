using System;
using CLIForms.Components;

namespace CLIForms.Interfaces
{
    public delegate bool KeyPressEventHandler(DisplayObject sender, ConsoleKeyInfo key);
    public delegate bool FocusEventHandler(DisplayObject sender);
}