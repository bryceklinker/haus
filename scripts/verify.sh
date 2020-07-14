#!/usr/bin/env bash

set -ex

run_unit_tests() {
    echo "**************************** STARTING UNIT TESTS ****************************"
    /bin/bash './scripts/run_unit_tests.sh'
    echo "**************************** FINISHED UNIT TESTS ****************************"
}

run_acceptance_tests() {
    echo "**************************** STARTING ACCEPTANCE TESTS ****************************"
    /bin/bash './scripts/run_acceptance_tests.sh'
    echo "**************************** FINISHED ACCEPTANCE TESTS ****************************"
}

main() {
    run_unit_tests
    run_acceptance_tests
}

main