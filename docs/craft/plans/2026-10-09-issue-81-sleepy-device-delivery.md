# Plan — Sleepy/battery end-device-aware outbound delivery (issue #81)

Criteria: `docs/craft/intake/2026-10-09-issue-81-sleepy-device-delivery.md`.
Design: `docs/craft/design/2026-10-09-sleepy-device-aware-outbound-delivery.md`.

## Parallelism map (dependency graph)

```
I1 (root: shared sleepy type on core models)
 ├─ I2  (interview + KnownDeviceTable threading)        depends I1
 ├─ I3  (ZDP Node Descriptor codec, new files)          depends I1   ── parallel with I2
 ├─ I5  (DeviceDiscoveredEvent + DeviceEntity + migration + host mappers)  depends I1 ── parallel with I2,I3,I6
 ├─ I6  (diagnostics parity: model/state/event/publisher)                  depends I1 ── parallel with I2,I3,I5
 └─ I7a (pending-hold store, new file)                  depends I1   ── parallel with I2,I3,I5,I6
I4  (on-demand Node Descriptor re-query + coordinator wiring)  depends I2,I3
I7b (KnownDeviceTable sleepy + NWK→IEEE lookup)                depends I2
I7c (CommandSender hold branch)                                depends I7a,I7b
I7d (wake-signal listener + coordinator wiring)                depends I7a,I7b,I7c,I4
I7e (TTL expiry: drop + report)                                depends I7c,I7d
I8  (unify ZigbeeOutboundRelay onto single path)               depends I7c,I7e
```

Pre-declaring I1 first (the shared `IsSleepy` field on `ZigbeeDevice`/`ZigbeeDeviceJoined`) is
deliberate: multiple downstream increments reference these shared types, and declaring them once up
front keeps parallel implementers from each inventing a colliding field in the same namespace.

## Increments

### I1 — [independent] Decode rx-on-when-idle and carry it on the core Zigbee models
Behavior: bit 3 (rx-on-when-idle) of the MAC capability byte is decoded; `IsSleepy` (= NOT
rx-on-when-idle) is exposed on `DeviceAnnounce`, and a defaulted `IsSleepy` field is added to the
`ZigbeeDevice` and `ZigbeeDeviceJoined` records. No behavior change yet beyond the parse.
Tests: a sleepy byte parses to `IsSleepy == true`, a mains byte to `false`.
Criteria: AC1, AC2 (model shape).
Files:
- `src/Haus.Zigbee/Zdp/DeviceAnnounceParser.cs`
- `src/Haus.Zigbee/Models/ZigbeeDevice.cs`
- `src/Haus.Zigbee/Models/ZigbeeDeviceJoined.cs`
- `tests/Haus.Zigbee.Tests/Zdp/DeviceAnnounceParserTests.cs`

### I2 — [depends: I1] Thread the bit through DeviceInterview and KnownDeviceTable
Behavior: `DeviceInterview.InterviewAsync` forwards the announce's sleepy state into the constructed
`ZigbeeDevice` and `ZigbeeDeviceJoined` instead of dropping it; `KnownDeviceTable.UpdateNetworkAddress`
preserves an existing device's sleepy state on an address-only refresh.
Tests: interviewing a sleepy announce yields a sleepy `ZigbeeDeviceJoined` and a sleepy entry in the
table; `UpdateNetworkAddress` on a sleepy entry keeps it sleepy.
Criteria: AC2, AC3.
Files:
- `src/Haus.Zigbee/Coordinator/DeviceInterview.cs`
- `src/Haus.Zigbee/Coordinator/KnownDeviceTable.cs`
- `tests/Haus.Zigbee.Tests/Coordinator/DeviceInterviewTests.cs`
- `tests/Haus.Zigbee.Tests/Coordinator/KnownDeviceTableTests.cs`

### I3 — [depends: I1] ZDP Node Descriptor Request/Response codec
Behavior: encode a `0x0002` Node Descriptor Request for a network address; decode a `0x8002` response,
surfacing rx-on-when-idle from its MAC capability flags; truncated/malformed payload → null (no throw),
mirroring `NwkAddrResponseCodec`.
Tests: round-trip encode matches the wire layout; a response with rx-on-when-idle clear decodes to
sleepy; a short payload decodes to null.
Criteria: AC-NODEDESC, AC6.
Files (new, disjoint from I2):
- `src/Haus.Zigbee/Zdp/NodeDescriptorRequest.cs`
- `tests/Haus.Zigbee.Tests/Zdp/NodeDescriptorRequestTests.cs`

### I4 — [depends: I2, I3] On-demand Node Descriptor re-query wired into the coordinator
Behavior: a coordinator capability issues a Node Descriptor request for a known IEEE address and
refreshes that device's sleepy state in `KnownDeviceTable` from the `0x8002` response, independent of
any live Device_annce (the after-restart path). Implemented as a new pollLoop subscriber component
(sibling to `NetworkAddressResolver`) plus a coordinator method, to minimise shared-file contention
with I2.
Tests: a decoded Node Descriptor response for a known device updates its `KnownDeviceTable` sleepy
state; an unknown address is a no-op.
Criteria: AC7.
Files:
- `src/Haus.Zigbee/Coordinator/NodeDescriptorQuery.cs` (new)
- `src/Haus.Zigbee/IZigbeeCoordinator.cs`
- `src/Haus.Zigbee/Coordinator/ZigbeeCoordinator.cs`
- `tests/Haus.Zigbee.Tests/Coordinator/NodeDescriptorQueryTests.cs` (new)
- `tests/Haus.Zigbee.Tests/Coordinator/ZigbeeCoordinatorTests.cs`

