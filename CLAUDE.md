# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

HAUS is a home automation system that runs on the user's personal network (only auth touches the cloud, via Auth0). It targets Zigbee devices via Zigbee2Mqtt/MQTT. The system is composed of several .NET services plus a Blazor site, orchestrated with Yarn/concurrently for local dev and Docker Compose for deployment.

## System requirements

- .NET (SDK pinned in `global.json`, currently 9.0.200, `rollForward: latestMinor`)
- Node/Yarn (Node version pinned in `.nvmrc`)
- An MQTT broker (`brew install mosquitto` on macOS; `docker-compose up haus_mqtt` otherwise)

Required environment variables for auth (both plain and `CYPRESS_`-prefixed variants are needed since acceptance tests use Cypress-style env vars): `GITHUB_TOKEN`, `AUTH_DOMAIN`, `AUTH_CLIENT_ID`, `AUTH_CLIENT_SECRET`, `AUTH_AUDIENCE`, `AUTH_USERNAME`, `AUTH_PASSWORD`. See `readme.md` for the full export block.

## Common commands

Run from the repo root (Yarn scripts `cd` into the relevant project via `package.json` `config` paths).

```bash
yarn install                 # install JS deps (also runs first-time)
yarn start                   # run web host, site host, zigbee host together (concurrently)
yarn start:watch             # same, but with dotnet watch for hot reload
yarn zigbee_host:start       # run only the Zigbee host
yarn web_host:start          # run only the API host (launch profile: acceptance)
yarn site_host:start         # run only the Blazor site host (launch profile: acceptance)

dotnet build                 # build the whole solution (Haus.sln)
dotnet tool restore          # restore local tools (csharpier, dotnet-ef, coverlet, reportgenerator, husky)

./scripts/run-unit-tests.sh        # runs all *.Tests projects via coverlet + generates HTML coverage report
./scripts/run-acceptance-tests.sh  # yarn acceptance: boots web+site hosts, waits, runs Haus.Acceptance.Tests

dotnet test tests/Haus.Core.Tests --no-build              # run a single test project
dotnet test tests/Haus.Core.Tests --filter FullyQualifiedName~SomeTestName  # run a single test
```

Coverage/build variables (VERSION, CONFIGURATION, publish/coverage paths) live in `scripts/variables.sh`, sourced by the other scripts.

### Formatting / linting

CSharpier + `dotnet format` run automatically on staged `*.cs` files via Husky.Net (`.husky/pre-commit` → `dotnet husky run`, tasks defined in `.husky/task-runner.json`). CSharpier config is in `.csharpierrc.json` (4-space indent, 120 col width). Don't bypass this hook.

### EF Core migrations

```bash
./scripts/create-ef-migration.sh <MigrationName>
```
Wraps `dotnet ef migrations add`, using `Haus.Web.Host` as the startup project and `Haus.Core` as the context project (`HausDbContext`), writing output to `Common/Storage/Migrations`.

### Adding a new project

```bash
./scripts/add-project.sh <dotnet-new-template> <ProjectName>
```
Creates the project under `src/` (or `tests/` for `test` type) and adds it to `Haus.sln`.

## Architecture

### Service topology

- **Haus.Zigbee.Host** — connects to Zigbee2Mqtt over MQTT, translates Zigbee2Mqtt messages to/from Haus domain messages.
- **Haus.Web.Host** — the API host: ASP.NET Core (Kestrel), owns the SQLite-backed domain data (EF Core via `HausDbContext`), exposes REST + SignalR, talks to MQTT, and hosts the CQRS bus (`IHausBus`).
- **Haus.Site.Host** — Blazor front end (`App.razor` root), consumes the Web Host API (via `Haus.Api.Client`) and SignalR for realtime updates; served behind nginx in Docker (`nginx.conf`, `haus-site-dockerfile`).
- **haus_mqtt** — mosquitto broker connecting all services (see `docker-compose.yml`, `mosquitto.conf`).

In Docker Compose, `haus_zigbee` and `haus_web` both connect to `haus_mqtt`; `haus_site` (nginx) proxies to `haus_web`.

### CQRS core (`Haus.Cqrs`)

A thin CQRS/mediator layer built on MediatR, exposed through a single facade `IHausBus` (`Haus.Cqrs/HausBus.cs`) with four operations: `ExecuteCommandAsync`, `ExecuteQueryAsync`, `PublishAsync` (events), and `Enqueue`/`FlushAsync` (domain events, inherited from `IDomainEventBus`). Each of Commands/Queries/Events/DomainEvents has its own bus + logging decorator (`LoggingCommandBus`, `LoggingQueryBus`, etc., all built on the shared `LoggingBus` base which times execution and logs start/finish/error). Consumers should only ever depend on `IHausBus`, not the individual sub-buses.

### Domain layer (`Haus.Core`)

Organized by feature/bounded-context folder, not by technical layer: `Devices`, `Rooms`, `DeviceSimulator`, `Discovery`, `Health`, `Logs`, `Lighting`, `Diagnostics`, `Application`. Within each feature folder, the convention is technical subfolders: `Commands`, `Queries`, `Entities`, `Repositories`, `Validators`, `Events` (external/integration events), `DomainEvents` (internal side-effects). `Common` holds cross-cutting building blocks (`Common/Commands`, `Common/Queries`, `Common/Entities`, `Common/Events`, `Common/Storage` — including EF Core migrations). When adding a new feature, mirror this folder shape.

### Models & clients

- **Haus.Core.Models** — feature-mirrored shared DTOs/models (`Devices`, `Rooms`, `Lighting`, `Discovery`, `Health`, `Logs`, `ExternalMessages`, etc.) shared between the API host, the API client, and the site.
- **Haus.Api.Client** — typed client for the Web Host API, mirrors `Haus.Core.Models`' feature folders.
- **Haus.Mqtt.Client** — MQTT wrapper/abstraction used by both `Haus.Web.Host` and `Haus.Zigbee.Host`.
- **Haus.Udp.Client**, **Haus.Hosting**, **Haus.Utilities** — supporting infra. `Haus.Utilities` also hosts CLI tooling (`Common/Cli`) for TypeScript model generation (`TypeScript/GenerateModels`) and Zigbee2Mqtt default device-type option generation (`Zigbee2Mqtt/GenerateDefaultDeviceTypeOptions`).

### Tests

Test projects mirror `src/` 1:1 by name (e.g. `tests/Haus.Core.Tests` ↔ `src/Haus.Core`), with the same feature-folder layout inside. `Haus.Testing.Support` holds shared test infrastructure/fixtures. `Haus.Acceptance.Tests` is a separate end-to-end suite that runs against live `web_host`/`site_host` processes started via the `acceptance` launch profile (see `yarn acceptance` / `run-acceptance-tests.sh`) — Cypress-style env vars (`CYPRESS_AUTH_*`) drive its Auth0 login.

## CI/CD

- **`.github/workflows/main.yaml`** — on push/PR to `main`: sets up the machine (`.github/actions/setup-machine`, which installs .NET/Node, starts a mosquitto broker, trusts dev certs), runs `prepare-build.sh`, `run-unit-tests.sh`, then `run-acceptance-tests.sh`.
- **`.github/workflows/release.yaml`** — manual `workflow_dispatch`: bumps version/tag, publishes app artifacts (`scripts/publish-app.sh`), pushes Docker images (`scripts/publish-to-docker-hub.sh`), and creates a GitHub release with the service package.
