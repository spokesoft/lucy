#!/usr/bin/env bash
# build.sh - Build and test script for Lucy

set -e

CONFIGURATION=Release
RUNTIME=""
TEST=false
PUBLISH=false
OUTPUT="publish"

while [[ $# -gt 0 ]]; do
  case $1 in
    -c|--configuration)
      CONFIGURATION="$2"
      shift 2
      ;;
    -r|--runtime)
      RUNTIME="$2"
      shift 2
      ;;
    -t|--test)
      TEST=true
      shift
      ;;
    -p|--publish)
      PUBLISH=true
      shift
      ;;
    -o|--output)
      OUTPUT="$2"
      shift 2
      ;;
    *)
      echo "Unknown option $1"
      exit 1
      ;;
  esac
done

CONSOLE_PROJ="$(pwd)/src/Console/Console.csproj"

echo "=== Restoring dependencies ==="
dotnet restore

echo "=== Building project ($CONFIGURATION) ==="
dotnet build --configuration "$CONFIGURATION" --no-restore

if [ "$TEST" = true ]; then
  echo "=== Running tests ==="
  dotnet test --no-restore --configuration "$CONFIGURATION"
fi

if [ "$PUBLISH" = true ]; then
  PUBLISH_ARGS=(--configuration "$CONFIGURATION" --output "$OUTPUT" --no-restore)
  if [ -n "$RUNTIME" ]; then
    PUBLISH_ARGS+=(--runtime "$RUNTIME" --self-contained true)
  fi
  echo "=== Publishing ($CONFIGURATION, $RUNTIME) to '$OUTPUT' ==="
  dotnet publish "$CONSOLE_PROJ" "${PUBLISH_ARGS[@]}"
fi

echo "=== Done ==="
