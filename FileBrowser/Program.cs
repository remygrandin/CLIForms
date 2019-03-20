using System.Collections.Generic;
using System.IO;
using CLIForms.Components.Drawings;
using CLIForms.Components.Globals;
using CLIForms.Components.Misc;
using CLIForms.Engine;
using CLIForms.Styles;

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

            new Box(screen, screen.Width, screen.Height - 2) {Y = 1, Shadow = ShadowStyle.None};


            //Tree view
            TreeView tree = new TreeView(screen, 30, screen.Height - 4);
            tree.X = 1;
            tree.Y = 2;


            List<MenuItem> drives = new List<MenuItem>();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                drives.Add(new FSItem(drive.Name));
            }

            tree.RootNodes = drives;

            new VericalLine(screen, screen.Height - 2)
            {
                X = tree.Width + 2,
                Y = 1,
                Border = BorderStyle.Thick,
                End1 = LineEndingStyle.T,
                End2 = LineEndingStyle.T
            };



            engine.Start();
        }
    }
}
