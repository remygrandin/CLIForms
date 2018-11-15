Import-Module "./CLIForms.dll"

$Engine = [CLIForms.Engine]::instance

$screen = (New-Object -TypeName "CLIForms.Components.Screen")

$Engine.ActiveScreen = $screen



<#
$Dialog = [CLIForms.Widgets.Dialog]::new($Root)
$Dialog.Text = "Dialog Title"
$Dialog.Width = 75
$Dialog.Height = 20
$Dialog.Top = 2
$Dialog.Left = 2
$Dialog.Border = [CLIForms.BorderStyle]::Thick

$StatusBar = [CLIForms.Widgets.StatusBar]::new($Root)
$StatusBar.TextLeft = "Left Status"
$StatusBar.TextCenter = "Center Status"
$StatusBar.TextRight = "Right Status"

$Table = [CLIForms.Widgets.Table]::new($Dialog)
$Table.Top = 4
$Table.Left = 2
$Table[0,0] = "1"
$Table[1,0] = "22"
$Table[0,1] = "3333"
$Table[1,1] = "44444444"
$Table.Border = [CLIForms.BorderStyle]::Thick
#>






$StackTrace

read-host