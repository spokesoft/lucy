# update.ps1 - Update version numbers in Directory.Build.Props #

param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Version
)

try {
    $ErrorActionPreference = 'Stop'

    if ($Version.StartsWith('v')) {
        $versionRaw = $Version.Substring(1)
    } else {
        $versionRaw = $Version
    }

    $informationalVersion = $versionRaw
    $propsPath = 'Directory.Build.Props'

    if (-not (Test-Path -Path $propsPath)) {
        Write-Error "Directory.Build.Props not found at path: $propsPath"
        exit 1
    }

    [xml]$xml = Get-Content -Path $propsPath
    $propertyGroup = $null
    if ($xml.Project.PropertyGroup) {
        foreach ($pg in $xml.Project.PropertyGroup) {
            if ($pg.Version) { $propertyGroup = $pg; break }
        }
        if (-not $propertyGroup) { $propertyGroup = $xml.Project.PropertyGroup[0] }
    }

    if (-not $propertyGroup) {
        $propertyGroup = $xml.CreateElement('PropertyGroup')
        $xml.Project.AppendChild($propertyGroup) | Out-Null
    }

    function Get-OrCreateElement {
        param($parent, $name)
        $node = $null
        $node = $parent.SelectSingleNode($name)
        if (-not $node) {
            $node = $xml.CreateElement($name)
            $parent.AppendChild($node) | Out-Null
        }
        return $node
    }

    $prevVersion = ($propertyGroup.Version -as [string])
    $prevInformational = ($propertyGroup.InformationalVersion -as [string])
    $prevFileVer = ($propertyGroup.FileVersion -as [string])
    $prevAssemblyVer = ($propertyGroup.AssemblyVersion -as [string])

    $baseVersion = ($versionRaw -split '[-+]')[0]

    # Split into dot parts and keep numeric prefix only
    $parts = @()
    if ($baseVersion -ne '') {
        $rawParts = $baseVersion -split '\.'
        foreach ($p in $rawParts) {
            if ($p -match '^[0-9]+$') {
                $parts += $p
            } else {
                break
            }
        }
    }

    if ($parts.Count -ge 4) {
        $fileVersion = "{0}.{1}.{2}.{3}" -f $parts[0], $parts[1], $parts[2], $parts[3]
    } else {
        while ($parts.Count -lt 3) { $parts += '0' }
        $fileVersion = "{0}.{1}.{2}.0" -f $parts[0], $parts[1], $parts[2]
    }

    (Get-OrCreateElement -parent $propertyGroup -name 'Version').InnerText = $versionRaw
    (Get-OrCreateElement -parent $propertyGroup -name 'FileVersion').InnerText = $fileVersion
    (Get-OrCreateElement -parent $propertyGroup -name 'AssemblyVersion').InnerText = $fileVersion
    (Get-OrCreateElement -parent $propertyGroup -name 'InformationalVersion').InnerText = $informationalVersion

    $xml.Save($propsPath)

    Write-Host "Updated Version from '$($prevVersion -or '<not found>')' to '$versionRaw'"
    Write-Host "Updated InformationalVersion from '$($prevInformational -or '<not found>')' to '$informationalVersion'"
    Write-Host "Updated FileVersion/AssemblyVersion from '$($prevFileVer -or '<not found>')/$($prevAssemblyVer -or '<not found>')' to '$fileVersion'"

} catch {
    Write-Error "Failed to update versions: $_"
    exit 2
}
