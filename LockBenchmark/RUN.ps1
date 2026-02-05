dotnet build -c Release
$netList=@("net10.0", "net5.0")
if ($env:OS -eq "Windows_NT" -and $env:PROCESSOR_ARCHITECTURE -eq "ARM64") {
    $netList=@("netcoreapp3.1") + $netList
}

Write-Host "BENCHMARK RUNTIMES: [$netList]"
sleep 2

foreach($runtime in $netList) {
   Write-Host "runtime=[$runtime]" -ForegroundColor Magenta
   $output = $(& dotnet run --no-build -c Release -f "$runtime" -- --print-id)
   $lines = $output
   $id = $lines | Select -Last 1
   $id = "$id"
   Write-Host "ID=[$id]" -ForegroundColor Green
   dotnet run -c Release -f "$runtime" --runtimes "$runtime" | tee "Results $id.log"
}
