#!/bin/bash

set -ex

generate_cert() {
  OUTPUT_KEY_PATH=$1
  OUTPUT_CERT_PATH=$2

  rm -f "${OUTPUT_KEY_PATH}"
  rm -f "${OUTPUT_CERT_PATH}"

  openssl req -nodes -new -x509 \
    -keyout "${OUTPUT_KEY_PATH}" \
    -out "${OUTPUT_CERT_PATH}" \
    -subj "/C=US/ST=Iowa/L=IDK/O=Klinker Consulting, LLC./OU=IT/CN=localhost"
}

generate_certs() {
  generate_cert "./src/identity/server.key" "./src/identity/server.cert"
}

main() {
  generate_certs
}

main