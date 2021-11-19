#!/usr/bin/env bash
set -ex

source ./scripts/variables.sh

function install_node_packages() {
  yarn install
}

function build_solution() {
  dotnet build /p:Version="${VERSION}"
}

function main() {
  install_node_packages
  build_solution
}

main
