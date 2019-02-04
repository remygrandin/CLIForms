namespace CLIForms.Engine
{
    public delegate void ActivateEvent(Events.Event evt);
    public delegate void KeyboardEvent(Events.KeyboardEvent evt);
    public delegate void MouseEvent(Events.MouseEvent evt);
    public delegate void FocusEvent(Events.FocusEvent evt, Direction vector);
}