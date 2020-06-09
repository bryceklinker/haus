#!/bin/bash

set -ex

CERT_DIRECTORY="${HOME}/.aspnet/https"
CERT_CONF_PATH="./scripts/haus-cert.conf"
CERT_KEY_PATH="${CERT_DIRECTORY}/haus.key"
CERT_PFX_PATH="${CERT_DIRECTORY}/haus.pfx"
CERT_CRT_PATH="${CERT_DIRECTORY}/haus.crt"
CERT_PASSWORD="haus"

generate_dev_cert() {
    mkdir -p ${CERT_DIRECTORY}
    openssl req -x509 \
        -nodes \
        -days 365 \
        -newkey rsa:2048 \
        -keyout ${CERT_KEY_PATH} \
        -out ${CERT_CRT_PATH} \
        -subj '/CN=localhost'
}

convert_cert_to_pfx() {
    openssl pkcs12 -export \
        -out ${CERT_PFX_PATH} \
        -inkey ${CERT_KEY_PATH} \
        -in ${CERT_CRT_PATH} \
        -passout pass:${CERT_PASSWORD}
}

trust_dev_cert() {
 sudo cp ${CERT_CRT_PATH} /usr/local/share/ca-certificates
 sudo update-ca-certificates
}

main() {
  generate_dev_cert
  convert_cert_to_pfx
  trust_dev_cert
}

main