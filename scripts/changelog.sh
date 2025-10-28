#!/bin/bash

set -e

# Check if version is provided
if [ -z "$1" ]; then
    echo "Error: Version number required"
    echo "Usage: $0 <version>"
    echo "Example: $0 1.0.0"
    exit 1
fi

VERSION="$1"

# Validate version format (basic semver check)
if ! printf '%s' "$VERSION" | grep -E -q '^[0-9]+\.[0-9]+\.[0-9]+(-((alpha(\.[0-9]+)?)|(beta(\.[0-9]+)?)|(rc(\.[0-9]+)?)))?$'; then
    echo "Error: Version must be like 1.0.0 or include a prerelease: 1.0.0-alpha, 1.0.0-alpha.1, 1.0.0-beta, 1.0.0-rc, 1.0.0-rc.1"
    exit 1
fi

REPO_URL="https://github.com/spokesoft/lucy"
CHANGELOG_FILE="CHANGELOG.md"
DATE=$(date +%Y-%m-%d 2>/dev/null || date -I 2>/dev/null || echo "$(date +%Y)-$(date +%m)-$(date +%d)")

# Check if changelog exists
if [ ! -f "$CHANGELOG_FILE" ]; then
    echo "Error: $CHANGELOG_FILE not found"
    exit 1
fi

# Extract unreleased section (everything between ## [Unreleased] and next ## [)
unreleased=$(awk '/^## \[Unreleased\]/{flag=1; next} /^## \[/{flag=0} flag' "$CHANGELOG_FILE")

if [ -z "$unreleased" ]; then
    echo "Error: No 'Unreleased' section found in $CHANGELOG_FILE"
    exit 1
fi

