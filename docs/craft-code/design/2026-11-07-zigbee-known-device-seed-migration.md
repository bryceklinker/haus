# Design note: Migrate existing Zigbee device data into Haus.Zigbee.Host's DB (#105)

## Problem shape

`NetworkAddress` / `Endpoints` / `IsSleepy` for devices that paired **before** this feature existed live only in `Haus.Web.Host`'s SQLite `DeviceEntity`. `Haus.Zigbee.Host`'s new `KnownZigbeeDeviceEntity` DB starts empty, and its two self-healing paths cannot recover that data on their own:

- **`DeviceBackfillService`** re-reads devices from `coordinator.GetDevicesAsync()` → `KnownDeviceTable`, which is populated *only* by live `Device_annce` broadcasts (`DeviceInterview`). On a cold start it is empty; a device that is asleep/offline at cutover never re-announces and is simply absent. Confirmed: insufficient on its own, and **out of scope to "fix"** (making the coordinator enumerate the physical network's pre-paired table is a protocol-level change, and still can't reach an offline device).
- The DBs are separate SQLite files on separate Docker volumes — **no shared file access**, so the only path is across a service boundary.

So #105 genuinely needs a **cross-service seed** of Core's existing rows into Zigbee.Host's DB. It is *complementary* to `DeviceBackfillService` (live network), not a replacement — leave `DeviceBackfillService` untouched.

## Boundary, direction, transport

**Direction:** retiring owner (Web.Host/Core) -> new owner (Zigbee.Host). **Transport:** the existing MQTT bus, via a dedicated **retained** "known-device seed snapshot" topic.

Why this and not the alternatives:
- **Zigbee.Host pulls from Web.Host's HTTP API (rejected):** reverses the clean #99 dependency direction (Web.Host -> Zigbee.Host), and would force `IsSleepy` onto Core's *public generic* `DeviceModel`/devices API right before #100 -- expanding the exact leak #99 is removing.
- **Web.Host pushes to a new HTTP write endpoint on Zigbee.Host (rejected):** violates #99's explicit read-only/no-write/no-auth diagnostics boundary.
- **MQTT seed (chosen):** fits the established transport (both hosts already use the bus; Core itself has no MQTT -- outbound publishing is already a Web.Host concern, cf. `SimulatedEventsPublisher`), keeps the HTTP diagnostics API strictly read-only, and adds no new dependency direction.
- **Retained snapshot vs. fire-and-forget at startup:** a plain publish is lost if Zigbee.Host is down at that instant, healed only on the next Web.Host restart -- a weak guarantee against "no user-visible loss" given common restarts. A single **retained** message (the full current snapshot) means the broker replays it to Zigbee.Host the moment it (re)subscribes, closing the startup race cheaply. Matches the project's own "retain needs its own dedicated topic" learning. Cost: the retained message must be cleared when the mechanism is removed (see Lifetime).

## Operations

1. **Web.Host seed publisher** (new, Web.Host) -- runs once at Web.Host startup, following the `BackfillDeviceNetworkAddressesCommand`/`InitializeCommand` *shape* (idempotent, startup-triggered). Reads Core's `DeviceEntity` rows carrying Zigbee data (a thin Core query projecting `ExternalId`, `NetworkAddress`, `Endpoints` {`EndpointId`,`InClusters`}, `IsSleepy`), builds one snapshot, and publishes it **retained** to the dedicated seed topic via `IHausMqttClientFactory`. Idempotent: it's a full-snapshot republish, not an incremental.
2. **Zigbee.Host seed subscriber** (new, in `ZigbeeHausBridge`) -- subscribes to the seed topic alongside the existing `CommandsTopic` subscription, same detached try/catch-log pattern as `HandleHausCommandAsync`. For each device in the snapshot it applies a **non-destructive** persist.
3. **`IKnownZigbeeDeviceStore.SeedIfAbsentAsync(RegisterKnownDevice)`** (new store method) -- inserts a record only when no row exists for that `ExternalId`; **no-op if present**. Primes `DeviceAddressRegistry`'s cache for the address only when absent (so a seeded device resolves immediately, same purpose as `LoadAsync`).

### The load-bearing correctness decision: seed must never clobber live data

The seed is **lossy** -- Core only has `EndpointId`+`InClusters`, whereas `ZigbeeEndpoint` also carries `ProfileId`/`DeviceId`/`OutClusters`, which the seed fills with defaults. Therefore the seed must **defer to live-discovered data**, never downgrade it. `SeedIfAbsentAsync` (insert-if-absent) gives *live-wins convergence in either sequential order*:
- seed first -> live join later overwrites via the existing rich `UpsertAsync` path;
- live join first -> later seed no-ops.

This is a sequential-arrival guarantee, not a concurrent one: `SeedIfAbsentAsync`'s existence-check-then-insert is not atomic against a *concurrent* `UpsertAsync` for the same brand-new device. The unique index on `ExternalId` turns that race into the loser's insert throwing (logged), not a duplicate or silently corrupted row -- and because the targeted offline-at-cutover case can't race (the device isn't live to begin with), and a live device that does lose the race will simply re-announce and have `UpsertAsync` overwrite the seed row on the next pass, this is a narrow, self-healing window rather than a permanent loss. Adversarial review (2026-11-07) confirmed this reading and recommended documenting it rather than adding locking disproportionate to the risk.

