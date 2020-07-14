#!/usr/bin/env bash

set -ex

function start_web() {
    pushd "./src/web/Haus.Web"
        export ASPNETCORE_ENVIRONMENT="Development"
        export ASPNETCORE_URLS="http://+:5000;https://+:5001"
        dotnet run 
    popd
}

start_web