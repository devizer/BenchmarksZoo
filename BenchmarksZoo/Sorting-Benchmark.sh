dotnet build -c Release -f netcoreapp2.2 BenchmarksZoo.csproj 
pushd bin/Release/netcoreapp2.2
dotnet benchmark BenchmarksZoo.dll -j medium -filter *SortingBenchmark*
popd