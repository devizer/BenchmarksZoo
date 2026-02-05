$netList=@("net10.0", "net5.0")
if (-not ($env:OS -eq "Windows_NT" -and $env:PROCESSOR_ARCHITECTURE -eq "ARM64")) {
    $netList=@("netcoreapp3.1") + $netList
}

Write-Host "BENCHMARK RUNTIMES: [$netList]" -ForegroundColor Magenta
sleep 3
dotnet build -c Release -v:q /p:NoWarn="NETSDK1138"

# required by 3.1 only
$ENV:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="1"

foreach($runtime in $netList) {
   Write-Host "runtime=[$runtime]" -ForegroundColor Magenta
   $output = $(& dotnet run --no-build -c Release -f "$runtime" -- --print-id)
   $lines = $output
   $id = $lines | Select -Last 1
   $id = "$id"
   Write-Host "ID=[$id]" -ForegroundColor Green
   $sep="$([System.IO.Path]::DirectorySeparatorChar)"
   $artifacts_folder="BenchmarkDotNet.Artifacts$($sep)$id"
   dotnet run -c Release -f "$runtime" --runtimes "$runtime" --job Medium --artifacts "$artifacts_folder"
   $mdList = Get-ChildItem -Path "$artifacts_folder" -Filter "*github.md" -Recurse -File
   $reportFile="REPORT $id.REPORT"
   $mdList | % { Copy-Item -Path "$_" -Destination $reportFile -Force }
}

Get-ChildItem -Path "." -Filter "REPORT*.REPORT" -File | sort-object -Property FullName | % { cat "$_" | out-host }
