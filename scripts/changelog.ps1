param(
    [Parameter(Mandatory=$true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    Write-Error "Error: Version must be in semver format (e.g., 1.0.0)"
    exit 1
}

$RepoUrl = "https://github.com/spokesoft/lucy"
$ChangelogFile = "CHANGELOG.md"
$Date = Get-Date -Format "yyyy-MM-dd"

if (-not (Test-Path $ChangelogFile)) {
    Write-Error "Error: $ChangelogFile not found"
    exit 1
}

$changelogContent = Get-Content $ChangelogFile -Raw

$unreleasedMatch = [regex]::Match($changelogContent, '(?ms)^## \[Unreleased\]\r?\n(.*?)(?=^## \[|\z)')
if (-not $unreleasedMatch.Success) {
    Write-Error "Error: No Unreleased section found in $ChangelogFile"
    exit 1
}

$unreleased = $unreleasedMatch.Groups[1].Value

$contentLines = $unreleased -split "`r?`n" | Where-Object {
    $_ -match '\S' -and $_ -notmatch '^###\s+(Added|Changed|Deprecated|Removed|Fixed|Security)\s*$'
}

if ($contentLines.Count -eq 0) {
    Write-Error "Error: Unreleased section is empty"
    exit 1
}

function Get-Section {
    param([string]$Content, [string]$Header)
    $pattern = "(?ms)^### $Header\s*`$\r?`n(.*?)(?=^###|\z)"
    $match = [regex]::Match($Content, $pattern)
    if ($match.Success) {
        return $match.Groups[1].Value.Trim()
    }
    return ""
}

$added = Get-Section $unreleased "Added"
$changed = Get-Section $unreleased "Changed"
$deprecated = Get-Section $unreleased "Deprecated"
$removed = Get-Section $unreleased "Removed"
$fixed = Get-Section $unreleased "Fixed"
$security = Get-Section $unreleased "Security"

$newVersionSection = "## [$Version] - $Date"

if ($added) { $newVersionSection += "`n`n### Added`n`n$added" }
if ($changed) { $newVersionSection += "`n`n### Changed`n`n$changed" }
if ($deprecated) { $newVersionSection += "`n`n### Deprecated`n`n$deprecated" }
if ($removed) { $newVersionSection += "`n`n### Removed`n`n$removed" }
if ($fixed) { $newVersionSection += "`n`n### Fixed`n`n$fixed" }
if ($security) { $newVersionSection += "`n`n### Security`n`n$security" }

$unreleasedTemplate = @"
## [Unreleased]

### Added
### Changed
### Deprecated
### Removed
### Fixed
### Security
"@

$headerMatch = [regex]::Match($changelogContent, '(?ms)^(.*?)(?=^## \[Unreleased\])')
$header = $headerMatch.Groups[1].Value

$existingVersions = ""
$existingVersionsMatch = [regex]::Matches($changelogContent, '(?ms)^## \[\d+\.\d+\.\d+\].*?(?=^## \[|^\[.*\]:|\z)')
if ($existingVersionsMatch.Count -gt 0) {
    $existingVersions = ($existingVersionsMatch | ForEach-Object { $_.Value }) -join "`n"
    $existingVersions = $existingVersions.TrimEnd()
}

$newContent = $header + $unreleasedTemplate + "`n`n" + $newVersionSection

if ($existingVersions) {
    $newContent += "`n`n" + $existingVersions
}

$linksMatch = [regex]::Match($changelogContent, '(?ms)(^\[.*\]:.*$)+')
if ($linksMatch.Success) {
    $allExistingLinks = $linksMatch.Value
    $existingVersionLinks = ($allExistingLinks -split "`r?`n" | Where-Object { $_ -match '^\[\d' }) -join "`n"
    $prevVersionMatch = [regex]::Match($existingVersionLinks, '^\[(\d+\.\d+\.\d+)\]')

    if ($prevVersionMatch.Success) {
        $prevVersion = $prevVersionMatch.Groups[1].Value
        $newContent += "`n`n[unreleased]: $RepoUrl/compare/v$Version...HEAD`n"
        $newContent += "[$Version]: $RepoUrl/compare/v$prevVersion...v$Version"
        if ($existingVersionLinks) {
            $newContent += "`n$existingVersionLinks"
        }
    } else {
        $newContent += "`n`n[unreleased]: $RepoUrl/compare/v$Version...HEAD`n"
        $newContent += "[$Version]: $RepoUrl/releases/tag/v$Version"
    }
} else {
    $newContent += "`n`n[unreleased]: $RepoUrl/compare/v$Version...HEAD`n"
    $newContent += "[$Version]: $RepoUrl/releases/tag/v$Version"
}

$newContent | Set-Content $ChangelogFile -NoNewline

Write-Host "Updated $ChangelogFile with version $Version" -ForegroundColor Green
