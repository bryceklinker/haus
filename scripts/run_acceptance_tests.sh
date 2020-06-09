#!/bin/bash

set -ex

generate_certs() {
  /bin/bash './scripts/generate_certs.sh'
}

run_acceptance_tests() {
    pushd './features'
      yarn install
      yarn test
    popd
}

main() {
  generate_certs
  run_acceptance_tests
}

main