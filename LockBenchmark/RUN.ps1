dotnet build -c Release
foreach($runtime in "net10.0", "netcoreapp3.1") {
   Write-Host "runtime=[$runtime]" -ForegroundColor Magenta
   $output = $(& dotnet run --no-build -c Release -f "$runtime" -- --print-id)
   $lines = $output
   $id = $lines | Select -Last 1
   $id = "$id"
   Write-Host "ID=[$id]" -ForegroundColor Green
   dotnet run -c Release -f "$runtime" --runtimes "$runtime" | tee "Results $id.log"
}
