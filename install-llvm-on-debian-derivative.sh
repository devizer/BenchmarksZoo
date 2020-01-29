v=$1
v=${v:-9}
echo Installing LLVM v${v}
lsb_release || sudo apt-get install lsb-release -yqq
code=$(lsb_release -c -s)
echo "
deb http://apt.llvm.org/bionic/ llvm-toolchain-$code-$v main
deb-src http://apt.llvm.org/bionic/ llvm-toolchain-$code-$v main
" | sudo tee /etc/apt/sources.list.d/llvm.list
sudo apt-get update -q
apt-cache policy llvm
sudo apt-get install clang-format clang-tidy clang-tools clang clangd libc++-dev libc++1 libc++abi-dev libc++abi1 libclang-dev libclang1 liblldb-dev libllvm-ocaml-dev libomp-dev libomp5 lld lldb llvm-dev llvm-runtime llvm python-clang -yq
llvm --version