### I5 — [depends: I1] Persist sleepy state through to DeviceEntity (+ migration) and host mappers
Behavior: `DeviceDiscoveredEvent` carries sleepy state; `DeviceJoinedMapper`, `DevicesMapper`, and
`DeviceBackfillService` populate it from `ZigbeeDeviceJoined`/`ZigbeeDevice`; `DeviceEntity` gains a
persisted power-source/`IsSleepy` field (distinct from `BatteryLevel`) set by
`UpdateFromDiscoveredDevice`; new EF migration + entity configuration + snapshot.
Tests: `DeviceEntity` created from a sleepy `DeviceDiscoveredEvent` reports sleepy and round-trips
through the context; `DeviceJoinedMapper.Map` carries the bit.
Criteria: AC4, AC2 (distinct-from-BatteryLevel constraint).
Files:
- `src/Haus.Core.Models/Devices/Events/DeviceDiscoveredEvent.cs`
- `src/Haus.Core/Devices/Entities/DeviceEntity.cs`
- `src/Haus.Core/Devices/Entities/DeviceEntityConfiguration.cs`
- `src/Haus.Core/Common/Storage/Migrations/*` (new migration + `HausDbContextModelSnapshot.cs`)
- `src/Haus.Zigbee.Host/Zigbee/Mappers/ToHaus/DeviceJoinedMapper.cs`
- `src/Haus.Zigbee.Host/Zigbee/Mappers/ToHaus/DevicesMapper.cs`
- `src/Haus.Zigbee.Host/Zigbee/Services/DeviceBackfillService.cs`
- `tests/Haus.Core.Tests/Devices/Entities/DeviceEntityTests.cs`
- `tests/Haus.Zigbee.Host.Tests/Zigbee/Mappers/ToHaus/DeviceJoinedMapperTests.cs`

Note: this is the only increment that touches the EF migration snapshot — keep it the sole
migration-adding increment so two parallel implementers never both rewrite `HausDbContextModelSnapshot.cs`.

### I6 — [depends: I1] Mirror sleepy state into the diagnostics parity path
Behavior: `ZigbeeKnownDeviceModel` gains a sleepy/power-source property; `IZigbeeState`/`ZigbeeState`
record it on join/info-discovered; the carrying event (`ZigbeeDeviceInfoDiscoveredEvent` and/or
`ZigbeeDeviceJoinedEvent`) carries it; `ZigbeeDiagnosticsPublisher.HandleDeviceJoinedAsync` populates
it from `ZigbeeDeviceJoined`.
Tests: a sleepy `ZigbeeDeviceJoined` published through the diagnostics publisher surfaces on the
known-device model via the state reducer.
Criteria: AC5.
Files (disjoint from I5):
- `src/Haus.Core.Models/Zigbee/ZigbeeKnownDeviceModel.cs`
- `src/Haus.Core.Models/Zigbee/Events/ZigbeeDeviceInfoDiscoveredEvent.cs`
- `src/Haus.Core/Zigbee/State/ZigbeeState.cs`
- `src/Haus.Core/Zigbee/Events/ZigbeeDeviceInfoDiscoveredEventHandler.cs`
- `src/Haus.Zigbee.Host/Zigbee/Services/ZigbeeDiagnosticsPublisher.cs`
- `tests/Haus.Zigbee.Host.Tests/Zigbee/Services/ZigbeeDiagnosticsPublisherTests.cs`
- `tests/Haus.Core.Tests/Zigbee/Events/ZigbeeEventHandlersTests.cs`

### I7a — [depends: I1] Per-device pending-hold store with TTL semantics
Behavior: a new store holds at most one pending command per device IEEE address, stamped with an
expiry = now + configurable TTL (default 1 day); it exposes "take the held command for this device"
(wake) and "take expired holds" (sweep). Pure, time-injectable, no I/O.
Tests: a held command is returned on take; a hold past its TTL is reported expired and not returned
on wake; TTL defaults to 1 day.
Criteria: AC-TTL (TTL value + bounded), AC9 (slot), foundation for AC12.
Files (new):
- `src/Haus.Zigbee/Coordinator/SleepyCommandHold.cs`
- `src/Haus.Zigbee/Coordinator/SleepyHoldOptions.cs`
- `tests/Haus.Zigbee.Tests/Coordinator/SleepyCommandHoldTests.cs`

