Import-Module "./CLIForms.dll"

$Root = [CLIForms.Widgets.RootWindow]::new()

$Dialog = [CLIForms.Widgets.Dialog]::new($Root)
$Dialog.Text = "Dialog Title"
$Dialog.Width = 75
$Dialog.Height = 20
$Dialog.Top = 2
$Dialog.Left = 2
$Dialog.Border = [CLIForms.BorderStyle]::Thick

$Dialog = [CLIForms.Widgets.StatusBar]::new($Root)
$Dialog.TextLeft = "Left Status"
$Dialog.TextCenter = "Center Status"
$Dialog.TextRight = "Right Status"


$Root.Run()

read-host