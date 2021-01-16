#!/usr/bin/env bash
set -ex

DB_CONTEXT_NAME="HausDbContext"
STARTUP_PROJECT="./src/Haus.Web.Host/Haus.Web.Host.csproj"
CONTEXT_PROJECT="./src/Haus.Core/Haus.Core.csproj"
OUTPUT_DIRECTORY="Common/Storage/Migrations"

function main() {
  MIGRATION_NAME=$1
  
  dotnet ef migrations add "${MIGRATION_NAME}" \
     --startup-project "${STARTUP_PROJECT}" \
     --project "${CONTEXT_PROJECT}" \
     --context "${DB_CONTEXT_NAME}" \
     --output-dir "${OUTPUT_DIRECTORY}"
}

main "${1}"