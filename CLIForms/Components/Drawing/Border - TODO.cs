using System;
using CLIForms.Buffer;
using CLIForms.Components;
using CLIForms.Components.Containers;

namespace CLIForms.Widgets
{
    public class Border : DisplayObject
    {
        public Border(Container parent) : base(parent)
        {
        }

        public override ConsoleCharBuffer Render()
        {
            throw new NotImplementedException();
        }
    }
}
