#!/usr/bin/env bash
opts=$1
pushd $(mktemp -d) >/dev/null
echo 'class Z { public static void Main() { System.Console.WriteLine("SUCCESSFUL COMPILATION"); }}' > class1.cs
csc /nologo /target:exe /out:class1.exe class1.cs
mono $opts class1.exe
code=$?
echo "exit code: $code"
rm -rf class1* 2>/dev/null
popd >/dev/null
