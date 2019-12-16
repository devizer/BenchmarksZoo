#!/usr/bin/env bash
# work=$HOME/build/devizer; mkdir -p $work; cd $work; git clone https://github.com/devizer/BenchmarksZoo; cd BenchmarksZoo; git pull; bash run-llvm.sh 

dotnet restore || true
nuget restore || true; 
cd BenchmarksZoo
echo "BUILDING...."
msbuild /t:rebuild /p:Configuration=Release /v:q
cd bin/Release/net47
echo "ABOUT BenchmarksZoo.exe FILE:"
ls -la BenchmarksZoo.exe
# --llvm?
mono BenchmarksZoo.exe --help
mono --llvm --aot -O=all BenchmarksZoo.exe $1
# mono --llvm BenchmarksZoo.exe $1

cd ..
cd ..



