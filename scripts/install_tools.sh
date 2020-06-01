#!/bin/bash

set -ex

YARN_VERSION=${YARN_VERSION:-"1.22.4"}

install_yarn() {
  npm install --global "yarn@${YARN_VERSION}"
}

main() {
  install_yarn
}

main