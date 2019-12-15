# work=$HOME/build/devizer; mkdir -p $work; cd $work; git clone https://github.com/devizer/BenchmarksZoo; cd BenchmarksZoo; git pull; bash run-llvm.sh 

nuget restore || true; dotnet restore || true
cd BenchmarksZoo
msbuild /t:rebuild /p:Configuration=Release /v:q
cd bin/Release/net47
mono --llvm BenchmarksZoo.exe $1
echo "ABOUT BenchmarksZoo.exe FILE:"
ls -la BenchmarksZoo.exe
cd ..
cd ..



