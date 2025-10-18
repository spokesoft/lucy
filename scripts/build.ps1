# build.ps1 - Build and test script for Lucy

param(
    [string]$Configuration = "Release",
    [string]$Runtime = "",
    [switch]$Test,
    [switch]$Publish,
    [string]$Output = "artifacts"
)

$ConsoleProj = Join-Path -Path (Get-Location) -ChildPath "src/Console/Console.csproj"

Write-Host "=== Restoring dependencies ==="
dotnet restore

Write-Host "=== Building project ($Configuration) ==="
dotnet build --configuration $Configuration --no-restore

if ($Test) {
    Write-Host "=== Running tests ==="
    dotnet test --no-restore --configuration $Configuration
}

if ($Publish) {
    $publishArgs = @(
        "--configuration", $Configuration,
        "--output", $Output
    )
    if ($Runtime -ne "") {
        $publishArgs += "--runtime"
        $publishArgs += $Runtime
        $publishArgs += "--self-contained"
        $publishArgs += "true"
    }
    $RuntimeTag = if ($Runtime -ne "") { $Runtime } else { "portable" }
    Write-Host "=== Publishing ($Configuration, $RuntimeTag) to '$Output' ==="
    dotnet publish $ConsoleProj @publishArgs --no-restore
}

Write-Host "=== Done ==="
