# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

HAUS is a home automation system that runs on the user's personal network (only auth touches the cloud, via Auth0). It targets Zigbee devices via Zigbee2Mqtt/MQTT. The system is composed of several .NET services plus a Blazor site, orchestrated with a Makefile for local dev and Docker Compose for deployment.

## System requirements

- .NET (SDK pinned in `global.json`, currently 10.0.100, `rollForward: latestMinor`)
- Docker (used by `make start` to run the full stack via `docker-compose.local.yml`, including the MQTT broker — no separate MQTT install needed)

Required environment variables for auth (both plain and `CYPRESS_`-prefixed variants are needed since acceptance tests use Cypress-style env vars): `GITHUB_TOKEN`, `AUTH_DOMAIN`, `AUTH_CLIENT_ID`, `AUTH_CLIENT_SECRET`, `AUTH_AUDIENCE`, `AUTH_USERNAME`, `AUTH_PASSWORD`. See `readme.md` for the full export block.

## Common commands

Run from the repo root (Make targets `cd` into the relevant project as needed; see the `Makefile`).

```bash
make start                   # publish, generate dev certs, then boot the full stack via docker-compose.local.yml
make watch                   # run web host + site host directly via dotnet watch (hot reload, no Docker)
make zigbee-host             # run only the Zigbee host
make web-host                # run only the API host (launch profile: acceptance)
make site-host               # run only the Blazor site host (launch profile: acceptance)

dotnet build                  # build the whole solution (Haus.slnx)
dotnet tool restore           # restore local tools (csharpier, dotnet-ef, coverlet, reportgenerator, husky)

make test-unit                # runs all *.Tests projects via coverlet + generates HTML coverage report
make test-acceptance          # boots web+site hosts via docker compose --wait, runs Haus.Acceptance.Tests, tears the stack down

dotnet test tests/Haus.Core.Tests --no-build              # run a single test project
dotnet test tests/Haus.Core.Tests --filter FullyQualifiedName~SomeTestName  # run a single test
```

Coverage/build variables (VERSION, CONFIGURATION, publish/coverage paths) live in `scripts/variables.sh`, sourced by the other scripts.

### Formatting / linting

CSharpier + `dotnet format` run automatically on staged `*.cs` files via Husky.Net (`.husky/pre-commit` → `dotnet husky run --group pre-commit`, tasks defined in `.husky/task-runner.json`). CSharpier config is in `.csharpierrc.json` (4-space indent, 120 col width). Don't bypass this hook.

### Commit messages

