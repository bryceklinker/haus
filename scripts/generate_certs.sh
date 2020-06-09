#!/bin/bash

set -ex

PFX_CERT_PATH="${HOME}/.aspnet/https/haus.pfx"
CRT_CERT_PATH="${HOME}/.aspnet/https/haus.crt"
CA_CERTS_PATH="/usr/local/share/ca-certificates"
CERT_PASSWORD="haus"

generate_dev_cert() {
    dotnet dev-certs https -ep ${PFX_CERT_PATH} -p ${CERT_PASSWORD}
}

trust_dev_cert() {
    openssl pkcs12 -in ${PFX_CERT_PATH} -nokeys -out ${CRT_CERT_PATH} -nodes -passin ${CERT_PASSWORD}
    cp ${CRT_CERT_PATH} ${CA_CERTS_PATH}
    sudo update-ca-certificates
    openssl verify ${CRT_CERT_PATH}
}

main() {
  generate_dev_cert
  trust_dev_cert
}

main