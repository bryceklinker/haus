# Intake: Ubuntu Package (issue #19)

## Problem

`scripts/linux-install.sh` is a manual, hand-run script: scp/copy a
`service_package.zip` release asset to the target box, run the script, which
generates HTTPS certs, unzips the package, copies `haus-app.service` into
`/etc/systemd/system`, then `docker-compose pull` + `systemctl enable/restart`.
There's no dependency tracking, no clean upgrade/removal path, and no
integration with the host's package manager. Issue #19 asks for a real Ubuntu
package so install/upgrade/removal go through standard tooling instead.

## Decision: `.deb` (dpkg/apt), not snap

Investigated snapd's confinement model against what this package actually
needs to do at install time: write `haus-app.service` into
`/etc/systemd/system`, write `docker-compose.yml`/`mosquitto.conf` into a host
config directory, invoke `docker compose pull` against the host Docker socket,
and run `systemctl daemon-reload/enable/start`.

- **Strict confinement** (snap's default and the only mode Canonical will list
  in the store without manual review) sandboxes the snap into its own mount
  namespace via AppArmor/seccomp. A strict snap cannot write to
  `/etc/systemd/system`, cannot manage a *separate*, non-snap systemd unit,
  and has no supported interface for driving the host's `docker-compose`
  against arbitrary host paths — `docker-support` exists for privileged
  *container* workloads, not for a snap acting as an installer/orchestrator
  of other host services. This rules strict confinement out entirely; it
  cannot do the job.
- **Classic confinement** removes the sandbox and gives full system access —
  functionally equivalent to a `.deb`'s postinst — but requires a manual
  Canonical review to publish to the store, and self-hosting a classic snap
  outside the store loses most of snap's distribution benefit anyway. It buys
  nothing over a `.deb` for this package while adding review/publishing
  friction.
- A **`.deb`**'s maintainer scripts (`postinst`/`prerm`/`postrm`) run as root
  with no confinement, so all of the above (`/etc/systemd/system`, Docker
  socket, `systemctl`) is direct and unremarkable — exactly what
  `linux-install.sh` already does by hand today, just wired through
  `dpkg`/`apt` lifecycle hooks instead of a person running a shell script.
  It also gives real `Depends:` dependency resolution against the Docker
  packages, and standard `apt remove`/`apt purge` semantics for removal.

**Decision: build a `.deb` only.** Snap adds no capability here and strict
confinement actively blocks the core job; classic confinement just reinvents
a `.deb` with extra publishing overhead.

## Scope boundary: distribution mechanism

True `sudo apt install haus-app` (install by name, no local file) requires a
hosted, GPG-signed APT repository (Packages/Release indices + hosting +
signing-key distribution) — this project has none of that infrastructure
today (see `.craft-ops.yml`: no cloud/IaC, no existing artifact hosting
besides Docker Hub and GitHub Releases).

**In scope for this issue:** produce a properly structured, versioned
`.deb` artifact (correct control metadata, dependency declarations, working
maintainer scripts) published as a GitHub Release asset alongside the
existing `service_package.zip`/Docker images, installed via
`sudo apt install ./haus-app_<version>_amd64.deb` (apt still resolves
dependencies and registers the package normally — this is standard practice
for projects without a hosted repo).

**Out of scope for this issue:** standing up a hosted/signed APT repository
or PPA so `apt install haus-app` works by name with no local file. Flagged as
a reasonable, separate follow-up rather than silently redefined — it's
substantial new infrastructure (GPG key management, static index hosting,
key distribution to users) beyond what one packaging issue should carry.

## What the package does (postinst/prerm/postrm)

- **Depends:** `docker.io | docker-ce`, `docker-compose-plugin | docker-compose`
  — Docker itself is a prerequisite, not something this package installs;
  apt's dependency resolver enforces it at install time instead of the
  package silently failing later.
- **postinst:**
  - Create `/etc/haus` (config) and `/var/lib/haus/data` (persistent data,
    mirrors `linux-install.sh`'s `data/` dir).
  - Install `haus-app.service`, `docker-compose.yml`, `mosquitto.conf` into
    those locations. `docker-compose.yml` ships with each service image
    pinned to *this package's* release version tag (e.g. `haus-web-1.4.2`),
    not `:latest` — this is a deliberate improvement over
    `linux-install.sh`'s implicit "whatever `:latest` is right now" pull,
    making installs of a given package version reproducible.
  - Generate a self-signed HTTPS cert via `openssl req -x509 ...` if one
    doesn't already exist at the target path, *if* one isn't already present
    — reusing the openssl step from `linux-install.sh` but dropping its
    `dotnet dev-certs` step, which requires the .NET SDK on the host. A
    package that promises not to bundle (or require) .NET tooling on the
    target machine can't depend on the SDK being there just for cert
    generation; openssl alone reproduces an equivalent self-signed cert
    without that dependency.
  - `systemctl daemon-reload`, `enable`, `restart` the service (best-effort:
    `|| true` on the restart — see below). `docker compose up -d` pulls
    whatever image isn't already present locally; there's deliberately no
    separate `docker compose pull` step, since each release pins an
    immutable, version-tagged image (never `:latest`) that, once cached, is
    guaranteed correct — forcing a re-pull on every start would make
    startup fail on a flaky network or registry hiccup even when a
    perfectly good image is already local. This also keeps postinst from
    doing a long, network-dependent operation that dpkg policy discourages.
  - The restart is `|| true`, not fatal: `haus_zigbee` needs a physical
    Zigbee dongle at `/dev/ttyACM0`, so `docker compose up` can legitimately
    fail on a host where it isn't plugged in yet (verified locally — Docker
    Compose aborts with a non-zero exit when one service's device is
    missing, even though the other containers start fine). That must not
    fail `apt install` itself; the unit stays enabled for the next
    successful boot or manual restart once the dongle is attached.
