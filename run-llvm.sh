#!/usr/bin/env bash
# work=$HOME/build/devizer; mkdir -p $work; cd $work; git clone https://github.com/devizer/BenchmarksZoo; cd BenchmarksZoo; git pull; bash run-llvm.sh 

BENCHMARK_DURATION=${BENCHMARK_DURATION:-Short}
NET_VER=${NET_VER:-net47}

export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
export LC_ALL=en_US.UTF-8
export LANG=en_US.UTF-8
export LANGUAGE=en_US.UTF-8

dotnet restore || true
nuget restore || true; 
pushd BenchmarksZoo

echo "BUILDING...."
dotnet build -c Release -v q
msbuild /t:build /p:Configuration=Release /v:q

echo "AOTING...."
pushd bin/Release/$NET_VER
echo "ABOUT BenchmarksZoo.exe FILE:"
ls -la BenchmarksZoo.exe

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
/usr/lib/mono/gac/System.Core/4.0.0.0__b77a5c561934e089/System.Core.dll
'
if [[ "$PLUS_AOT" ]]; then
    for to_aot in $list_for_aot; do
      echo "AOT: $to_aot"
      time sudo mono --aot=try-llvm -O=all "$to_aot" 
    done
    
    echo ""
    echo "LIBS AFTER SYSTEM LIBRARY AOT:" 
    echo "---------------" 
    mono BenchmarksZoo.exe --help
fi

mono --llvm --aot -O=all BenchmarksZoo.exe
# echo "DRY?"
# mono --llvm BenchmarksZoo.exe --dry
echo "RUNNING (BENCHMARK_DURATION is ${BENCHMARK_DURATION}) NET is ${NET_VER}...."
mono --llvm BenchmarksZoo.exe --help \
   && sudo bash -c "PATH=$PATH mono --llvm BenchmarksZoo.exe ${BENCHMARK_DURATION}" \
   || sudo bash -c "PATH=$PATH mono BenchmarksZoo.exe ${BENCHMARK_DURATION}" \
   
chown -R $(whoami) .

popd
popd
