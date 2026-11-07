# Plan: Migrate existing Zigbee device data into Haus.Zigbee.Host's DB (#105)

Mechanism is already decided in the design note
(`scratchpad/issue-105-design-note.md`): Web.Host publishes a one-time,
idempotent, **retained** MQTT snapshot at startup; Zigbee.Host's
`ZigbeeHausBridge` subscribes and seeds its own `KnownZigbeeDeviceStore`
via a new insert-if-absent operation that never clobbers richer
live-discovered data.

## Acceptance criteria (agreed)

Live-wins / non-destructive seed
- Given Zigbee.Host's store has no row for an `ExternalId`, when a seed for
  that device is applied, then a row is created with the seeded
  `NetworkAddress` / `Endpoints` / `IsSleepy`.
- Given Zigbee.Host's store already has a row for an `ExternalId`, when a seed
  for that device is applied, then the existing row is left unchanged (no-op).
- Given a device was first persisted through the rich live path
  (`UpsertAsync`, carrying `ProfileId`/`DeviceId`/`OutClusters`), when a later
  seed for the same `ExternalId` arrives, then the live record is not
  downgraded (the richer endpoint fields survive).
- Given a seed first created a row, when the device is later live-discovered,
  then the existing rich `UpsertAsync` path overwrites the seeded row (live
  wins in either arrival order).

Idempotency
- Given the same seed snapshot, when it is applied twice, then the store holds
  exactly one row per `ExternalId` (no duplicates, no churn).
- Given Web.Host restarts, when it publishes again, then it republishes the
  full current snapshot to the same retained topic (full-snapshot republish,
  not an incremental), so the broker holds one current snapshot.

Publisher
- Given Core `DeviceEntity` rows carrying Zigbee data, when the Web.Host seed
  publisher runs at startup, then it emits exactly one **retained** message on
  the dedicated seed topic whose payload carries, per device, `ExternalId`,
  `NetworkAddress`, `Endpoints` (`EndpointId` + `InClusters` only), and
  `IsSleepy`.