- **prerm:** stop and disable the service before files are removed.
- **postrm:** on purge, remove `/etc/haus`; `/var/lib/haus/data` (user data)
  is left in place — same "don't delete user data" posture as removing any
  database-backed package.

## `scripts/linux-install.sh`: superseded, not removed

The `.deb` postinst reproduces every step `linux-install.sh` performs today,
idempotently, through dpkg's lifecycle instead of a hand-run script. It's
marked superseded (a comment header + README note) rather than deleted:
existing docs/tooling may still reference it, and removing a working script
outright isn't necessary to ship the new path. New CI/release wiring targets
the `.deb`; `linux-install.sh` gets no further investment.

## Acceptance criteria (agreed 2026-08-14)

1. Given a clean Ubuntu host with Docker + Compose plugin already installed,
   when an operator runs `sudo apt install ./haus-app_<version>_amd64.deb`,
   then `haus-app.service` is installed, enabled, and running, and
   `docker compose ps` (in the package's install directory) shows the three
   HAUS containers up.
2. Given the package is installed and no cert already exists at the
   configured path, then a self-signed HTTPS cert is generated via `openssl`
   only — no `dotnet`/.NET SDK dependency introduced by the package or its
   scripts.
3. Given the package is installed, when it's upgraded to a newer version
   (`sudo apt install ./haus-app_<newversion>_amd64.deb`), then the service
   is restarted against the new version's pinned image tags, and any
   existing cert and `/var/lib/haus/data` contents are preserved (not
   regenerated/wiped).
4. Given the package is installed, when it's removed (`sudo apt remove
   haus-app`), then the service is stopped and disabled and its systemd
   unit is gone, but `/var/lib/haus/data` still exists; when purged
   (`sudo apt purge haus-app`), `/etc/haus` is also removed.
5. `docker-compose.yml` shipped inside the package pins each service image
   to that package version's tag, not `:latest`.
6. Docker/Compose are a declared package dependency, not something the
   package installs itself; installing on a host without Docker fails at
   `apt install` dependency resolution, not partway through a script.
7. The release pipeline (`.github/workflows/release.yaml`) builds the `.deb`
   as part of the existing release job and attaches it to the GitHub Release
   as an asset, alongside the existing `service_package.zip` and Docker Hub
   image publish — using the version tag already computed by the existing
   `tag_version` step, no new tagging scheme.
8. `docker-compose.yml`/`haus-dockerfile`/`haus-site-dockerfile` container
   contents are unchanged; this issue only changes how the *host* is
   provisioned, not what runs inside the containers.
9. `scripts/linux-install.sh` remains present and behavior-unchanged, marked
   superseded (comment + README note pointing at the new package); it is not
   wired into the new CI/release path and gets no further feature work.
10. `readme.md` install instructions are updated to lead with
    `sudo apt install ./haus-app_<version>_amd64.deb` as the primary Ubuntu
    install path.

Snap packaging is explicitly not pursued for this issue, for the confinement
reasons above.

## Follow-up (2026-08-15): legacy data migration

The original acceptance criteria above cover a clean install and a `.deb`-to-`.deb`
upgrade preserving `/var/lib/haus/data`, but not the transition *from*
`scripts/linux-install.sh`'s manual install into the `.deb`. That install put
the SQLite db, Zigbee state, and MQTT broker state under
`/home/$(whoami)/haus/{haus_web,haus_zigbee,haus_mqtt}` (see
`scripts/linux-install.sh`'s `HAUS_LOCATION`); a host upgrading straight to
the `.deb` would silently start against the fresh, empty
`/var/lib/haus/data` tree `postinst` creates, losing every device/room/log
the operator had.

`postinst` now migrates that legacy data forward the first time it runs on a
host where it's found:

- `find_legacy_data_dir` locates the legacy `~/haus` directory: it prefers
  `$SUDO_USER`'s home (set by `sudo apt install`), then falls back to
  scanning every `/home/*/haus` and `/root/haus` for one containing
  `haus_web`, `haus_zigbee`, or `haus_mqtt`.
- `migrate_legacy_data` copies (`cp -a --update=none`, never moves) each of those
  subdirectories into the matching `/var/lib/haus/data/<subdir>`, then
  writes a marker file at `/var/lib/haus/.legacy-data-migrated`.
- Every later `postinst` run (package upgrades) checks that marker first and
  skips the migration entirely once it exists, so a legacy directory that's
  still sitting around can never overwrite newer `.deb`-produced data.
- The legacy directory itself is never modified or deleted -- the migration
  is purely additive, so a failed or unexpected copy can always be redone by
  hand from the untouched original.

Validated by sourcing `postinst`'s functions in a sandboxed fake filesystem
(see the PR for `polly/fix-deb-data-migration`) covering: resolving the
legacy dir via `$SUDO_USER`, a full first-time migration, a second run after
the marker exists not clobbering newer data, and a fresh install with no
legacy dir being a clean no-op.