This is why the seed does **not** route through the existing `DeviceAddressRegistry.RegisterKnownDevice` (which calls the clobbering `UpsertAsync`) -- that would downgrade a richer live record. It is also what makes the whole mechanism safe to run on every restart.

## Types and where they live

- **Seed event contract** (new): a dedicated internal event `{ ExternalId, NetworkAddress, Endpoints[{EndpointId,InClusters}], IsSleepy }`, placed in `Haus.Core.Models/Zigbee/Events/` -- the **already-shared** namespace both hosts reference (Web.Host via `Haus.Core`, Zigbee.Host directly) **and** the namespace #100 already deletes. Web.Host does *not* reference `Haus.Zigbee.Models`, so that project is not a viable shared home without a new reference; `Haus.Core.Models` is the existing common denominator.
  - This is also **how `IsSleepy` crosses**: a dedicated event, *not* the public `DeviceModel`. No new generic-API surface is added to Core.
  - Rejected reuse of `DeviceDiscoveredEvent` (carries all three fields) -- Zigbee.Host *publishes* it and Core consumes it with heavy side effects (`UpdateFromDiscoveredDevice` resets lighting); re-publishing it would self-loop and mutate Core devices. Rejected `ZigbeeDeviceInfoDiscoveredEvent` -- lacks `NetworkAddress` and runs the wrong direction.
- **`RegisterKnownDevice`** already exists and is the right input record for `SeedIfAbsentAsync`.

## Detached-work owner & failure signals

- Publisher failure: surfaces through the existing command/logging-bus decorator at Web.Host startup; evidence = log + no retained snapshot on the topic.
- Subscriber failure: owned by the `ZigbeeHausBridge` handler's try/catch-log (matching the existing per-topic handlers); per-device persist failures logged as today's `PersistKnownDeviceAsync` does. Evidence that it broke = the device missing from `GET api/zigbee/devices`; evidence of why = the handler/persist log line.
- Both persist writes stay on the existing detached `Forget` pattern in `DeviceAddressRegistry`.

## Lifetime / scope guards

- **Transitional, not permanent.** After #100 drops Core's columns the publisher has nothing to read and becomes a no-op. The whole mechanism (seed event model in the doomed `Core.Models/Zigbee` namespace, the Web.Host publisher, the subscriber, `SeedIfAbsentAsync`, and clearing the retained message) is removed **as part of #100** -- flagging this for the planner so #100's "remove the now-replaced stuff" includes it.
- #105 **only reads** Core's columns; it does **not** drop them (#100).
- #105 does **not** touch `DeviceBackfillService`'s live-reachability behavior.
- No write/auth surface added to the read-only diagnostics API; no new HTTP dependency direction; no `IsSleepy` on `DeviceModel`.
- Independent of whether #103's consumer cutover has merged (it hasn't -- `Haus.Web.Host/Zigbee/ZigbeeController.cs` still reads Core's in-memory store). #105's destination is Zigbee.Host's DB regardless.

## Verification ("no user-visible loss")

- **Unit (Web.Host, via real DI container per the project's test-DI rule):** publisher reads seeded `DeviceEntity` rows and emits one retained snapshot carrying `NetworkAddress`/`Endpoints`/`IsSleepy`.
- **Unit (Zigbee.Host, real DI + EF InMemory as existing store tests do):** `SeedIfAbsentAsync` creates an absent record; is a no-op when present; **live-wins** test -- a prior rich `UpsertAsync` record is not downgraded by a later seed; **idempotency** test -- running the seed twice yields one record.
- **Acceptance (the real guarantee):** boot the full stack via `docker-compose.local.yml` with Web.Host's DB pre-seeded with a device that is *not* live on the coordinator (so `DeviceBackfillService` cannot account for it) carrying `NetworkAddress`/`Endpoints`/`IsSleepy`; after startup assert `GET {zigbee-host}/api/zigbee/devices` returns that device with the same values. This proves the offline-at-cutover case that live re-discovery can't cover.

## Files this touches (for the planner)

- New: seed event in `src/Haus.Core.Models/Zigbee/Events/`; Core query projecting the three columns (`src/Haus.Core/Devices/Queries` or the doomed `Zigbee` folder); Web.Host startup publisher (near `src/Haus.Web.Host/DeviceSimulator/Events/SimulatedEventsPublisher.cs` as the outbound-publish precedent); `SeedIfAbsentAsync` in `src/Haus.Zigbee.Host/Storage/{IKnownZigbeeDeviceStore,KnownZigbeeDeviceStore}.cs`.
- Changed: `src/Haus.Zigbee.Host/Zigbee/Services/ZigbeeHausBridge.cs` (new subscription + handler); possibly `src/Haus.Zigbee.Host/Zigbee/DeviceAddressRegistry.cs` (seed-cache-prime entry point); seed topic name in the MQTT/Haus options both hosts read.
- Unchanged by design: `DeviceBackfillService`, the read-only `ZigbeeDiagnosticsController`, Core's public `DeviceModel`.
