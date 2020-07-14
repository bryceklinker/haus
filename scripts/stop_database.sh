#!/usr/bin/env bash

set -ex 

function stop_database() {
    pushd "./features"
        node -e 'require("./lib/database-runner").stopDatabase()'
    popd
}

stop_database