- Given no devices carry Zigbee data (e.g. after #100 drops the columns), when
  the publisher runs, then it is a harmless no-op snapshot (empty device list),
  i.e. the mechanism degrades to nothing once there is nothing to read.

Subscriber
- Given a retained seed snapshot on the seed topic, when Zigbee.Host
  (re)subscribes at startup, then the broker replays the snapshot and each
  device is seeded via the insert-if-absent path, closing the startup race
  without requiring Web.Host to be up at that instant.
- Given one device in a snapshot fails to persist, when the handler processes
  it, then the failure is logged and the remaining devices still seed
  (per-device isolation, matching the existing bridge handlers' try/catch-log).
- Given a seeded device (endpoints carry only `EndpointId` + `InClusters`),
  when it is stored, then the missing `ProfileId` / `DeviceId` / `OutClusters`
  are filled with defaults (the seed is knowingly lossy; live data later
  supplies the rest).

No user-visible loss across cutover (the real guarantee — acceptance)
- Given Web.Host's DB is pre-seeded with a device carrying
  `NetworkAddress` / `Endpoints` / `IsSleepy` that is **not** live on the
  coordinator (so `DeviceBackfillService` cannot account for it), when the full
  stack boots via `docker-compose.local.yml`, then
  `GET {zigbee-host}/api/zigbee/devices` returns that device with the same
  values.

Explicit non-goals (out of scope here)
- Does **not** drop Core's `NetworkAddress` / `IsSleepy` / `Endpoints` columns
  or the `Haus.Core/Zigbee` migrations — that is #100, which also removes this
  whole transitional mechanism (seed event, publisher, subscriber,
  `SeedIfAbsentAsync`, and clearing the retained message).
- Does **not** change `DeviceBackfillService`'s live-reachability behavior.
- Does **not** add any write/auth surface to the read-only diagnostics API, add
  a new HTTP dependency direction, or put `IsSleepy` on Core's public
  `DeviceModel`/devices API.

## Fix-level decision

The level was a real choice and is already recorded in the design note: a
cross-service **MQTT retained seed** (retiring owner Web.Host → new owner
Zigbee.Host), chosen over (a) Zigbee.Host pulling Web.Host's HTTP API — reverses
#99's dependency direction and forces `IsSleepy` onto Core's public API; and (b)
Web.Host pushing to an HTTP write endpoint on Zigbee.Host — violates #99's
read-only diagnostics boundary. Retained (not fire-and-forget) is chosen so the
broker replays the snapshot whenever Zigbee.Host (re)subscribes, closing the
startup race. Not re-opening these here.

## Increments

Increment 1 is the shared contract both branches compile against — land it
first (pre-declare the shared type before any parallel work, per the
symbol-collision learning). Branches A (publisher) and B (subscriber) then touch
disjoint projects and are independent of each other; within each branch the
steps are sequential. The acceptance test (0) is written up front and goes green
once both branches land.

0. [independent] Outer acceptance test (written first, red until A+B land)
     Pre-seed Web.Host's DB with an offline-at-cutover device carrying
     `NetworkAddress`/`Endpoints`/`IsSleepy`; boot the full stack; assert
     `GET {zigbee-host}/api/zigbee/devices` returns it with the same values.
     criteria: "no user-visible loss across cutover"
     files: tests/Haus.Acceptance.Tests/* (+ any test-only Web.Host DB seed hook)

1. [depends: none] Seed event contract + dedicated retained topic
     New `ZigbeeKnownDeviceSeedEvent` in
     `src/Haus.Core.Models/Zigbee/Events/` — one snapshot carrying a device
     list; each entry `{ ExternalId, NetworkAddress, Endpoints (reuse
     `DeviceEndpointModel` = EndpointId + InClusters), IsSleepy }`; implements
     `IHausEventCreator<T>` with a `Type` constant. Add the seed topic constant
     to `DefaultHausMqttTopics` and a property to `HausMqttSettings` (so both
     hosts, incl. Zigbee.Host's `HausOptions`, read the same topic name).
     criteria: "publisher emits one retained message on the dedicated seed
     topic"; shared type for both branches
     files: src/Haus.Core.Models/Zigbee/Events/ZigbeeKnownDeviceSeedEvent.cs,
            src/Haus.Core.Models/Common/DefaultHausMqttTopics.cs,
            src/Haus.Mqtt.Client/Settings/HausMqttSettings.cs

--- Branch A: publisher (Core + Web.Host) ---

2. [depends: 1] Core query projecting the Zigbee columns into seed entries
     A query (shape of `BackfillDeviceNetworkAddressesCommand` — reads
     `HausDbContext`) projecting `DeviceEntity` rows carrying Zigbee data to the
     seed entry list (`ExternalId`, `NetworkAddress`, `Endpoints`
     {EndpointId, InClusters}, `IsSleepy`). Unit-tested via the real DI
     container + `HausDbContext` (test-DI rule).
     criteria: "payload carries, per device, ExternalId/NetworkAddress/
     Endpoints/IsSleepy"; "empty list when nothing carries Zigbee data"
     files: src/Haus.Core/Devices/Queries/* (new query + handler),
            tests/Haus.Core.Tests/Devices/Queries/*

3. [depends: 2] Web.Host startup seed publisher
     New publisher (near `SimulatedEventsPublisher` as the outbound-publish
     precedent) that runs the query, builds one snapshot, and publishes it
     **retained** to the seed topic via `IHausMqttClientFactory`; idempotent
     full-snapshot republish. Unit-tested via real DI asserting a single
     retained publish with the expected payload. Wire its one-shot startup
     trigger following the `InitializeCommand`/`Program.cs` precedent (thin
     seam, proven by the acceptance test, not a unit test).
     criteria: "publisher emits one retained message..."; "harmless no-op
     snapshot when nothing to read"; "full-snapshot republish on restart"
     files: src/Haus.Web.Host/Zigbee/* (new publisher), src/Haus.Web.Host/Program.cs,
            tests/Haus.Web.Host.Tests/Zigbee/*

--- Branch B: subscriber (Zigbee.Host) — independent of Branch A ---

4. [depends: 1] `SeedIfAbsentAsync` insert-if-absent store operation
     New `SeedIfAbsentAsync(RegisterKnownDevice)` on `IKnownZigbeeDeviceStore` /
     `KnownZigbeeDeviceStore`: insert only when no row exists for the
     `ExternalId`; no-op if present (does NOT route through the clobbering
     `UpsertAsync`). Unit-tested via real DI + EF InMemory (existing store-test
     shape): creates absent; no-op present; live-wins (a prior `UpsertAsync`
     rich record is not downgraded); idempotent (twice → one row).
     criteria: "row created when absent"; "left unchanged when present"; "live
     record not downgraded"; "exactly one row per ExternalId"
     files: src/Haus.Zigbee.Host/Storage/IKnownZigbeeDeviceStore.cs,
            src/Haus.Zigbee.Host/Storage/KnownZigbeeDeviceStore.cs,
            tests/Haus.Zigbee.Host.Tests/Storage/KnownZigbeeDeviceStoreTests.cs

5. [depends: 4] Registry seed-prime entry point
     New `SeedKnownDevice(RegisterKnownDevice)` on `DeviceAddressRegistry` that
     primes the in-memory cache for the address **only when absent** and calls
     `store.SeedIfAbsentAsync` via the existing detached `Forget` pattern (so a
     seeded device resolves immediately, same purpose as `LoadAsync`, without
     clobbering a live cache entry). Unit-tested.
     criteria: "device seeded via the insert-if-absent path"
     files: src/Haus.Zigbee.Host/Zigbee/DeviceAddressRegistry.cs,
            tests/Haus.Zigbee.Host.Tests/Zigbee/DeviceAddressRegistryTests.cs

6. [depends: 5] Bridge subscribes to the seed topic and applies the snapshot
     `ZigbeeHausBridge` subscribes to the seed topic alongside `CommandsTopic`;
     handler deserializes the snapshot, maps each entry's endpoints
     (EndpointId + InClusters → `ZigbeeEndpoint` with default
     ProfileId/DeviceId/OutClusters) and calls `SeedKnownDevice` per device,
     with the same detached per-item try/catch-log as the existing handlers.
     criteria: "broker replays the retained snapshot and each device is
     seeded"; "one failing device is logged and the rest still seed"; "missing
     endpoint fields filled with defaults"
     files: src/Haus.Zigbee.Host/Zigbee/Services/ZigbeeHausBridge.cs,
            tests/Haus.Zigbee.Host.Tests/Zigbee/Services/*

## Notes for implementation

- Strict-TDD each increment; increment 0 is the outer double-loop acceptance
  test (watch it fail first, let 1–6 drive it green).
- Reuse the existing `RegisterKnownDevice` record as `SeedIfAbsentAsync`'s input
  and `DeviceEndpointModel` for the seed entry's endpoints — no new near-duplicate
  types.
- The retained snapshot must be cleared when #100 removes this mechanism; that
  removal is #100's responsibility (flagged in the design note), not this work.
