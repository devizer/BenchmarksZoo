      $ErrorActionPreference = "SilentlyContinue"
      & mono --version 
      & curl.exe --version 
      Write-Host "Downloading mono msi installer"
      & curl.exe -L -o mono-x64.msi https://download.mono-project.com/archive/6.6.0/windows-installer/mono-6.6.0.161-x64-0.msi
      function monoExists { Test-Path "C:\Program Files\Mono\bin" };
      Write-Host "[Before] Mono Installed: $(monoExists)";

      echo "RUNNING MSIEXEC"
      Start-Process "msiexec" @("/i", "mono-x64.msi", "/qn", "/L*v", "mono-installer-x64.log") -Wait
      Write-Host "[After] Mono Installed: $(monoExists)";
      echo "TAIL of mono-installer-x64.log"
      # cat .\mono-installer-x64.log
      get-content .\mono-installer-x64.log -tail 1000 | where {-not ($_ -like 'Property*')}
      
      if (Test-Path "c:\program files\mono\bin") {
        $temp_Path="$($Env:PATH);c:\program files\mono\bin"
        $Env:PATH=$tempPath
        echo "##vso[task.setvariable variable=PATH;isOutput=true]$temp_Path"
        echo "##vso[task.setvariable variable=PATH]$temp_Path"
        & "c:\program files\mono\bin\mono" --version

        # User PATH 
        $prev_user_path=[Environment]::GetEnvironmentVariable("PATH", [System.EnvironmentVariableTarget]::User)
        $new_user_path="$($prev_user_path);c:\program files\mono\bin"
        Write-Host "Prev User PATH: $prev_user_path"
        Write-Host "NEW  User PATH: $new_user_path"
        [Environment]::SetEnvironmentVariable("PATH", $new_user_path, [System.EnvironmentVariableTarget]::User)

        # System PATH 
        $prev_system_path=[Environment]::GetEnvironmentVariable("PATH", [System.EnvironmentVariableTarget]::Machine)
        $new_system_path="$($prev_system_path);c:\program files\mono\bin"
        Write-Host "Prev System PATH: $prev_System_path"
        Write-Host "NEW  System PATH: $new_System_path"
        [Environment]::SetEnvironmentVariable("PATH", $new_System_path, [System.EnvironmentVariableTarget]::Machine)

      }
      ""
