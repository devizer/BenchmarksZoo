@dotnet build -c Release -f netcoreapp2.2 BenchmarksZoo.csproj 
@pushd bin\release\netcoreapp2.2
@rem dotnet benchmark BenchmarksZoo.dll -j short --warmupCount 4 -filter *SortingBenchmark*
@rem dotnet benchmark BenchmarksZoo.dll -j dry -filter *SortingBenchmark*
dotnet benchmark BenchmarksZoo.dll -j medium -filter *SortingBenchmark*
@popd