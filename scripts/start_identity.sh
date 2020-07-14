#!/usr/bin/env bash

set -ex

function start_identity() {
    pushd "./src/identity/Haus.Identity.Web"
        export ASPNETCORE_ENVIRONMENT="Development"
        export ASPNETCORE_URLS="http://+:5002;https://+:5003"
        dotnet run
    popd
}

start_identity