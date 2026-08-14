# Pipeline design: `.deb` package build/gate/publish in release.yaml

Targeted change to an existing pipeline (`.github/workflows/release.yaml`) —
going deep only on the areas the new `.deb` step touches; the rest is a
single not-implicated line at the end.

## Artifact strategy

One new immutable artifact per release: `haus-app_<version>_amd64.deb`,
where `<version>` is the same string `tag_version.outputs.new_tag` already
produces for the Docker image tags and the GitHub Release (git-tag-derived,
stripped of its leading `v` the same way `scripts/variables.sh` already does
for `VERSION`). No separate version scheme, no separate tag — one source of
truth for "what version is this release."

*Why:* the whole point of locking postinst to pin image tags instead of
`:latest` (intake criterion #5) collapses if the `.deb`'s version and the
image tags it references can drift apart. Reusing the existing tag output is
the only way to guarantee they can't.

The `.deb` is built once in this job and is the same file uploaded as the
release asset — no rebuild-for-a-different-target step exists or is needed,
since there's exactly one install target shape (`amd64` Ubuntu + systemd).

## Stage ordering

Inserted as **`make deb-package`, immediately after `make docker-publish`,
before `create_release`**:

```
checkout → setup-machine → bump-tag → make publish → make docker-publish → make deb-package → create_release → upload service_package.zip → upload haus-app_<version>_amd64.deb
```

*Why this position, not earlier:* the `.deb`'s bundled `docker-compose.yml`
pins `haus-web-<version>`, `haus-zigbee-<version>`, `haus-site-<version>`
image tags. The install-time smoke test (below) actually pulls those tags
from Docker Hub to prove the package works — so the images must already be
pushed. `docker-publish` is therefore a hard prerequisite, not just a
sibling step. Building the `.deb` before `docker-publish` would let a smoke
test either fail (image not found yet) or silently pull a stale previous
version, defeating the point of the pinning.

*Why before `create_release`, not after:* if the `.deb` fails to build or
fails its install smoke test, the release must not be created at all — a
GitHub Release with only a `service_package.zip` asset and no `.deb`, or a
`.deb` known to be broken, is worse than no release, since it's presented to
users as "this version is ready to install." Failing the job here leaves
the git tag pushed (already true today for any failure after `bump-tag`,
unchanged) but no GitHub Release, matching today's behavior for a failure
in `make publish`/`make docker-publish`.

## The gate

There is no staging environment to promote through
(`environments.order: [production]` in `.craft-ops.yml` — HAUS is
self-hosted software installed by an operator, not a service this pipeline
deploys to). The gate substitutes **install-in-place on the runner itself**
for a staging deploy:

- GitHub-hosted `ubuntu-22.04` runners are full VMs with systemd as PID 1
  and Docker pre-installed — i.e. the runner already *is* a disposable,
  systemd-capable, Docker-capable Ubuntu host, discarded at job end. No
  nested container/VM needs to be spun up to get a realistic install target;
  the runner IS one.
- `make deb-package` therefore doesn't just run `dpkg-deb --build` — it
  also: `sudo apt install -y ./haus-app_<version>_amd64.deb`, then polls
  `docker compose ps` per-container (in the package's install directory)
  for `haus_mqtt`/`haus_web`/`haus_sit` specifically, rather than waiting
  on the systemd unit's own active/failed verdict — verified during
  implementation that `haus-app.service` can legitimately end up "failed"
  (haus_zigbee needs a physical dongle a build runner won't have) while
  the other three containers are still up fine, so the per-container check
  is the real signal. It then hits haus-web's HTTPS port, and
  `sudo apt purge -y haus-app` to leave the runner clean (matters less
  since the runner is discarded, but keeps the script honest/idempotent for
  local use too) before the packaged file is uploaded.
- Any failure in that sequence — dpkg dependency resolution, postinst,
  service not reaching active, containers not coming up, health check not
  responding — fails the job hard, before `create_release` runs. This *is*
  the hard automated gate; there is no separate human promotion gate for
  this artifact, consistent with how `make docker-publish` and
  `make publish` are already ungated automated steps in this pipeline.

*Why install-and-smoke-test in CI rather than trusting `dpkg-deb --build`
alone:* a `.deb` that merely builds without error proves nothing about
whether postinst actually enables/starts the service or whether the
dependency declarations are satisfiable — exactly the class of bug this
issue exists to eliminate by moving off a hand-run script. Building without
installing would just move the manual-and-unverified problem into the
pipeline instead of removing it.

## Versioning / identity consistency

`make deb-package` receives `VERSION` the same way `make publish` and
`make docker-publish` already do — as an env var set from
`tag_version.outputs.new_tag` in the workflow step, consumed via
`scripts/variables.sh`'s existing `VERSION`/leading-`v`-strip logic. The
Debian `Version:` control field, the image tags baked into the shipped
`docker-compose.yml`, and the GitHub Release tag are thus all the same
value by construction, not by convention that could drift.

## Not implicated

Reproducibility seams, secrets & config boundary, and evidence of done for
the *production* install (as opposed to the CI smoke test above) are
unchanged by this addition: the `.deb` build needs no network access beyond
already-available local tooling (`dpkg-deb`, already on `ubuntu-22.04`
runners), bakes in no secrets (postinst pulls public Docker Hub images the
same way `docker-compose.yml`/`linux-install.sh` already do, unauthenticated),
and production evidence-of-done for an operator's actual install is a
deployment-design concern out of scope for this pipeline change (this repo
has no automated deploy step to a production target at all — the operator
runs the installer by hand, unchanged by this issue).
