$ErrorActionPreference = "SilentlyContinue"
& mono --version 
& curl.exe --version

Function Download-and-Install-Mono 
{
    param([string] $url)
    $file=[System.IO.Path]::GetFileName($url)
    Write-Host "Downloading mono msi installer $file"
    & curl.exe -L -o mono-x64.msi $url
    # function monoExists { Test-Path "C:\Program Files\Mono\bin" };
    # Write-Host "[Before] Mono Installed: $(monoExists)";

    echo "RUNNING MSIEXEC"
    Start-Process "msiexec" @("/i", "mono-x64.msi", "/qn", "/L*v", "mono-installer-x64.log") -Wait
    # Write-Host "[After] Mono Installed: $(monoExists)";
    echo "TAIL of mono-installer-x64.log"
    get-content .\mono-installer-x64.log -tail 1000 | where {-not ($_ -like 'Property*')}
}

function Patch-Path-to-Mono
{
    param( [string] $path_to_mono )
    if (Test-Path $path_to_mono)
    {
        $temp_Path = "$( $Env:PATH );$($path_to_mono)"
        $Env:PATH = $tempPath
        echo "##vso[task.setvariable variable=PATH;isOutput=true]$temp_Path"
        echo "##vso[task.setvariable variable=PATH]$temp_Path"
        Write-Host "About mono at '$path_to_mono'"
        & "$($path_to_mono)\mono" --version

        # User PATH 
        $prev_user_path = [Environment]::GetEnvironmentVariable("PATH", [System.EnvironmentVariableTarget]::User)
        $new_user_path = "$( $prev_user_path );$($path_to_mono)"
        Write-Host "Prev User PATH: $prev_user_path"
        Write-Host "NEW  User PATH: $new_user_path"
        [Environment]::SetEnvironmentVariable("PATH", $new_user_path, [System.EnvironmentVariableTarget]::User)

        # System PATH 
        $prev_system_path = [Environment]::GetEnvironmentVariable("PATH", [System.EnvironmentVariableTarget]::Machine)
        $new_system_path = "$( $prev_system_path );$($path_to_mono)"
        Write-Host "Prev System PATH: $prev_System_path"
        Write-Host "NEW  System PATH: $new_System_path"
        [Environment]::SetEnvironmentVariable("PATH", $new_System_path, [System.EnvironmentVariableTarget]::Machine)
    }
    else
    {
        Write-Host "Warning! Missing directory '$path_to_mono'"
    }
}

$url="https://download.mono-project.com/archive/6.6.0/windows-installer/mono-6.6.0.161-x64-0.msi"
Download-and-Install-Mono $url
Patch-Path-to-Mono "c:\program files\mono\bin"
""
