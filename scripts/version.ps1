# update.ps1 - Update version numbers in Directory.Build.Props

param(
    [string]$Version,
    [string]$InformationalVersion
)

if (-not $Version) {
    Write-Error "Version parameter is required."
    exit 1
}

if (-not $InformationalVersion) {
    $InformationalVersion = $Version
}

$PropsPath = "Directory.Build.Props"

[xml]$xml = Get-Content $PropsPath

$PreviousVersion = $xml.Project.PropertyGroup.Version
$PreviousInformationalVersion = $xml.Project.PropertyGroup.InformationalVersion

$xml.Project.PropertyGroup.Version = $Version
$xml.Project.PropertyGroup.FileVersion = "$Version.0"
$xml.Project.PropertyGroup.AssemblyVersion = "$Version.0"
$xml.Project.PropertyGroup.InformationalVersion = $InformationalVersion

$xml.Save($PropsPath)

Write-Host "Updated Version from $PreviousVersion to $Version"
Write-Host "Updated InformationalVersion from $PreviousInformationalVersion to $InformationalVersion"
