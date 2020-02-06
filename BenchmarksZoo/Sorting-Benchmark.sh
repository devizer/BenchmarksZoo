dotnet build -c Release -f netcoreapp2.2 BenchmarksZoo.csproj 
pushd bin/release/netcoreapp2.2
dotnet benchmark BenchmarksZoo.dll -j medium -filter *SortingBenchmark*
popd