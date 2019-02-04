using System.Collections.Generic;
using CLIForms.Components.Globals;
using CLIForms.Components.Misc;
using CLIForms.Engine;

namespace FileBrowser
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine engine = Engine.Instance;

            Screen screen = new Screen();
            engine.ActiveScreen = screen;
            engine.DebugEnabled = true;

            TreeView tree = new TreeView(screen);

            tree.RootNodes = new List<MenuItem>()
            {
                new FSItem("c:\\")
            };

            engine.Start();
        }
    }
}
