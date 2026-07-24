# Plan: One-command local/CI startup via a new docker-compose file

Acceptance criteria (agreed 2026-07-24):

1. Given a clean checkout with only Docker installed (Mac or Linux, no manual
   mosquitto/cert setup), when a developer runs `yarn start`, then MQTT,
   web_host, zigbee_host, and site_host all come up successfully as
   containers with no separately-started dependency required.
2. Given the new compose is running, when a browser hits the site's HTTPS
   port, then the site loads and can reach web_host's API — matching
   current `yarn start` behavior. (Revised after self-review: the ports
   below are non-standard — see "Port scheme" — and the plain-HTTP port
   is not a usable browser entry point, only a redirect target; the
   canonical entry point is HTTPS.)
3. Given the new compose is running, when web_host/zigbee_host start, they
   connect to the haus_mqtt container without requiring a pre-existing
   external broker.
4. Given CI runs `run-unit-tests.sh` then `run-acceptance-tests.sh`
   unchanged (no edits to `main.yaml`), then acceptance tests boot the full
   stack via this new compose instead of raw `dotnet run`, and
   `setup-machine`'s standalone MQTT-broker step is no longer needed.
5. `docker-compose.yml` (the production/deployment file) and
   `scripts/linux-install.sh` are untouched.
6. Cert generation for haus_site's nginx (`cert.crt`/`cert.key`) is
   automated as part of the new compose's startup path, reusing the same
   `dotnet dev-certs` + `openssl` approach as `linux-install.sh`, scoped to
   Mac/Linux only.

Note on TDD shape: this is entirely infrastructure/config (docker-compose
YAML, shell scripts, package.json, a GitHub composite action) with no
unit-testable business logic — there is no meaningful red before green.
Each increment's "test" is actually running the relevant command and
observing real behavior (containers start, ports respond, files appear),
per `verification`'s standard of running it for real rather than reasoning
about it.

Design decisions locked in during intake:
- New file `docker-compose.local.yml`, separate from `docker-compose.yml`.
- `haus_mqtt`: same mosquitto image/config as the production compose.
- `haus_web`, `haus_zigbee`: built via the existing generic
  `haus-dockerfile` (same one already used to build these two for
  production) — HTTP only, dev ports (5000 for web), no certs.
- `haus_site`: built via the existing `haus-site-dockerfile` (nginx),
  matching production exactly — needs `cert.crt`/`cert.key` present.
- No real Zigbee hardware / `zigbee2mqtt` bridge service — matches today,
  where acceptance tests simulate devices by publishing directly to MQTT.
- `yarn start` itself gets rewired to boot the new compose (not a
  separate `start:docker` script), so CI's existing calls into
  `run-unit-tests.sh`/`run-acceptance-tests.sh` pick it up with zero
  changes to `main.yaml`. `yarn start:watch` and the individual
  `web_host:start`/`site_host:start`/`zigbee_host:start` scripts are
  untouched (still raw `dotnet run`, for fast local iteration outside
  Docker).

Known compatibility detail to resolve during increment 3: the current
`web_host:wait` script does
`wait-on tcp:5000 && wait-on $npm_package_config_web_host/haus_acceptance.db`,
checking for the SQLite file's existence on the *host* filesystem as a
migrations-complete signal. Once web_host runs in a container, that file
lands in the container's filesystem, not the host's, unless bind-mounted.
Resolve by either bind-mounting the container's data directory back to the
same host path the script already checks, or simplifying the wait to
rely on `tcp:5000` alone if migrations are confirmed to complete before
Kestrel binds (observed to be the case in this app's startup ordering).

## Port scheme (added mid-implementation)

This dev machine already runs the real production stack (`docker-compose.yml`)
on the standard ports (5000/5001 web, 1883/8883 mqtt). `docker-compose.local.yml`
uses non-standard ports throughout to avoid colliding with it:

- `haus_mqtt`: host `11883`/`18883` → container `1883`/`8883`.
- `haus_web`: host `15000` → container `80` (HTTP only).
- `haus_site`: host `15003` → container `443` (HTTPS only — see below).

All wait-on targets, the acceptance test suite's `BaseURL`/MQTT connection
string, and the Site.Host WASM client's `appsettings.Acceptance.json`
`Api.BaseUrl` were updated to match. Container-internal env vars
(`Mqtt__Server=mqtt://haus_mqtt:1883` etc.) are unaffected by this, since
docker's internal network uses the container's real listening port, not
the host-side republish.

`haus_site` only publishes its HTTPS port to the host. nginx's `:80` listener
(added by the nginx.conf fix, see below) still exists inside the container
for completeness, but since HTTP and HTTPS map to *different*, non-standard
host ports, an HTTP→HTTPS redirect using `$http_host` cannot correctly target
the *host's* HTTPS port from inside the container (it has no way to know the
external port mapping). Publishing the `:80` port anyway would advertise a
dead end — a client that reaches it always gets redirected back to itself.
So it's deliberately not published to the host; the canonical, working entry
point for both the acceptance-test suite and manual browsing is the HTTPS
port directly.

