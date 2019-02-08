using System.Collections.Generic;
using System.IO;
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

            List<MenuItem> drives = new List<MenuItem>();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                drives.Add(new FSItem(drive.Name));
            }

            tree.RootNodes = drives;

            engine.Start();
        }
    }
}
