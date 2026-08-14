# Plan: restore Docker Hub publish + harden deb-package smoke test

## Acceptance criteria

**`scripts/publish-to-docker-hub.sh`**
- Given `DOCKER_HUB_USERNAME`/`DOCKER_HUB_ACCESS_TOKEN` are set, when the script runs, then it performs a real
  `docker login` before building, and a real `docker push --all-tags "${DOCKER_HUB_USERNAME}/${DOCKER_HUB_REPO}"`
  after building. The `echo "skipped..."` no-ops and commented-out real commands are gone.
- The three image builds (haus-web, haus-zigbee, haus-site) are unchanged.
- Login/push failures propagate as a non-zero exit.

**`scripts/build-deb-package.sh` (`install_and_smoke_test`)**
- Before installing/starting `haus-app.service`, the just-built local `haus-*` image tags are invalidated
  (`docker rmi`) or an explicit `docker compose pull` is forced, so `docker compose up -d --remove-orphans`
  cannot succeed off the local build cache alone.
- If the registry push was skipped/broken (images unavailable remotely), the smoke test fails.
- If the registry push succeeded, the smoke test still passes as before.

Out of scope: Docker Hub account/repo visibility settings.

## Test approach

No shell test framework exists in this repo (no bats/shellspec) and none is being introduced. Tests are plain
bash scripts under `scripts/tests/` that stub out `docker` (and other externals) via a temp directory prepended
to `PATH`, source the real script with its `main` guarded out, invoke the function under test, and assert on a
call-log file the stub writes to. This proves call order/arguments without needing real Docker Hub credentials
or touching the host.

## Increments

1. `[independent]` Restore `docker login` before the image builds.
   - criteria: real login runs before build step, using `DOCKER_HUB_USERNAME`/`DOCKER_HUB_ACCESS_TOKEN`.
   - files: `scripts/publish-to-docker-hub.sh`, `scripts/tests/test-publish-to-docker-hub.sh` (new)

2. `[depends: 1]` Restore `docker push --all-tags` after the builds and wire the call back into `main()`.
   - criteria: real push runs after builds; a failing login or push exits non-zero.
   - files: `scripts/publish-to-docker-hub.sh`, `scripts/tests/test-publish-to-docker-hub.sh`
   - depends on 1: same file, same test suite — sequenced to avoid a collision.

3. `[independent]` Harden `install_and_smoke_test` to prove a real registry pull.
   - criteria: local `haus-*` tags are removed before install, in the right order relative to the smoke test.
     The unit tests only cover that `remove_locally_built_images` untags the right images and runs before
     `install_and_smoke_test` — they use a stub `docker` and so cannot themselves prove the gate discriminates
     a broken push from a working one. That is a separate, still-open verification step (see below).
   - files: `scripts/build-deb-package.sh`, `scripts/tests/test-build-deb-package.sh` (new)
   - independent of 1/2: disjoint files.

## Verification (beyond the stub-docker unit tests)

- `make docker-publish` against real (or the user's own) Docker Hub credentials, confirming a real
  `docker login` + `docker push --all-tags` occur — checked via `docker manifest inspect` or equivalent, not
  just "the script didn't error".
- Prove the deb-package gate's new discriminating power without touching the real host: use the existing
  nested sandbox (`scripts/deb-verify/`, `scripts/verify-deb-package-locally.sh`) or an equivalent isolated
  container, once with the push step skipped/broken (gate must fail) and once with it working (gate must pass).
  Never run `install_and_smoke_test`'s real `sudo apt-get install` against the actual dev machine — that
  installs a real systemd unit + containers on a persistent box, which is exactly what
  `scripts/deb-verify/Dockerfile`'s design note says to avoid outside disposable CI runners.
