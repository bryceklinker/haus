#!/usr/bin/env bash
set -ex

# Usage: ./scripts/generate-release-notes.sh [tag]
#
# With no argument, previews notes for the commits made since the last tag
# (handy for a developer checking what a release would say before cutting
# one). With a tag argument, labels those same pending commits with that
# tag name -- used by the release pipeline, which generates notes for the
# version scripts/compute-next-version.sh just computed before that tag
# actually exists (see docs/craft/design/2026-08-14-gitcliff-release-notes-pipeline.md).
TAG="${1:-}"

if [ -n "${TAG}" ]; then
  git-cliff --config cliff.toml --tag "${TAG}" --unreleased
else
  git-cliff --config cliff.toml --unreleased
fi
