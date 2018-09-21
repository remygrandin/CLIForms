using System;
using CLIForms;
using CLIForms.Components.Containers;
using CLIForms.Components.Drawings;
using CLIForms.Components.Forms;
using CLIForms.Components.Globals;
using CLIForms.Components.Spinners;
using CLIForms.Components.Tables;
using CLIForms.Components.Texts;
using CLIForms.Styles;

namespace SampleCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine engine = Engine.Instance;

            Screen screen = new Screen();

            engine.ActiveScreen = screen;


            Tabs tabs = new Tabs(screen, new[] { "Texts", "Forms", "Drawings", "Spinners", "Tables" }, 77, 26)
            {
                X = 1,
                Y = 2
            };

            StatusBar statusBar = new StatusBar(screen)
            {
                TextLeft = "Left Text",
                TextCenter = "Status Bar",
                TextRight = "Right Text"
            };

            MenuBar menuBar = new MenuBar(screen, new MenuItem(" ==CLIForms== ", null, true,
                new MenuItem("Menu 1", new MenuItem("Item 1")
                                     , new MenuItem("Item 2")
                                     , new MenuItem("Item 3")),
                new MenuItem("Menu 2"),
                new MenuItem("Menu 3")));


            // ==== Texts Tab ====

            Label simpleLabel = new Label(null, "Simple Label")
            {
                X = 1,
                Y = 1
            };
            tabs.AddChild(simpleLabel, "Texts");

            Label multiLinesLabel = new Label(null, "Multi\nLines\nLabel")
            {
                X = 1,
                Y = 3
            };
            tabs.AddChild(multiLinesLabel, "Texts");

            // ==== Forms Tab ====

            Button button = new Button(null, "Button")
            {
                X = 1,
                Y = 1,
                Width = 10
            };
            tabs.AddChild(button, "Forms");

            SingleLineTextbox simpleTextbox = new SingleLineTextbox(null, "", "PlaceHolder", width: 20)
            {
                X = 1,
                Y = 5
            };
            tabs.AddChild(simpleTextbox, "Forms");

            SingleLineTextbox simpleTextboxPwd = new SingleLineTextbox(null, "", "Password", width: 20)
            {
                X = 1,
                Y = 7,
                IsPassword = true
            };
            tabs.AddChild(simpleTextboxPwd, "Forms");

            Checkbox checkbox = new Checkbox(null, "Checkbox")
            {
                X = 1,
                Y = 9
            };
            tabs.AddChild(checkbox, "Forms");

            Radio radio1 = new Radio(null, "Radio 1")
            {
                X = 1,
                Y = 12
            };
            tabs.AddChild(radio1, "Forms");

            Radio radio2 = new Radio(null, "Radio 2")
            {
                X = 1,
                Y = 14
            };
            tabs.AddChild(radio2, "Forms");

            Toggle toggle = new Toggle(null, "toggle")
            {
                X = 1,
                Y = 16
            };
            tabs.AddChild(toggle, "Forms");

            // ==== Drawings Tab ====
            Box box = new Box(null, 20, 10)
            {
                X = 1,
                Y = 1
            };
            tabs.AddChild(box, "Drawings");

            VericalLine vline = new VericalLine(null, 15)
            {
                X = 25,
                Y = 1
            };
            tabs.AddChild(vline, "Drawings");

            HorizontalLine hline = new HorizontalLine(null, 15)
            {
                X = 27,
                Y = 1,
                End1 = LineEndingStyle.T,
                End2 = LineEndingStyle.Plus
            };
            tabs.AddChild(hline, "Drawings");

            // ==== Spinner Tab ====
            TinySpinner tinySpinner = new TinySpinner(null)
            {
                X = 1,
                Y = 1
            };
            tabs.AddChild(tinySpinner, "Spinners");

            Spinner spinner = new Spinner(null)
            {
                X = 4,
                Y = 1
            };
            tabs.AddChild(spinner, "Spinners");

            // ==== Tables Tab ====
            SimpleTable table = new SimpleTable(null)
            {
                X = 1,
                Y = 1,
                ColumnCount = 5,
                LineCount = 4,
                ColumnsWidth = new[] { 4, 15, 15, 5, 15 },
                ColumnsAlignments = new[] { AlignmentStyle.Right, AlignmentStyle.Left, AlignmentStyle.Left, AlignmentStyle.Right, AlignmentStyle.Left },
                TableStyle = TableStyle.CompactWithHeaderNoExtBorder
            };

            table[0, 0] = "ID";
            table[1, 0] = "Name";
            table[2, 0] = "Surname";
            table[3, 0] = "Age";
            table[4, 0] = "Town";

            table[0, 1] = "1";
            table[1, 1] = "John";
            table[2, 1] = "Doe";
            table[3, 1] = "25";
            table[4, 1] = "Paris";

            table[0, 2] = "2";
            table[1, 2] = "Mickel";
            table[2, 2] = "Mercy";
            table[3, 2] = "30";
            table[4, 2] = "Luxembourg";

            table[0, 3] = "3";
            table[1, 3] = "Albert";
            table[2, 3] = "Wood";
            table[3, 3] = "18";
            table[4, 3] = "JacksonVille";

            tabs.AddChild(table, "Tables");

            tabs.ActiveTab = 1;

            engine.Start();

            Console.ReadLine();
        }
    }
}
