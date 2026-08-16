#!/usr/bin/env bash
set -ex

# CURRENT_VERSION is intentionally re-derived from git tags here rather than
# trusted from the caller's environment: the release workflow's only source
# of truth for "what was the last release" is the tag history itself.
CURRENT_VERSION=$(git tag --list 'v*' --sort=-v:refname | head -n1)
export CURRENT_VERSION

# VERSION_BUMP is intentionally left unset when the caller didn't provide one --
# the .NET command treats an empty VERSION_BUMP as "compute it from the commit
# log since CURRENT_VERSION" rather than defaulting to patch here.

dotnet run --project src/Haus.Utilities -- release bump-version
