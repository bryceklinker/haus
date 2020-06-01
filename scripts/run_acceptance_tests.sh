#!/bin/bash

set -ex

install_tools() {
  /bin/bash './scripts/install_tools.sh'
}

run_acceptance_tests() {
    pushd './features'
      yarn install
      yarn test
    popd
}

main() {
  install_tools
  run_acceptance_tests
}

main