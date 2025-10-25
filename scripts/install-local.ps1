$projectPath = "./src/Console"
$packageId = "Lucy.Console"
$outputDir = "./src/Console/bin/Release"
$toolCommandName = "lucy"

function ExitWithError($message) {
    Write-Host $message -ForegroundColor Red
    exit 1
}

if (-not (Test-Path -Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir | Out-Null
}

if (-not (dotnet build $projectPath --configuration Release)) {
    ExitWithError "Build failed. Exiting."
}

if (-not (dotnet pack $projectPath --configuration Release --output $outputDir)) {
    ExitWithError "Pack failed. Exiting."
}

$nugetPackage = Get-ChildItem -Path $outputDir -Filter "$packageId.*.nupkg" | Select-Object -First 1

if (-not $nugetPackage) {
    ExitWithError "NuGet package not found. Ensure the tool name is correct and the pack step succeeded."
}

if (-not (dotnet tool install --global --add-source $outputDir $packageId)) {
    ExitWithError "Tool installation failed. Exiting."
}

Write-Host "$packageId was successfully installed. Try it now: $toolCommandName -v"
