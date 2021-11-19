#!/usr/bin/env bash
set -ex

source ./scripts/variables.sh

function main() {
  yarn acceptance_tests:test
}

main