# Overview

HAUS is a home automation system that works within your personal network. The only part of the application that requires
access to the cloud is for authentication of users. The current version is using Auth0 for authentication. However, this
may change in the future.

## Zigbee

Zigbee is an open protocol that various manufacturers are using in their "smart" devices. The current version is
targeting support for many Zigbee devices.

# System Requirements

- .NET 10
- Docker (used by `make start` to run the full stack, including the MQTT broker - no separate MQTT install needed)

Currently, this is only intended to be run from the command line with all code available locally.

# Installation (Ubuntu)

Each [release](https://github.com/bryceklinker/haus/releases) publishes a `haus-app_<version>_amd64.deb` package.
Download it, then:

```bash
sudo apt install ./haus-app_<version>_amd64.deb
```

This installs and enables a `haus-app` systemd service that runs HAUS via Docker Compose, using the
already-published `haus-web`/`haus-zigbee`/`haus-site` images pinned to that release's version (never `:latest`).
Docker and the Docker Compose plugin must already be installed on the host -- the package declares them as a
dependency but does not install them itself. A self-signed HTTPS cert is generated automatically on first install
if one doesn't already exist.

```bash
sudo systemctl status haus-app   # check status
sudo apt remove haus-app         # stop and remove, keeping your data
sudo apt purge haus-app          # also remove generated config/certs (data is still kept)
```

`scripts/linux-install.sh` (the previous manual install method) still works but is superseded by this package --
it gets no further changes.

# Commands

```bash
# Build Solution
make build

# Run Unit Tests
make test-unit

# Run Acceptance Tests
make test-acceptance

# Run build, unit tests, and acceptance tests
make verify
```

# Environment Variables

```bash
export GITHUB_TOKEN="{insert GitHub PAT w/ read access to repository}";

export AUTH_DOMAIN="{insert auth0 domain}"
export AUTH_CLIENT_ID="{insert auth0 client id}"
export AUTH_CLIENT_SECRET="{insert client secret for above client id}"
export AUTH_AUDIENCE="https://haus-portal-api.com"
export AUTH_USERNAME="{insert user name}"
export AUTH_PASSWORD="{insert user password"

export CYPRESS_AUTH_DOMAIN="${AUTH_DOMAIN}"
export CYPRESS_AUTH_CLIENT_ID="${AUTH_CLIENT_ID}"
export CYPRESS_AUTH_CLIENT_SECRET="${AUTH_CLIENT_SECRET}"
export CYPRESS_AUTH_AUDIENCE="${AUTH_AUDIENCE}"
export CYPRESS_AUTH_USERNAME="${AUTH_USERNAME}"
export CYPRESS_AUTH_PASSWORD="${AUTH_PASSWORD}"
```