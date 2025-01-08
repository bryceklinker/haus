#!/usr/bin/env bash
set -ex

SOLUTION_PATH="./Haus.sln"
TESTS_DIRECTORY="./tests"
SOURCE_DIRECTORY="./src"

function main() {
  TYPE="${1}"
  PROJECT_NAME="${2}"
  OUTPUT=""
  
  if [[ "${TYPE}" == "test" ]]; then
    OUTPUT="${TESTS_DIRECTORY}/${PROJECT_NAME}"
  fi
  
  if [[ "${TYPE}" == "classlib" ]]; then
      OUTPUT="${SOURCE_DIRECTORY}/${PROJECT_NAME}"
  fi
  
  if [[ "${TYPE}" == "web" ]]; then
      OUTPUT="${SOURCE_DIRECTORY}/${PROJECT_NAME}"
  fi
  
  if [[ "${TYPE}" == "blazorwasm-empty" ]]; then
      OUTPUT="${SOURCE_DIRECTORY}/${PROJECT_NAME}"
  fi
  
  dotnet new "${TYPE}" --name "${PROJECT_NAME}" --output "${OUTPUT}"
  dotnet sln "${SOLUTION_PATH}" add "${OUTPUT}"
}

main "${1}" "${2}"