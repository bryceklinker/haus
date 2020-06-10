#!/usr/bin/env bash

set -ex

FEATURES_DIRECTORY="./features"
IDENTITY_DIRECTORY="./src/identity/Haus.Identity.Web"

start_database() {
    pushd ${FEATURES_DIRECTORY}
        docker-compose up -d database
    popd
}

start_app() {
    pushd ${IDENTITY_DIRECTORY}
        dotnet run
    popd
}

stop_database() {
    pushd ${FEATURES_DIRECTORY}
        docker-compose down
    popd
}

main() {
    stop_database
    start_database
    start_app
}

trap "stop_database" EXIT
main