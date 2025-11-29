$solutionDir = Split-Path -Parent $PSScriptRoot
$docsDir = Join-Path -Path $solutionDir -ChildPath "docs"
$consoleDir = Join-Path -Path $solutionDir -ChildPath "src/Console"
$consoleProj = Join-Path -Path $consoleDir -ChildPath "Console.csproj"
$binDir = Join-Path -Path $consoleDir -ChildPath "bin/Release/net8.0"
$lucy = Join-Path -Path $binDir -ChildPath "Lucy.Console.exe"

function GetCommandsFromHelp {
    param (
        $command
    )
    $helpOutput = & $lucy $command --help
    $commands = @()
    $commandsSection = $false
    foreach ($line in $helpOutput) {
        if ($line -match '^COMMANDS:') {
            $commandsSection = $true
            continue
        }
        if ($commandsSection) {
            if ($line -match '^\s*$') { break }
            if ($line -match '^\s+(\w+)\s+') {
                $commands += $Matches[1]
            }
        }
    }
    return $commands
}

function ExtractDescription {
    param (
        $helpOutput
    )
    $descriptionSection = $false
    foreach ($line in $helpOutput) {
        if ($line -match '^DESCRIPTION:') {
            $descriptionSection = $true
            continue
        }
        if ($descriptionSection) {
            if ($line -match '^\s*$') { break }
            return $line.Trim()
        }
    }
    return "No description available."
}

function ExtractUsage {
    param (
        $helpOutput
    )
    $usageSection = $false
    foreach ($line in $helpOutput) {
        if ($line -match '^USAGE:') {
            $usageSection = $true
            continue
        }
        if ($usageSection) {
            if ($line -match '^\s*$') { break }
            return $line.Trim()
        }
    }
    return "No usage information available."
}

function ExtractExamples {
    param (
        $helpOutput
    )
    $examples = @()
    $examplesSection = $false
    foreach ($line in $helpOutput) {
        if ($line -match '^EXAMPLES:') {
            $examplesSection = $true
            continue
        }
        if ($examplesSection) {
            if ($line -match '^\s*$') { break }
            $examples += $line.Trim()
        }
    }
    if ($examples.Count -gt 0) {
        return $examples -join "`n"
    }
    return "No examples available."
}

function ExtractArguments {
    param (
        $helpOutput
    )
    $arguments = @()
    $argumentsSection = $false
    foreach ($line in $helpOutput) {
        if ($line -match '^ARGUMENTS:') {
            $argumentsSection = $true
            continue
        }
        if ($argumentsSection) {
            if ($line -match '^\s*$') { break }
            $arguments += $line.Trim()
        }
    }
    if ($arguments.Count -gt 0) {
        return $arguments -join "`n"
    }
    return "No arguments available."
}

function ExtractOptions {
    param (
        $helpOutput
    )
    $options = @()
    $optionsSection = $false
    foreach ($line in $helpOutput) {
        if ($line -match '^OPTIONS:') {
            $optionsSection = $true
            continue
        }
        if ($optionsSection) {
            if ($line -match '^\s*$') { break }
            $options += $line.Trim()
        }
    }
    if ($options.Count -gt 0) {
        return $options -join "`n"
    }
    return "No options available."
}

Write-Host "=== Generating documentation ==="

if (Test-Path -Path $docsDir) {
    Remove-Item -Recurse -Force -Path $docsDir
}

if (-Not (Test-Path -Path $docsDir)) {
    New-Item -ItemType Directory -Path $docsDir | Out-Null
}

dotnet build $consoleProj --configuration Release

# Get root commands from lucy.exe --help
$lucyHelp = & $lucy --help

# Parse the commands section
$commandsSection = $false
$rootCommands = GetCommandsFromHelp ""
$position = 0
$quote = '"'

foreach ($command in $rootCommands) {
    Write-Host "Generating docs for command: $command"
    if (-Not (Test-Path -Path "$docsDir\$command")) {
        New-Item -ItemType Directory -Path "$docsDir\$command" | Out-Null
    }

    $position++
    $_category_ = "{ ""position"": $position }"
    New-Item -ItemType File -Path "$docsDir\$command\_category_.json" -Value $_category_ | Out-Null

    $commandHelp = & $lucy $command --help
    $branchCommands = GetCommandsFromHelp $command
    $branchPosition = 0

    foreach ($branch in $branchCommands) {
        Write-Host "Generating docs for branch: $command $branch"
        $branchHelp = & $lucy $command $branch --help
        $description = ExtractDescription $branchHelp
        $usage = ExtractUsage $branchHelp
        $examples = ExtractExamples $branchHelp

        $branchPosition++

        $md = @(
            "---"
            "title: $command $branch"
            "description: Documentation for the '$command $branch' command."
            "sidebar_position: $branchPosition"
            "tags: [""$command"", ""$branch""]"
            "---"
            ""
            "# $command $branch"
            "$description"
            ""
            "## Usage"
            "``````"
            "$usage"
            "``````"
            ""
            "## Examples"
            "``````"
            "$examples"
            "``````"
            ""
        )

        $arguments = ExtractArguments $branchHelp
        $options = ExtractOptions $branchHelp

        if ($arguments -ne "No arguments available.") {
            $md += @(
                "## Arguments"
                "``````"
                "$arguments"
                "``````"
                ""
            )
        }

        if ($options -ne "No options available.") {
            $md += @(
                "## Options"
                "``````"
                "$options"
                "``````"
                ""
            )
        }

        $content = $md
        $content | Out-File -FilePath "$docsDir\$command\$branch.md" -Encoding utf8
    }
}