## Bugs found and fixed while getting this to actually verify

Three pre-existing bugs, shared with the production deployment, blocked
this feature from working and were fixed as part of it (not new
functionality, so no acceptance criterion of their own, but load-bearing
for criteria 1/2):

- `generate-dev-certs.sh`'s openssl-derived `cert.key` was passphrase
  protected, which nginx can't load non-interactively. Fixed with `-nodes`.
- `nginx.conf`'s `location /` proxied every request to the container's own
  HTTPS-only port with plain HTTP, which nginx always rejects. Replaced
  with a `:80 → :443` redirect and a `:443` server block serving content
  directly (no self-proxy).
- `haus-site-dockerfile` copied the whole publish output instead of just
  `wwwroot/`, so nginx's `root` never actually contained `index.html`
  (403). Fixed to `COPY $PUBLISH_DIR/wwwroot/ /var/www/web`.

A fourth bug, specific to this feature (not shared with production, since
production doesn't run the WASM client against a locally-built web_host),
was found during self-review: `appsettings.Acceptance.json` (the Blazor
WASM client's runtime config, fetched by the browser) hardcoded
`Api.BaseUrl` to `http://localhost:5000` — the *production* web_host's
port. A browser against the local stack would have silently issued API
calls against the live production instance. Fixed to `http://localhost:15000`.

Self-review also flagged that `package.json`'s `start` script used
`docker compose` (v2, space-separated) syntax, which is not installed in
this dev sandbox (only legacy `docker-compose` v1) and wasn't confirmed
available on the actual GitHub Actions runner either. Fixed by routing
through `scripts/docker-compose-local.sh`, which tries `docker compose`
first and falls back to `docker-compose`.

## Increments

1. **[independent]** Add `scripts/generate-dev-certs.sh`: generates
   `cert.pfx`/`cert.crt`/`cert.key` at the repo root if they don't already
   exist, reusing the same `dotnet dev-certs https --export-path` +
   `openssl pkcs12` split approach as `scripts/linux-install.sh`'s
   `generate_https_cert` (Mac/Linux only; no `sudo`/system-CA-trust step
   needed since these are just files consumed by containers, not
   installed as OS-trusted CAs).
   - files: new `scripts/generate-dev-certs.sh`
   - criteria: 6
   - verify: run the script twice — certs created on first run, second
     run is a no-op (idempotent, doesn't regenerate/clobber).

2. **[independent]** Add `docker-compose.local.yml` defining `haus_mqtt`
   (mosquitto, mirrors the production compose), `haus_web` and
   `haus_zigbee` (built from `haus-dockerfile`, HTTP-only dev ports,
   `Mqtt__Server` pointing at `haus_mqtt`), and `haus_site` (built from
   `haus-site-dockerfile`, nginx, cert bind mounts). See "Port scheme"
   above for the actual (non-standard) ports used.
   - files: new `docker-compose.local.yml`
   - criteria: 1, 2, 3, 5
   - verify: after running `scripts/publish-app.sh` and
     `scripts/generate-dev-certs.sh` once manually, run
     `./scripts/docker-compose-local.sh up --build`; confirm all 4
     containers start, web_host's health check reports the MQTT
     connection healthy, `curl -k https://localhost:15003` returns the
     site's `index.html`, and web_host responds on port 15000.

3. **[depends: 1, 2]** Rewire yarn's `start` script (and add a `prestart`
   hook, reusing today's orphaned `prestart:docker`/`publish-app.sh`
   pairing renamed to match) to run cert generation + publish + the new
   compose, replacing the current `concurrently ... npm:*:start`
   definition. Resolve the `web_host:wait` compatibility detail noted
   above. Leave `start:watch` and the individual `*_host:start` scripts
   untouched.
   - files: `package.json`
   - criteria: 1, 4
   - verify: `yarn start` from a clean checkout (no manual pre-steps)
     boots the full stack; `yarn acceptance` (which wraps `yarn start` +
     `yarn wait` + the test runner) completes its wait step without
     hanging and reaches the actual test run.

4. **[depends: 3]** Remove the now-redundant "Start MQTT Server" step from
   `.github/actions/setup-machine/action.yaml`, since MQTT now starts as
   part of the compose triggered via `yarn start`/`yarn acceptance`.
   - files: `.github/actions/setup-machine/action.yaml`
   - criteria: 4
   - verify: push and observe a CI run — unit tests step unaffected,
     acceptance tests step still brings up MQTT successfully (visible in
     the compose's own logs), and the separate mosquitto-github-action
     step no longer appears in the CI log.

5. **[depends: 3]** Update `CLAUDE.md` / `readme.md` to remove the
   instruction to manually install/start mosquitto for local dev, since
   it's no longer a manual prerequisite.
   - files: `CLAUDE.md`, `readme.md`
   - criteria: 1
   - verify: read the updated docs; confirm they no longer instruct a
     manual mosquitto install/start step for local dev.

Increments 4 and 5 are mutually independent of each other (disjoint
files) once 3 lands, but both depend on 3 existing first.
