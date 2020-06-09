#!/bin/bash

set -ex

PFX_CERT_PATH="${HOME}/.aspnet/https/haus.pfx"
CERT_PASSWORD="haus"

generate_dev_cert() {
#    sudo security set-key-partition-list -D localhost -S unsigned:,teamid:UBF8T346G9 /Users/runner/Library/Keychains/login.keychain-db
#    sudo dotnet dev-certs https -ep ${PFX_CERT_PATH} -p ${CERT_PASSWORD}
    echo "WAIT"
}

trust_dev_cert() {
#    sudo dotnet dev-certs https --trust
#    ls -la ~/.aspnet/https
echo "Something"
}

main() {
  generate_dev_cert
  trust_dev_cert
}

main