### I7b — [depends: I2] KnownDeviceTable sleepy lookup + NWK→IEEE reverse resolution
Behavior: `KnownDeviceTable` answers "is the device at this destination (IEEE or NWK) sleepy?" and
resolves a `SourceNwkAddress` back to its IEEE address, so the send branch and the wake listener can
key on the same identity `DeviceCommandQueue` uses.
Tests: a sleepy device is reported sleepy by IEEE and by its NWK address; an unknown destination
reports not-sleepy; NWK→IEEE resolves a known device and returns null for an unknown one.
Criteria: supports AC8, AC9, AC10.
Files:
- `src/Haus.Zigbee/Coordinator/KnownDeviceTable.cs`
- `tests/Haus.Zigbee.Tests/Coordinator/KnownDeviceTableTests.cs`

### I7c — [depends: I7a, I7b] Hold branch in CommandSender
Behavior: `CommandSender.SendCommandAsync` consults the sleepy lookup; a non-sleepy target enqueues
immediately through the unchanged #77 path (AC8); a sleepy target is placed in the pending-hold store
instead of being enqueued (AC9). Hold/replay stays separate from `CommandRetryHandler` (AC11).
Tests: a mains device still enqueues-and-sends; a sleepy device does not reach the queue/sender and
lands in the hold store.
Criteria: AC8, AC9, AC11.
Files:
- `src/Haus.Zigbee/Coordinator/CommandSender.cs`
- `tests/Haus.Zigbee.Tests/Coordinator/CommandSenderTests.cs`

### I7d — [depends: I7a, I7b, I7c, I4] Wake-signal listener releases held commands
Behavior: a new listener subscribes to inbound APS-DATA indications; for any indication whose
`SourceNwkAddress` resolves to a device with a held command, it releases that command into
`DeviceCommandQueue` for delivery (implicit check-in; no Poll Control). Wired into `ZigbeeCoordinator`
alongside the other pollLoop subscribers.
Tests: an inbound indication from a held device's address flushes its command into the queue; an
indication from a device with no hold is a no-op.
Criteria: AC10.
Files:
- `src/Haus.Zigbee/Coordinator/WakeSignalListener.cs` (new)
- `src/Haus.Zigbee/Coordinator/ZigbeeCoordinator.cs`
- `tests/Haus.Zigbee.Tests/Coordinator/WakeSignalListenerTests.cs` (new)
- `tests/Haus.Zigbee.Tests/Coordinator/ZigbeeCoordinatorTests.cs`

### I7e — [depends: I7c, I7d] TTL expiry drops and reports the held command
Behavior: when a held command outlives its TTL (checked on sweep and/or on the next send attempt), it
is dropped and surfaced as a delivery failure equivalent to a permanently-failed command (the same
`CommandDeliveryFailedException`/`TransportError` surfacing today), never retained past TTL.
Tests: a hold that never wakes within TTL produces the delivery-failure signal and is removed; it is
not retained and not silently discarded.
Criteria: AC-TTL (expired → dropped+reported), AC12.
Files:
- `src/Haus.Zigbee/Coordinator/SleepyCommandHold.cs`
- `src/Haus.Zigbee/Coordinator/CommandSender.cs`
- `tests/Haus.Zigbee.Tests/Coordinator/CommandSenderTests.cs`
- `tests/Haus.Zigbee.Tests/Coordinator/SleepyCommandHoldTests.cs`

### I8 — [depends: I7c, I7e] Unify ZigbeeOutboundRelay onto the single sleepy-aware path
Behavior: `ZigbeeOutboundRelay.SendWithApsAckRetryAsync` is removed; lighting commands route through
`coordinator.SendCommandAsync` (→ `CommandSender` → sleepy branch / #77 retry). Any APS-ACK-escalation
worth keeping is folded into the unified path (`CommandRetryHandler`); no second retry/backoff
pipeline survives.
Tests: a lighting command to a sleepy device is held/replayed via the single path (AC13); the relay no
longer builds its own Polly pipeline and no duplicate retry implementation remains (AC14).
Criteria: AC-UNIFY, AC13, AC14.
Files:
- `src/Haus.Zigbee.Host/Zigbee/Services/ZigbeeOutboundRelay.cs`
- `src/Haus.Zigbee/Coordinator/CommandRetryHandler.cs` (only if APS-ACK escalation is folded in)
- `tests/Haus.Zigbee.Host.Tests/Zigbee/Services/ZigbeeOutboundRelayTests.cs`
- `tests/Haus.Zigbee.Tests/Coordinator/CommandRetryHandlerTests.cs`

## Config / DI wiring (fold into the first increment that needs it)
The configurable TTL (`SleepyHoldOptions`, default 1 day) needs binding where `CommandRetryOptions`
is wired — `ZigbeeCoordinator`'s constructor and the host's options binding. Carry this in I7a/I7c
rather than as a separate increment; it touches `ZigbeeCoordinator.cs` (already in I7d's set) and the
host configuration, so sequence it inside the I7 chain, not in parallel with it.

## Suggested dispatch waves
- Wave 1: **I1** (root, alone).
- Wave 2 (parallel, disjoint files): **I2**, **I3**, **I5**, **I6**, **I7a**.
- Wave 3 (parallel): **I4** (after I2,I3), **I7b** (after I2).
- Wave 4: **I7c** → **I7d** → **I7e** (sequential chain, shared files).
- Wave 5: **I8**.
```
```
</content>
