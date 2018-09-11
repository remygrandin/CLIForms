using System;
using CLIForms;
using CLIForms.Components;
using CLIForms.Components.Containers;
using CLIForms.Components.Texts;

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

            Label simpleLabel = new Label(dialog, "Simple Label")
            {
                X = 2,
                Y = 2
            };
            
            Label multiLinesLabel = new Label(dialog, "Multi\nLines\nLabel")
            {
                X = 2,
                Y = 4
            };

            Button buttonup = new Button(dialog, "Up")
            {
                X = 2,
                Y = 8,
                Width = 10
            };

            SingleLineTextbox simpleTextbox = new SingleLineTextbox(dialog, "","PlaceHolder", width: 20)
            {
                X = 2,
                Y = 12
            };
            */

            Tabs tabs = new Tabs(screen,new []{"Texts", "Button", "Texts", "Button", "Texts", "Button", "Texts", "Texts", "Button" , "Texts", "Button" , "Texts", "Button" }, 70, 25)
            {
                X = 1,
                Y = 1
            };

            engine.Start();

            Console.ReadLine();
        }
    }
}
