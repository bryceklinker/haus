#!/usr/bin/env bash

set -ex

run_acceptance_tests() {
    pushd './features'
      yarn install
      yarn test
    popd
}

main() {
  run_acceptance_tests
}

main