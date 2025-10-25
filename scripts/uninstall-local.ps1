$packageId = "Lucy.Console"

if (-not (dotnet tool uninstall --global $packageId)) {
    Write-Host "Tool uninstallation failed. Ensure the tool is installed or check for errors." -ForegroundColor Red
    exit 1
}

Write-Host "$packageId was uninstalled."