Commit messages must follow [Conventional Commits](https://www.conventionalcommits.org/) (`type(scope): description`) — see `CONTRIBUTING.md` for the allowed types and why (it drives git-cliff release notes). Enforced by a Husky.Net `commit-msg` hook (`.husky/commit-msg` → `dotnet husky run --group commit-msg`), backed by `Haus.Utilities`' `git validate-commit-message` CLI command (`src/Haus.Utilities/Git`).

### EF Core migrations

```bash
./scripts/create-ef-migration.sh <MigrationName>
```
Wraps `dotnet ef migrations add`, using `Haus.Web.Host` as the startup project and `Haus.Core` as the context project (`HausDbContext`), writing output to `Common/Storage/Migrations`.

### Adding a new project

```bash
./scripts/add-project.sh <dotnet-new-template> <ProjectName>
```
Creates the project under `src/` (or `tests/` for `test` type) and adds it to `Haus.slnx`.

## Architecture

### Service topology

- **Haus.Zigbee.Host** — connects to Zigbee2Mqtt over MQTT, translates Zigbee2Mqtt messages to/from Haus domain messages.
- **Haus.Web.Host** — the API host: ASP.NET Core (Kestrel), owns the SQLite-backed domain data (EF Core via `HausDbContext`), exposes REST + SignalR, talks to MQTT, and hosts the CQRS bus (`IHausBus`).
- **Haus.Site.Host** — Blazor front end (`App.razor` root), consumes the Web Host API (via `Haus.Api.Client`) and SignalR for realtime updates; served behind nginx in Docker (`nginx.conf`, `haus-site-dockerfile`).
- **haus_mqtt** — mosquitto broker connecting all services (see `docker-compose.yml`, `mosquitto.conf`).

In Docker Compose, `haus_zigbee` and `haus_web` both connect to `haus_mqtt`; `haus_site` (nginx) serves the published Blazor bundle directly — the browser calls `haus_web`'s API itself, nginx does not proxy to it. `docker-compose.yml` is the production deployment file; `docker-compose.local.yml` is a separate compose file (non-standard ports) for one-command local dev/CI startup, building all three .NET services from local source via the same Dockerfiles.

### CQRS core (`Haus.Cqrs`)

A thin, dependency-free CQRS/mediator layer with its own reflection-based handler discovery and dispatch (`HandlerInvoker`, `ServiceCollectionExtensions.AddHausCqrs`) — no third-party mediator library. It's exposed through a single facade `IHausBus` (`Haus.Cqrs/HausBus.cs`) with four operations: `ExecuteCommandAsync`, `ExecuteQueryAsync`, `PublishAsync` (events), and `Enqueue`/`FlushAsync` (domain events, inherited from `IDomainEventBus`). Each of Commands/Queries/Events/DomainEvents has its own bus + logging decorator (`LoggingCommandBus`, `LoggingQueryBus`, etc., all built on the shared `LoggingBus` base which times execution and logs start/finish/error). Consumers should only ever depend on `IHausBus`, not the individual sub-buses. Commands and queries require exactly one registered handler (throws if zero or multiple); events and domain events dispatch to zero-to-many handlers.

### Domain layer (`Haus.Core`)

Organized by feature/bounded-context folder, not by technical layer: `Devices`, `Rooms`, `DeviceSimulator`, `Discovery`, `Health`, `Logs`, `Lighting`, `Diagnostics`, `Application`. Within each feature folder, the convention is technical subfolders: `Commands`, `Queries`, `Entities`, `Repositories`, `Validators`, `Events` (external/integration events), `DomainEvents` (internal side-effects). `Common` holds cross-cutting building blocks (`Common/Commands`, `Common/Queries`, `Common/Entities`, `Common/Events`, `Common/Storage` — including EF Core migrations). When adding a new feature, mirror this folder shape.

### Models & clients

- **Haus.Core.Models** — feature-mirrored shared DTOs/models (`Devices`, `Rooms`, `Lighting`, `Discovery`, `Health`, `Logs`, `ExternalMessages`, etc.) shared between the API host, the API client, and the site.
- **Haus.Api.Client** — typed client for the Web Host API, mirrors `Haus.Core.Models`' feature folders.
- **Haus.Mqtt.Client** — MQTT wrapper/abstraction used by both `Haus.Web.Host` and `Haus.Zigbee.Host`.
- **Haus.Udp.Client**, **Haus.Hosting**, **Haus.Utilities** — supporting infra. `Haus.Utilities` also hosts CLI tooling (`Common/Cli`) for TypeScript model generation (`TypeScript/GenerateModels`) and Zigbee2Mqtt default device-type option generation (`Zigbee2Mqtt/GenerateDefaultDeviceTypeOptions`).

### Tests

Test projects mirror `src/` 1:1 by name (e.g. `tests/Haus.Core.Tests` ↔ `src/Haus.Core`), with the same feature-folder layout inside. `Haus.Testing.Support` holds shared test infrastructure/fixtures. `Haus.Acceptance.Tests` is a separate end-to-end suite that runs against the full stack booted via `docker-compose.local.yml` (see `make test-acceptance`) — Cypress-style env vars (`CYPRESS_AUTH_*`) drive its Auth0 login.

## CI/CD

- **`.github/workflows/main.yaml`** — on push/PR to `main`: sets up the machine (`.github/actions/setup-machine`, which installs .NET, trusts dev certs), runs `make build`, `make test-unit` (which starts and tears down its own disposable MQTT broker on port 21883 for `Haus.Web.Host.Tests`'s real MQTT integration tests), then `make test-acceptance` (which separately boots the full stack, including its own MQTT broker on non-standard ports, via `docker-compose.local.yml`).
- **`.github/workflows/release.yaml`** — manual `workflow_dispatch`: bumps version/tag, publishes app artifacts (`make publish`), pushes Docker images (`make docker-publish`), and creates a GitHub release with the service package.
