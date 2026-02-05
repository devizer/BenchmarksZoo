if ((Get-Os-Platform) -eq "Windows") { $work="C:\Work\LockBenchmark" } Else { $work="$($ENV:HOME)/Work/LockBenchmark" }
Write-Host "WORK IS '$work'" -ForeGroundColor Magenta; & git clone https://github.com/devizer/BenchmarksZoo "$work"; cd "$work"; git reset --hard ; git pull; cd LockBenchmark; pwsh RUN.ps1
