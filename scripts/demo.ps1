#!/usr/bin/env pwsh
# Demo script to set up Lucy with sample data

$ErrorActionPreference = "Stop"

Write-Host "Setting up Lucy demo..." -ForegroundColor Cyan

# Navigate to Console directory
$consoleDir = Join-Path (Join-Path (Join-Path $PSScriptRoot "..") "src") "Console"
$binDir = Join-Path $consoleDir "bin\debug\net8.0"
$bin = Join-Path $consoleDir "bin\debug\net8.0\Lucy.Console.exe"

Push-Location $consoleDir

try {
    # Remove existing database files
    Write-Host "Cleaning up existing database files..." -ForegroundColor Yellow
    if (Test-Path "$binDir\lucy.db") {
        Remove-Item "$binDir\lucy.db" -Force
        Write-Host "  Removed $binDir\lucy.db" -ForegroundColor Gray
    }
    if (Test-Path "$binDir\logs.db") {
        Remove-Item "$binDir\logs.db" -Force
        Write-Host "  Removed $binDir\logs.db" -ForegroundColor Gray
    }

    # Create a Project
    & $bin new project DEMO --name "Demo Project"
    & $bin update project DEMO --description "This is a demo project."

    # Define a workflow
    & $bin new status DEMO REVIEW --order 3 --color yellow
    & $bin update status DEMO REVIEW --name "In Review"
    & $bin update status DEMO REVIEW --description "Tasks that are in review"

    # Manage Tickets
    & $bin new ticket "Implement authentication for pets" --project DEMO
    & $bin new ticket "Implement purr-based two-factor authentication (P2FA)" --project DEMO
    & $bin new ticket "Integrate with laser pointer for user engagement" --project DEMO
    & $bin update ticket DEMO-1 --status DONE
    & $bin update ticket DEMO-2 --status IN-PROGRESS
    & $bin update ticket DEMO-2 --description "See title for details"
    & $bin update ticket DEMO-3 --title "Develop laser pointer feature to redirect unproductive devs"

    # Define Tags
    & $bin new tag URGENT --color red --project DEMO
    & $bin new tag FEATURE --color blue --project DEMO

    # Assign Tags to Tickets
    & $bin add tag DEMO-1 URGENT
    & $bin add tag DEMO-2 FEATURE

    Write-Host "`nDemo setup complete!" -ForegroundColor Green
    & $bin show board DEMO
}
finally {
    Pop-Location
}
