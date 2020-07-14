#!/usr/bin/env bash

set -ex


run_javascript_unit_tests() {
  pushd $1
    yarn install
    yarn test --watchAll=false
  popd
}

run_dotnet_unit_tests() {
  dotnet test
}

main() {
  run_javascript_unit_tests "./src/web/Haus.Web/client-app"
  run_dotnet_unit_tests
}

main