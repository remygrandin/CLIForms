using System;
using CLIForms;
using CLIForms.Styles;
using CLIForms.Widgets;

namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = new RootWindow();

            var dialog = new Dialog(root) { Text = "Hello World!", Width = 60, Height = 32, Top = 4, Left = 4, Border = BorderStyle.Thick };


            var list = new ListBox(dialog) { Top = 10, Left = 4, Width = 32, Height = 6, Border = BorderStyle.Thin };
            var line = new VerticalLine(dialog) { Top = 4, Left = 40, Width = 1, Height = 6, Border = BorderStyle.Thick };

            var dialog2 = new Dialog(root) { Text = "ooooh", Width = 32, Height = 5, Top = 6, Left = 6, Border = BorderStyle.Thick, Visible = false };




            for (var i = 0; i < 25; i++ )
            {
                list.Items.Add("Item " + i.ToString());
            }

            root.Run();
        }

    }
}
