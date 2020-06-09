#!/bin/bash

set -ex

PFX_CERT_PATH="${HOME}/.aspnet/https/haus.pfx"
CERT_PASSWORD="haus"

generate_dev_cert() {
    sudo security set-key-partition-list -D localhost -S unsigned:,teamid:UBF8T346G9 ~/Library/Keychains/Login.keychain
    dotnet dev-certs https -ep ${PFX_CERT_PATH} -p ${CERT_PASSWORD}
}

trust_dev_cert() {
    dotnet dev-certs https --trust
}

main() {
  generate_dev_cert
  trust_dev_cert
}

main