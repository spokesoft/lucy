#!/usr/bin/env bash
# release.sh - Release build script for Lucy

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BUILD_SCRIPT="$SCRIPT_DIR/build.sh"

bash "$BUILD_SCRIPT" --test --publish
