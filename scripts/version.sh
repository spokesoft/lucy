#!/usr/bin/env bash
# update.sh - Update version numbers in Directory.Build.Props

set -e

if [[ $# -lt 1 ]]; then
  echo "Usage: $0 <version> [informational-version]"
  exit 1
fi

VERSION="$1"
INFORMATIONAL_VERSION="${2:-$VERSION}"
PROPS="Directory.Build.Props"
PREV_VERSION=$(grep -oP '(?<=<Version>)[^<]+' "$PROPS")
PREV_INFO_VERSION=$(grep -oP '(?<=<InformationalVersion>)[^<]+' "$PROPS")

sed -i \
  -e "s#<Version>.*</Version>#<Version>$VERSION</Version>#" \
  -e "s#<FileVersion>.*</FileVersion>#<FileVersion>${VERSION}.0</FileVersion>#" \
  -e "s#<AssemblyVersion>.*</AssemblyVersion>#<AssemblyVersion>${VERSION}.0</AssemblyVersion>#" \
  -e "s#<InformationalVersion>.*</InformationalVersion>#<InformationalVersion>$INFORMATIONAL_VERSION</InformationalVersion>#" \
  "$PROPS"

echo "Updated Version from $PREV_VERSION to $VERSION"
echo "Updated InformationalVersion from $PREV_INFO_VERSION to $INFORMATIONAL_VERSION"
