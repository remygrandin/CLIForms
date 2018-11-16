param([string]$projectdir = "foo", [string]$outdir = "bar")
Compress-Archive -Path ($projectdir + "\FontsIncluded") -DestinationPath ($projectdir + "\fonts.zip")
write-host "done"