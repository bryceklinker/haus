# Plan: Ubuntu `.deb` package (issue #19)

Source: `docs/craft/plans/2026-08-14-ubuntu-package-intake.md` (criteria),
`docs/craft/design/2026-08-14-ubuntu-deb-package-pipeline.md` (pipeline fit).

Directory layout decision (refines intake, same spirit): everything that's
*config* (compose file, mosquitto.conf, generated certs) lives under
`/etc/haus`, wiped on purge; everything that's *state* (SQLite db, zigbee
state, mosquitto data/log) lives under `/var/lib/haus/data`, never wiped by
any maintainer script. This keeps `docker-compose.yml`'s existing relative
volume paths (`./haus_web`, `./haus_mqtt/data`, ...) intact, just rooted at
`/etc/haus` via the unit's `WorkingDirectory`, matching how
`linux-install.sh` already roots everything under one directory
(`/home/$(whoami)/haus`) today — same shape, generic location.

1. **[independent]** Static packaging tree: `packaging/deb/DEBIAN/control`
   (name, version placeholder, `Depends: docker.io | docker-ce,
   docker-compose-plugin | docker-compose`), `packaging/deb/etc/systemd/system/haus-app.service`
   (adapted from the current `haus-app.service`: generic
   `WorkingDirectory=/etc/haus`, `docker compose` plugin form instead of the
   legacy hyphenated binary), `packaging/deb/etc/haus/mosquitto.conf`,
   `packaging/deb/etc/haus/docker-compose.yml.template` (current
   `docker-compose.yml` with `haus-web-latest`/`haus-zigbee-latest`/`haus-site-latest`
   replaced by version placeholders, volume paths unchanged since they're
   already relative).
   criteria: 5, 6, 8 · files: `packaging/deb/**`

2. **[independent]** Version-pinning generator, under `strict-tdd`: a small
   command in `src/Haus.Utilities` (new `Packaging` feature folder,
   following the existing `TypeScript`/`Zigbee2Mqtt` CLI-command precedent
   already documented in `CLAUDE.md`) that reads the compose template and a
   `VERSION` value and emits the pinned `docker-compose.yml`. Test cases:
   each `haus-<service>-latest` tag becomes `haus-<service>-<version>`;
   unrelated image refs (`eclipse-mosquitto:latest`) are left untouched;
   empty/missing version exits non-zero with a clear message instead of
   silently emitting a broken file.
   criteria: 5 · files: `src/Haus.Utilities/Packaging/**`,
   `tests/Haus.Utilities.Tests/Packaging/**`

3. **[depends: 1]** Maintainer scripts — `packaging/deb/DEBIAN/postinst`,
   `prerm`, `postrm`. postinst: create `/etc/haus` + `/var/lib/haus/data`
   tree, generate a self-signed cert via `openssl` only (no `dotnet
   dev-certs`) if one isn't already present, `systemctl daemon-reload &&
   enable && start`. prerm: stop + disable. postrm: on purge, remove
   `/etc/haus` only — `/var/lib/haus/data` is never touched by any
   maintainer script. No unit-test framework for shell exists in this repo
   (matches the `docker-compose-local-dev` precedent); correctness is
   proven end to end in increment 5, since these scripts only mean anything
   run by `dpkg` against a real system.
   criteria: 1, 2, 3, 4 · files: `packaging/deb/DEBIAN/{postinst,prerm,postrm}`

4. **[depends: 1, 2]** `scripts/build-deb-package.sh` + `make deb-package`:
   stage the packaging tree into a build dir, invoke increment 2's
   generator to produce the final `docker-compose.yml`, substitute
   `VERSION` into `DEBIAN/control`, run `dpkg-deb --build`, emit
   `publish/installer/haus-app_<version>_amd64.deb`. Scoped to "produces a
   structurally valid `.deb`" — verifiable via `dpkg-deb --info` /
   `dpkg --contents` without installing anything yet.
   criteria: 5, 7 (partial) · files: `scripts/build-deb-package.sh`, `Makefile`

5. **[depends: 3, 4]** Install smoke-test gate, appended to
   `scripts/build-deb-package.sh`: `apt install` the built `.deb`, wait for
   `systemctl is-active haus-app` and all containers up, hit `haus_web`'s
   health endpoint, then `apt purge`. This *is* the pipeline design's gate
   — no separate staging environment exists to promote through, so this
   step substitutes for one. Verified by actually running it, per
   `verification`'s standard.
   criteria: 1, 3, 4, 6 · files: `scripts/build-deb-package.sh`

6. **[depends: 4, 5]** Wire into `.github/workflows/release.yaml`: add the
   `make deb-package` step immediately after the existing
   `make docker-publish` step (needs the version-tagged images to already
   be pushed), upload the resulting `.deb` as a second release asset next
   to `service_package.zip`, reusing `tag_version.outputs.new_tag` — no new
   versioning scheme.
   criteria: 7 · files: `.github/workflows/release.yaml`

7. **[independent]** Docs: `readme.md` install instructions updated to lead
   with `sudo apt install ./haus-app_<version>_amd64.deb`;
   `scripts/linux-install.sh` gets a superseded/deprecation header comment
   pointing at the new package, no behavior change.
   criteria: 9, 10 · files: `readme.md`, `scripts/linux-install.sh`

Independence summary: 1, 2, 7 touch disjoint files and have no ordering
constraint on each other. 3 needs 1's tree shape to exist first. 4 needs
both 1 (tree) and 2 (generator). 5 needs 3 (scripts to actually exercise)
and 4 (a `.deb` to install). 6 needs 4 and 5 (won't wire a step into CI
that isn't gated yet). Given the tight coupling from 3 onward and the
modest total size, implementing sequentially in one pass is simpler than
dispatching parallel subagents here — the parallelism is noted for
completeness, not exercised.
