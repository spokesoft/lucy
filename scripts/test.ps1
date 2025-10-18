# test.ps1 - Test build script for Lucy

$invocationPath = $MyInvocation.MyCommand.Path
$buildDirectory = Split-Path -Parent $invocationPath
$buildScriptPath = Join-Path $buildDirectory "build.ps1"

& $buildScriptPath -Test
