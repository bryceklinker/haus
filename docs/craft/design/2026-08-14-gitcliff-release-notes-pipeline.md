# Pipeline design: git-cliff release notes + deprecated-action cleanup in release.yaml

Targeted change to an existing pipeline (`.github/workflows/release.yaml`) —
replaces three actions the real run
https://github.com/bryceklinker/haus/actions/runs/31835249891 (job
94880010990) flagged as forcing deprecated Node 20 / `set-output`: `actions/
create-release@v1`, `actions/upload-release-asset@v1` (both archived), and
`mathieudutour/github-tag-action@v6.2`. Adds git-cliff-generated release
notes as a second, independent goal riding the same steps.

## Artifact strategy

No new artifact. `release-notes.md` is a build-time intermediate (Release
step body input), not a published asset — it's regenerated every run from
git history, so keeping it around after the job ends has no value. The two
existing release assets (`service_package.zip`, the `.deb`) are unchanged;
only the action that uploads them changes.

## Version bump: manual input stays authoritative, no auto-bump from history

`mathieudutour/github-tag-action` is removed outright rather than replaced
1:1. Its `default_bump` was always a fallback to a workflow input humans
already set explicitly (`version_bump: patch|minor|major`,
`workflow_dispatch`) — its conventional-commit auto-detection added a second,
usually-redundant path to the same decision.

*Why not have git-cliff compute the bump instead* (`--bumped-version`,
raised as an option): this repo's commit history is not yet conventional —
`git log --oneline` shows `Add Ubuntu Package`, `Rename haus_sit compose
service key`, `Bump Microsoft.NET.Test.Sdk` — so an auto-detected bump would
be a guess dressed as a computation until the commit-message convention
(tracked separately) lands and has actually been followed for a while.
Keeping the explicit `version_bump` input is simpler and honest about who's
deciding: a human, at `workflow_dispatch` time, same as today.

The replacement is a small pure function — `SemVerBumper` in
`Haus.Utilities` (mirrors the existing `DebComposeVersionPinner` /
`packaging render-deb-compose` CLI-command pattern already in this project)
— given the latest `vX.Y.Z` tag and a bump kind, returns the next tag.
`scripts/compute-next-version.sh` finds the latest tag via
`git tag --list 'v*' --sort=-v:refname` and invokes it. Pure, deterministic,
easy to unit test — unlike the third-party action it replaces, which needed
network calls and repo write access to test at all.

## Release notes: git-cliff, generated the same way locally and in CI

`cliff.toml` (repo root) configures `conventional_commits = true` but
`filter_unconventional = false`: strict conventional-commit filtering would
silently drop nearly this repo's entire history today. `commit_parsers`
matches both real conventional prefixes (`feat`, `fix`, `docs`, ...) *and*
this repo's existing capitalized-imperative style (`Add`, `Fix`, `Rename`,
`Bump`, ...), so notes are useful now and get more precise for free once the
commit-message convention work lands — conventional-format commits already
match the same parsers.

`scripts/generate-release-notes.sh` wraps the `git-cliff` invocation
(`--config cliff.toml --unreleased`, or `--tag <tag> --unreleased` when a
tag argument is given) so the exact same command runs from a developer's
machine (`make release-notes`) and from CI — no CI-only templating logic
to drift out of sync with what a human sees locally. `--unreleased` is
used even in CI: the tag doesn't exist yet at notes-generation time (see
stage ordering below), so `--tag <computed-tag> --unreleased` asks
git-cliff to label the pending commits with that name without requiring
the tag object to already exist. `cliff.toml`'s `body` template
deliberately omits the `## [version] - date` heading git-cliff's default
template renders — GitHub's release page already shows the tag/name as
its own title, so that heading would just be a duplicate directly above
the categorized list.

git-cliff itself isn't vendored or installed as a prerequisite for `make
release-notes` — like `dotnet`/`docker`, it's assumed present locally
(README documents `cargo install git-cliff` / `brew install git-cliff`). In
CI it's installed via `taiki-e/install-action` (`tool: git-cliff`) — a
composite (pure shell) action with no Node.js runtime at all, so it can't
regress into the same deprecation warning this change is fixing.

## Stage ordering

```
checkout → setup-machine → compute-next-version → make publish →
make docker-publish → make deb-package → generate-release-notes →
create_release (tag + notes + both assets)
```

*Why compute-next-version replaces bump-tag in the same slot:* every later
step already depends on `VERSION` being known (image tags, `.deb` filename,
compose pinning) — same dependency the old step satisfied, just without
pushing a tag as a side effect.

*Why generate-release-notes moves to just before create_release, not
earlier:* it only needs git history, not the built artifacts, but placing
it right before the step that consumes its output (`release-notes.md` as
`body_path`) keeps the two adjacent and avoids the notes going stale
relative to any commits landing on `main` while a long docker-publish/
deb-package run is in flight (unlikely in a single job, but free to avoid).

*Why the git tag is created by the Release step now, not a separate push:*
`softprops/action-gh-release@v3` creates the tag itself if `tag_name`
doesn't already exist on the remote. Folding tag-push into the same
atomic action that creates the release removes a step and a window where a
tag could exist with no corresponding release (e.g. if `create_release`
failed after a manual push) — mirrors the existing gate philosophy in
`2026-08-14-ubuntu-deb-package-pipeline.md`: nothing user-visible
(tag or release) should exist for a build that didn't make it all the way
through.

## The action replacements

- `actions/create-release@v1` + 2x `actions/upload-release-asset@v1`
  (archived, `node20`) → single `softprops/action-gh-release@v3`
  (`node24`, actively maintained, ~last release 2026-07). One step creates
  the release, tags it, and uploads both assets via `files:`.
- `mathieudutour/github-tag-action@v6.2` (`node20`, and its `set-output`
  usage was the source of the 5x deprecation warnings) → removed; replaced
  by the `SemVerBumper`/`compute-next-version.sh` pair above, which use
  `$GITHUB_OUTPUT` (the current, non-deprecated mechanism) directly in the
  workflow step, not inside a third-party action.
- Added `permissions: contents: write` at job level: `softprops/
  action-gh-release` needs to create the tag and release; made explicit
  rather than relying on the default token's implicit permissions, which
  are read-only on repos with restrictive defaults.

## Evidence-of-done

- `git-cliff --config cliff.toml --unreleased` run against this repo's
  real history, output inspected for sane grouping.
- `dotnet test tests/Haus.Utilities.Tests --filter FullyQualifiedName~SemVerBumper`
  green.
- `actionlint .github/workflows/release.yaml` (or reasoning through each
  replaced action's `runs.using`) shows no Node 20 / `set-output` warnings.
