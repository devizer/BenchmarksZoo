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

echo ""
echo "LIBS BEFORE AOT:"
echo "----------------" 
mono BenchmarksZoo.exe --help
mono --aot=try-llvm -O=all BenchmarksZoo.exe

echo ""
echo "LIBS AFTER AOT:" 
echo "---------------" 
mono BenchmarksZoo.exe --help

list_for_aot='
/usr/lib/mono/gac/System/4.0.0.0__b77a5c561934e089/System.dll
./BenchmarkDotNet.dll
/usr/lib/mono/gac/System.Core/4.0.0.0__b77a5c561934e089/System.Core.dll
./netstandard.dll
'

if [[ $"PLUS_AOT" ]]; then
    for to_aot in $list_for_aot; do
      echo "AOT: $to_aot"
      time mono --aot=try-llvm -O=all "$to_aot" 
    done
    
    echo ""
    echo "LIBS AFTER SYSTEM LIBRARY AOT:" 
    echo "---------------" 
    mono BenchmarksZoo.exe --help
fi



mono --llvm --aot -O=all BenchmarksZoo.exe $1
# mono --llvm BenchmarksZoo.exe $1

cd ..
cd ..



