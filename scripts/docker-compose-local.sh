#!/usr/bin/env bash
set -e

if docker compose version >/dev/null 2>&1; then
  docker compose -f docker-compose.local.yml "$@"
else
  docker-compose -f docker-compose.local.yml "$@"
fi
