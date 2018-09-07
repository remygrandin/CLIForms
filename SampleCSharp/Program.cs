using System;
using CLIForms;
using CLIForms.Components;
using CLIForms.Components.Text;

namespace SampleCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine engine = Engine.Instance;

            Screen screen = new Screen();

            engine.ActiveScreen = screen;

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
                X = 15,
                Y = 10,
                Width = 10
            };

            Button buttoncenter = new Button(dialog, "Center")
            {
                X = 15,
                Y = 14,
                Width = 10
            };

            Button buttonDown = new Button(dialog, "Down")
            {
                X = 15,
                Y = 18,
                Width = 10
            };

            Button buttonLeft = new Button(dialog, "Left")
            {
                X = 4,
                Y = 14,
                Width = 10
            };


            Button buttonRight = new Button(dialog, "Right")
            {
                X = 26,
                Y = 14,
                Width = 10
            };


            engine.Start();

            Console.ReadLine();
        }
    }
}
