using System;
using CLIForms;
using CLIForms.Components;
using CLIForms.Components.Containers;
using CLIForms.Components.Drawings;
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
            /*
            Dialog dialog = new Dialog(screen, 70, 25)
            {
                Title = "Sample Dialog N°1",
                X = 1,
                Y = 1
            };

            
            

            
            */

            Tabs tabs = new Tabs(screen,new []{"Texts", "Forms", "Drawings", "Spinners", "Tables"}, 77, 28)
            {
                X = 1,
                Y = 1
            };

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

            // ==== Drawings Tab ====
            Box box = new Box(null,20,10)
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

            // ==== Tables Tab ====
            SimpleTable table = new SimpleTable(null)
            {
                X = 1,
                Y = 1,
                End1 = LineEndingStyle.T,
                End2 = LineEndingStyle.Plus
            };
            tabs.AddChild(hline, "Drawings");

            engine.Start();

            Console.ReadLine();
        }
    }
}
