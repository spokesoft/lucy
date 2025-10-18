#!/usr/bin/env bash
# test.sh - Test build script for Lucy

set -e

# Get absolute path to the directory of this script
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BUILD_SCRIPT="$SCRIPT_DIR/build.sh"

bash "$BUILD_SCRIPT" --test