# Check if unreleased section has any content (not just whitespace and headers)
has_content=false
while IFS= read -r line; do
    # Skip empty lines and whitespace
    if [[ "$line" =~ ^[[:space:]]*$ ]]; then
        continue
    fi
    # Skip section headers
    if [[ "$line" =~ ^###[[:space:]]+(Added|Changed|Deprecated|Removed|Fixed|Security)[[:space:]]*$ ]]; then
        continue
    fi
    # If we find any other line, we have content
    has_content=true
    break
done <<< "$unreleased"

if [ "$has_content" = false ]; then
    echo "Error: Unreleased section is empty (no changes documented)"
    exit 1
fi

# Parse sections - extract content between headers
added=$(echo "$unreleased" | awk '/^### Added[[:space:]]*$/{flag=1; next} /^###/{flag=0} flag{print}')
changed=$(echo "$unreleased" | awk '/^### Changed[[:space:]]*$/{flag=1; next} /^###/{flag=0} flag{print}')
deprecated=$(echo "$unreleased" | awk '/^### Deprecated[[:space:]]*$/{flag=1; next} /^###/{flag=0} flag{print}')
removed=$(echo "$unreleased" | awk '/^### Removed[[:space:]]*$/{flag=1; next} /^###/{flag=0} flag{print}')
fixed=$(echo "$unreleased" | awk '/^### Fixed[[:space:]]*$/{flag=1; next} /^###/{flag=0} flag{print}')
security=$(echo "$unreleased" | awk '/^### Security[[:space:]]*$/{flag=1; next} /^###/{flag=0} flag{print}')

# Build new version section
new_version_section="## [$VERSION] - $DATE"

# Trim whitespace and check if sections have content
added_trimmed=$(echo "$added" | sed '/^[[:space:]]*$/d')
changed_trimmed=$(echo "$changed" | sed '/^[[:space:]]*$/d')
deprecated_trimmed=$(echo "$deprecated" | sed '/^[[:space:]]*$/d')
removed_trimmed=$(echo "$removed" | sed '/^[[:space:]]*$/d')
fixed_trimmed=$(echo "$fixed" | sed '/^[[:space:]]*$/d')
security_trimmed=$(echo "$security" | sed '/^[[:space:]]*$/d')

if [ -n "$added_trimmed" ]; then
    new_version_section="$new_version_section

### Added

$added_trimmed"
fi

if [ -n "$changed_trimmed" ]; then
    new_version_section="$new_version_section

### Changed

$changed_trimmed"
fi

if [ -n "$deprecated_trimmed" ]; then
    new_version_section="$new_version_section

### Deprecated

$deprecated_trimmed"
fi

if [ -n "$removed_trimmed" ]; then
    new_version_section="$new_version_section

### Removed

$removed_trimmed"
fi

if [ -n "$fixed_trimmed" ]; then
    new_version_section="$new_version_section

### Fixed

$fixed_trimmed"
fi

if [ -n "$security_trimmed" ]; then
    new_version_section="$new_version_section

### Security

$security_trimmed"
fi

# Create new unreleased template
unreleased_template="
## [Unreleased]

### Added
### Changed
### Deprecated
### Removed
### Fixed
### Security"

# Check if there's a links section
links_line=$(grep -n '^\[.*\]:' "$CHANGELOG_FILE" | head -1 | cut -d: -f1)

# Get the header (everything before ## [Unreleased])
header_end=$(grep -n '^## \[Unreleased\]' "$CHANGELOG_FILE" | cut -d: -f1)
header=$(sed -n "1,$((header_end-1))p" "$CHANGELOG_FILE")

# Get existing version sections (everything between end of Unreleased and start of links)
if [ -n "$links_line" ]; then
    # Find where the Unreleased section ends (next ## [ line)
    unreleased_end=$(awk -v start="$header_end" 'NR > start && /^## \[/ {print NR; exit}' "$CHANGELOG_FILE")
    if [ -n "$unreleased_end" ]; then
        existing_versions=$(sed -n "${unreleased_end},$((links_line-1))p" "$CHANGELOG_FILE")
    else
        existing_versions=""
    fi
else
    # No links section, get everything after Unreleased section
    unreleased_end=$(awk -v start="$header_end" 'NR > start && /^## \[/ {print NR; exit}' "$CHANGELOG_FILE")
    if [ -n "$unreleased_end" ]; then
        existing_versions=$(sed -n "${unreleased_end},\$p" "$CHANGELOG_FILE")
    else
        existing_versions=""
    fi
fi

# Build the new changelog content
new_content="$header
$unreleased_template

$new_version_section"

# Add existing version sections if any
if [ -n "$existing_versions" ]; then
    new_content="$new_content

$existing_versions"
fi

# Handle links section
if [ -n "$links_line" ]; then
    # Extract ALL existing links from the original file
    all_existing_links=$(sed -n "${links_line},\$p" "$CHANGELOG_FILE")

    # Filter out the unreleased link (we'll create a new one)
    existing_version_links=$(echo "$all_existing_links" | grep '^\[[0-9]')

    # Find previous version from existing links (the most recent version, which should be first)
    prev_version=$(echo "$existing_version_links" | head -1 | sed -n 's/^\[\([0-9]\+\.[0-9]\+\.[0-9]\+\)\].*/\1/p')

    if [ -n "$prev_version" ]; then
        # Not first release - add new links plus all existing version links
        new_content="$new_content
[unreleased]: ${REPO_URL}/compare/v${VERSION}...HEAD
[$VERSION]: ${REPO_URL}/compare/v${prev_version}...v${VERSION}"

        # Add all existing version links
        if [ -n "$existing_version_links" ]; then
            new_content="$new_content
$existing_version_links"
        fi
    else
        # First release (links section exists but no version links)
        new_content="$new_content
[unreleased]: ${REPO_URL}/compare/v${VERSION}...HEAD
[$VERSION]: ${REPO_URL}/releases/tag/v${VERSION}"
    fi
else
    # No links section - first release
    new_content="$new_content

[unreleased]: ${REPO_URL}/compare/v${VERSION}...HEAD
[$VERSION]: ${REPO_URL}/releases/tag/v${VERSION}"
fi

# Output the new changelog
echo "$new_content" > "$CHANGELOG_FILE"

echo "✓ Updated $CHANGELOG_FILE with version $VERSION"
