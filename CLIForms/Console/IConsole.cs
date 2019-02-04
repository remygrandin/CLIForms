using System.Collections.Generic;
using CLIForms.Buffer;
using KeyboardEvent = CLIForms.Engine.KeyboardEvent;
using MouseEvent = CLIForms.Engine.MouseEvent;

namespace CLIForms.Console
{
    public interface IConsole
    {
        int Width { get; set; }
        int Height { get; set; }

        bool Draw { get; set; }

        void Init();
        void End();

        void Display(ConsoleCharBuffer buffer);
        void Display(IEnumerable<PositionedConsoleChar> chars);

        event KeyboardEvent KeyboardEvent;
        event MouseEvent MouseEvent;

        void StartCapture();
    }
}
