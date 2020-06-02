#!/bin/bash

set -ex


run_javascript_unit_tests() {
  pushd $1
    yarn install
    yarn test
  popd
}

run_dotnet_unit_tests() {
  dotnet test
}

main() {
  run_dotnet_unit_tests
}

main