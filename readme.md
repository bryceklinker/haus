# Overview

HAUS is a home automation system that works within your personal network. The only part of the application that requires
access to the cloud is for authentication of users. The current version is using Auth0 for authentication. However, this
may change in the future.

## Zigbee

Zigbee is an open protocol that various manufacturers are using in their "smart" devices. The current version is
targeting support for many Zigbee devices.

The Zigbee coordinator (e.g. a ConBee II dongle) is exposed to the `haus_zigbee` container via the
`ZIGBEE_SERIAL_PORT` environment variable, read by both `docker-compose.yml` and
`packaging/deb/etc/haus/docker-compose.yml.template` (default: `/dev/ttyACM0`). ttyACM/ttyUSB enumeration is not
stable across reboots/replugs, so if the dongle isn't at the default path, set `ZIGBEE_SERIAL_PORT` in a `.env` file
next to the compose file (`/etc/haus/.env` for the `.deb` install) -- docker compose loads it automatically. Prefer
the dongle's stable `/dev/serial/by-id/...` symlink over a raw `/dev/ttyACMx` node, since that path doesn't change
across replugs. On `.deb` installs, `postinst` auto-detects the dongle and writes `ZIGBEE_SERIAL_PORT` into
`/etc/haus/.env` at install/upgrade time when it isn't already set there -- it never overwrites a value you've set
manually.

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

If a host previously ran `scripts/linux-install.sh`, `postinst` migrates that install's data (SQLite db, Zigbee
state, MQTT broker state) from its `~/haus` layout into `/var/lib/haus/data` automatically on first `.deb` install
-- the original `~/haus` directory is only ever read, never modified or deleted, so it's safe to check afterward
and remove by hand once you've confirmed the migrated data looks right. This only happens once: a marker file at
`/var/lib/haus/.legacy-data-migrated` prevents re-running the migration (and clobbering newer data) on later
package upgrades.

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

# Preview release notes for the commits made since the last tag
# (requires git-cliff: https://git-cliff.org/docs/installation)
make release-notes
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
```