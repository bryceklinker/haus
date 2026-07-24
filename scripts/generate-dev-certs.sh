#!/usr/bin/env bash
set -ex

WORKING_DIRECTORY=$(pwd)
CERT_PFX_PATH="${WORKING_DIRECTORY}/cert.pfx"
CERT_CRT_PATH="${WORKING_DIRECTORY}/cert.crt"
CERT_KEY_PATH="${WORKING_DIRECTORY}/cert.key"
CERT_PASSWORD="password"

function certs_already_exist() {
  [[ -f "${CERT_PFX_PATH}" && -f "${CERT_CRT_PATH}" && -f "${CERT_KEY_PATH}" ]]
}

function generate_certs() {
  dotnet dev-certs https --export-path "${CERT_PFX_PATH}" --password "${CERT_PASSWORD}"

  openssl pkcs12 -in "${CERT_PFX_PATH}" \
    -nocerts \
    -nodes \
    -out "${CERT_KEY_PATH}" \
    -password pass:"${CERT_PASSWORD}" \
    -passin pass:"${CERT_PASSWORD}"

  openssl pkcs12 -in "${CERT_PFX_PATH}" \
    -clcerts \
    -nokeys \
    -out "${CERT_CRT_PATH}" \
    -password pass:"${CERT_PASSWORD}" \
    -passin pass:"${CERT_PASSWORD}" \
    -passout pass:"${CERT_PASSWORD}"
}

function main() {
  if certs_already_exist; then
    echo "Dev certs already exist, skipping generation."
    exit 0
  fi

  generate_certs
}

main
