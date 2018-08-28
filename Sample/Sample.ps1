Import-Module "./CLIForm.dll"

$Root = [CLIForm.Widgets.RootWindow]::new()

$Dialog = [CLIForm.Widgets.Dialog]::new($Root)

$Dialog.Text = "Dialog Title"
$Dialog.Width = 75
$Dialog.Height = 20
$Dialog.Top = 2
$Dialog.Left = 2
$Dialog.Border = [CLIForm.BorderStyle]::Thick

$Root.Run()