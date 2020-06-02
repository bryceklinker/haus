#!/bin/bash

set -ex


run_unit_tests() {
  pushd $1
    yarn install
    yarn test
  popd
}

main() {
  run_unit_tests './src/identity'
}

main