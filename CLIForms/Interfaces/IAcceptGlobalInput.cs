using CLIForms.Engine.Events;

namespace CLIForms.Interfaces
{
    interface IAcceptGlobalInput
    {
        bool FireGlobalKeypress(KeyboardEvent evt);
    }
}
