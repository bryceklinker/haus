#!/usr/bin/env bash

set -ex

function start_database() {
    pushd "./features"
        node -e 'require("./lib/database-runner").startDatabase()'
    popd
}

start_database