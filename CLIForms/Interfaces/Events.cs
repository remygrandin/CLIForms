using System;
using CLIForms.Components;

namespace CLIForms.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <returns>return true to stop propagation</returns>
    public delegate bool ActivatedEventHandler(DisplayObject sender);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="key"></param>
    /// <returns>return true to stop propagation and prevent default behavior</returns>
    public delegate bool KeyPressEventHandler(DisplayObject sender, ConsoleKeyInfo key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <returns>return true to stop propagation</returns>
    public delegate bool FocusEventHandler(DisplayObject sender);
}