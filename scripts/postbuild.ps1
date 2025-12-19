param([String]$ProjectName)

Write-Host "=== Starting post-build script ==="

& "C:\Program Files\dotnet\dotnet.exe" publish -c Release -r win-x64 --self-contained true "/p:PublishSingleFile=true" "/p:DebugType=None"

& "C:\Program Files\dotnet\dotnet.exe" publish -c Release -r linux-x64 --self-contained true "/p:PublishSingleFile=true" "/p:DebugType=None"

Rename-Item -Path "bin/Release/net8.0/linux-x64/publish" -NewName "$ProjectName"

& tar -cvf "$ProjectName.tar" -C "bin/Release/net8.0/linux-x64" "$ProjectName"

Write-Host "=== Post-build script completed ==="