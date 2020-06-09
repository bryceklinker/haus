#!/usr/bin/env bash

set -ex

IDENTITY_PFX_CERT_PATH="${HOME}/.aspnet/https/haus-identity.pfx"
CERT_PASSWORD="haus"

setup_dev_cert() {
    EXPORT_PATH=$1
    
    dotnet dev-certs https --export-path ${IDENTITY_PFX_CERT_PATH} -p ${CERT_PASSWORD}
    dotnet dev-certs https --trust
}

main() {
    setup_dev_cert ${IDENTITY_PFX_CERT_PATH}
}

main