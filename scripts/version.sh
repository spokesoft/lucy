#!/usr/bin/env bash
# version.sh - Update version information in Directory.Build.Props

set -euo pipefail

if [[ $# -ne 1 ]]; then
  echo "Usage: $0 <version>"
  exit 1
fi

INPUT_VERSION="$1"

VERSION_RAW="${INPUT_VERSION#v}"

PROPS="Directory.Build.props"

if [[ ! -f "$PROPS" ]]; then
  echo "Error: $PROPS not found in the current directory."
  exit 1
fi

PREV_VERSION=$(grep -oP '(?<=<Version>)[^<]+' "$PROPS" 2>/dev/null || true)
PREV_INFO_VERSION=$(grep -oP '(?<=<InformationalVersion>)[^<]+' "$PROPS" 2>/dev/null || true)
BASE_VERSION="${VERSION_RAW%%[-+]*}"
IFS='.' read -r -a parts <<< "$BASE_VERSION"

for i in "${!parts[@]}"; do
  if ! [[ "${parts[$i]}" =~ ^[0-9]+$ ]]; then
    parts=("${parts[@]:0:$i}")
    break
  fi
done

if [[ ${#parts[@]} -ge 4 ]]; then
  FILEVER="${parts[0]}.${parts[1]}.${parts[2]}.${parts[3]}"
else
  while [[ ${#parts[@]} -lt 3 ]]; do
    parts+=("0")
  done
  FILEVER="${parts[0]}.${parts[1]}.${parts[2]}.0"
fi

FINAL_VERSION="$VERSION_RAW"
FINAL_INFO="$VERSION_RAW"

sed -i -E \
  -e "s#<Version>[^<]*</Version>#<Version>${FINAL_VERSION}</Version>#" \
  -e "s#<FileVersion>[^<]*</FileVersion>#<FileVersion>${FILEVER}</FileVersion>#" \
  -e "s#<AssemblyVersion>[^<]*</AssemblyVersion>#<AssemblyVersion>${FILEVER}</AssemblyVersion>#" \
  -e "s#<InformationalVersion>[^<]*</InformationalVersion>#<InformationalVersion>${FINAL_INFO}</InformationalVersion>#" \
  "$PROPS"

echo "Updated Version from '${PREV_VERSION:-<not found>}' to '${FINAL_VERSION}'"
echo "Updated InformationalVersion from '${PREV_INFO_VERSION:-<not found>}' to '${FINAL_INFO}'"
echo "Updated FileVersion / AssemblyVersion to '${FILEVER}'"
