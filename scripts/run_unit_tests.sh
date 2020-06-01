#!/bin/bash

set -ex

install_tools() {
  /bin/bash './scripts/install_tools.sh'
}

run_unit_tests() {
  pushd $1
    yarn install
    yarn test
  popd
}

main() {
  install_tools
  run_unit_tests './src/identity'
}

main