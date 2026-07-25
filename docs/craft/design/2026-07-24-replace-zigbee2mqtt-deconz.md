# Design note — Replace Zigbee2MQTT with first-party deCONZ library (issue #9)

Status: architecture decided, ready for planning.
Scope basis: agreed acceptance criteria from intake (do not re-litigate).
Primary protocol reference: Zigbee spec **05-3474-23-csg** (CSA IoT) for APS/NWK/ZDP/ZCL
semantics; dresden-elektronik deCONZ serial protocol docs + zigbee-herdsman's deconz
adapter for the *vendor framing* that wraps those. deCONZ framing is vendor-specific and
NOT in the CSA spec — it carries APS/ZDP/ZCL payloads defined by the spec.

## The load-bearing boundary (from intake, restated)

Two projects, one hard wall between them, enforced at the **project-reference level** so the
compiler guards it:

- **`Haus.Zigbee`** (new class library) — protocol-only. Knows IEEE/network addresses,
  endpoints, ZCL clusters/attributes/commands, ZDP interview, deCONZ serial framing. It
  references NO Haus code (not `Haus.Core.Models`, not `Haus.Mqtt.Client`). Its only
  outward dependency is `System.IO.Ports` (+ optionally `System.IO.Pipelines`). If a PR ever
  makes `Haus.Zigbee` reference a Haus domain assembly, the boundary has been breached.
- **`Haus.Zigbee.Host`** — keeps its current job: translate between the raw protocol (now via
  `Haus.Zigbee` instead of zigbee2mqtt-shaped MQTT JSON) and Haus domain events/commands, and
  bridge to the Haus MQTT bus. The MQTT contract to `Haus.Web.Host` (topics + payloads) does
  NOT change.

## Two hexagons

### Hexagon A — `Haus.Zigbee` (self-contained protocol engine)

```
                       driving side (called by the Host)
                    ┌──────────────────────────────────────┐
                    │        IZigbeeCoordinator (facade)     │
                    │  ConnectAsync / GetDevicesAsync /      │
                    │  SetPermitJoinAsync / SendZclCommand   │
                    │  + DeviceJoined / AttributeReported    │
                    └──────────────────────────────────────┘
                                    │
        ┌───────────────────────────────────────────────────────────┐
        │  DOMAIN CENTER (protocol logic, no Haus types)             │
        │   • Deconz frame codec (SLIP + CRC, command frames)       │
        │   • Deconz protocol state machine (req/resp correlation,  │
        │     device-state poll, APS data request/indication)       │
        │   • APS data unit shapes (profile, cluster, endpoints)    │
        │   • ZCL codec (frame control/seq/cmd, foundation cmds,    │
        │     attribute type decode) + cluster/attribute constants  │
        │   • ZDP builders/parsers + interview orchestration        │
        │   • Network/device registry (IEEE, nwk addr, endpoints)   │
        └───────────────────────────────────────────────────────────┘
                                    │
                    ┌──────────────────────────────────────┐
                    │  PORT (driven):  ISerialTransport      │  ← the ONE seam
                    │  open/close, write(bytes),             │
                    │  bytes-received stream                 │
                    └──────────────────────────────────────┘
                                    │
                    ┌──────────────────────────────────────┐
                    │  ADAPTER: SerialPortTransport          │
                    │  (System.IO.Ports over /dev/ttyACM0)   │
                    └──────────────────────────────────────┘
```

**`ISerialTransport` is the single testability seam.** It abstracts the raw byte pipe to the
stick. A fake deCONZ endpoint substitutes here (replays/asserts frame bytes) so the whole
protocol engine can be driven in-process with no hardware. *Design of that fake is planning/
acceptance-testing's job — this note only guarantees the seam exists and is the only one
needed.* Everything above the seam is real, deterministic, in-process code (per strict-TDD's
"real implementations wherever they run deterministically").

Why exactly one port: the criteria demand talking to one serial coordinator. There is no
second transport in sight (no TCP deCONZ, no other adapter model), so no speculative
abstraction beyond `ISerialTransport`. YAGNI.

### Hexagon B — `Haus.Zigbee.Host` (translation, mostly-existing shape)

```
   Haus MQTT bus (haus/commands)                Haus MQTT bus (haus/events, haus/idk)
              │                                              ▲
              ▼                                              │
   ┌──────────────────────┐   translate    ┌───────────────────────────────┐
   │  ToZigbee mappers     │ ─────────────► │  ToHaus mappers                │
   │  Haus cmd → protocol  │                │  protocol event → Haus event   │
   └──────────────────────┘                └───────────────────────────────┘
              │  calls                                    ▲  subscribes to events
              ▼                                           │
        ┌───────────────────────────────────────────────────────┐
        │  PORT: IZigbeeCoordinator  (from Haus.Zigbee)           │
        └───────────────────────────────────────────────────────┘
        ┌───────────────────────────────────────────────────────┐
        │  PORT: IHausMqttClient  (existing, unchanged)           │
        └───────────────────────────────────────────────────────┘
```

The relay (`ZigbeeToHausRelay`, a `BackgroundService`) is the driving adapter. Today it wires
two MQTT clients. It becomes: subscribe to Haus commands over MQTT (unchanged) → ToZigbee →
`IZigbeeCoordinator` calls; and subscribe to `IZigbeeCoordinator` events → ToHaus → publish
Haus events/unknown over MQTT (unchanged topics/payloads).

## `Haus.Zigbee` public API surface (contract-first — fix this early)

All inputs are immutable records; the facade is the operation surface. Namespaced under
`Haus.Zigbee` (no Haus domain leakage). Illustrative shapes, not final signatures:

```
public readonly record struct IeeeAddress(ulong Value);   // canonical "0x00158d0001abcd12" formatting

public sealed record ZigbeeEndpoint(
    byte EndpointId, ushort ProfileId, ushort DeviceId,
    IReadOnlyList<ushort> InClusters, IReadOnlyList<ushort> OutClusters);

public sealed record ZigbeeDevice(
    IeeeAddress Ieee, ushort NetworkAddress,
    string ManufacturerName, string ModelIdentifier,   // Basic cluster 0x0000 attrs 0x0004/0x0005
    IReadOnlyList<ZigbeeEndpoint> Endpoints);

public sealed record ZclCommandRequest(
    IeeeAddress Target, byte Endpoint, ushort ClusterId,
    byte CommandId, ReadOnlyMemory<byte> Payload,
    bool ClusterSpecific = true);   // generic send — NOT "set lighting"

public sealed record ZigbeeAttributeReport(       // generic — NO illuminance/occupancy semantics
    IeeeAddress Source, ushort NetworkAddress, byte Endpoint,
    ushort ClusterId, ushort AttributeId, byte ZclDataType, object? Value);

public interface IZigbeeCoordinator
{
    Task ConnectAsync(CancellationToken ct);                       // open serial, handshake,
                                                                   //   read net config + device list
    Task<IReadOnlyList<ZigbeeDevice>> GetDevicesAsync(CancellationToken ct);
    Task SetPermitJoinAsync(TimeSpan duration, CancellationToken ct);  // duration.Zero = close
    Task SendZclCommandAsync(ZclCommandRequest request, CancellationToken ct);

    event EventHandler<ZigbeeDeviceJoinedEventArgs> DeviceJoined;      // after interview completes
    event EventHandler<ZigbeeAttributeReportEventArgs> AttributeReported;
}
```

`ZigbeeDeviceJoinedEventArgs` carries a fully-interviewed `ZigbeeDevice` (raw IEEE/nwk/
endpoints/manufacturer/model) — no vendor→DeviceType mapping (that stays Host-side).

**Decision — events vs Channel/IAsyncEnumerable for inbound (why):** the serial read loop is a
background producer; the relay is a background consumer. Plain C# `event`s are the minimal
shape and mirror how the current relay reacts to inbound messages, so I default to events.
`Channel<T>`/`IAsyncEnumerable` would give backpressure and simpler cancellation but add
surface the criteria don't require. Flagging as a reversible call for planning; if the read
loop needs backpressure it can switch without changing the mapper logic.

**Decision — `ConnectAsync` must not re-form the network (why):** existing paired devices must
survive. `ConnectAsync` only *reads* coordinator parameters and the existing device/neighbor
table (deCONZ read-parameter + APS device-table reads); it never issues network-form/leave.
This is a hard constraint from intake, called out so no increment "initializes" the network.

## Internal structure of `Haus.Zigbee`

Folder / namespace layout (mirrors the protocol stack, bottom → top):

- `Serial/` — `ISerialTransport`, `SerialPortTransport`. (the seam + its one adapter)
- `Deconz/` — `DeconzFrameCodec` (SLIP framing + CRC16), `DeconzCommand` enum, request/
  response frame records (Read/Write Parameter, Device State, APS Data Request/Indication/
  Confirm), `DeconzProtocol` (req/resp correlation + device-state poll loop, surfaces APS
  data indications).
- `Aps/` — APS data-unit record (profile id, cluster id, src/dest endpoints, ASDU bytes) —
  the payload that rides inside deCONZ APS frames (CSA spec § APS).
- `Zcl/` — `ZclFrameCodec` (frame control, transaction seq, command id), foundation commands
  Read Attributes (0x00) / Read Attributes Response (0x01) / Report Attributes (0x0a),
  attribute datatype decoding, and `ZclClusters`/`ZclAttributes` constants.
- `Zdp/` — request builders + response parsers for Active Endpoints (req 0x0005 / resp
  0x8005), Simple Descriptor (req 0x0004 / resp 0x8004), Device_annce (0x0013); plus
  `DeviceInterview` orchestration (active endpoints → per-endpoint simple descriptor → Basic
  cluster read of ManufacturerName 0x0004 / ModelIdentifier 0x0005).
- `Network/` — `DeviceRegistry` (IEEE ↔ nwk addr ↔ endpoints), coordinator config snapshot.
- root — `IZigbeeCoordinator` + `DeconzZigbeeCoordinator` facade, event args, public records.

Cluster/attribute IDs the Host will need the library to carry generically (grounded in
05-3474-23-csg ZCL, listed here so planning knows the constants exist, not to bake semantics
into the library): Basic 0x0000, Power Configuration 0x0001, On/Off 0x0006, Level Control
0x0008, Color Control 0x0300, Illuminance Measurement 0x0400, Temperature Measurement 0x0402,
Occupancy Sensing 0x0406. The library exposes these as raw IDs; it does NOT interpret them.

## How `Haus.Zigbee.Host`'s `Zigbee2Mqtt/` folder evolves

Rename `Zigbee2Mqtt/` → `Coordinator/` (it no longer speaks to zigbee2mqtt). Keep the
**ToHaus / ToZigbee split** — it's the right shape and the criteria say to preserve it.

Removed (no longer any MQTT-to-zigbee2mqtt hop):
- `Mqtt/MqttClientFactory.cs` — the *zigbee* client half goes; the *Haus* client half stays
  (relay still needs one `IHausMqttClient` for the Haus bus).
- `Models/Zigbee2MqttMessage.cs`, `Models/Zigbee2MqttMeta.cs` — replaced by `Haus.Zigbee`
  records (`ZigbeeDevice`, `ZigbeeAttributeReport`, etc.).
- `Mappers/ToHaus/Factories/Zigbee2MqttMessageFactory.cs` — no MQTT-JSON parsing anymore.
- `Configuration/Zigbee2MqttConfiguration.cs` + `ZigbeeOptions` (mqtt base topic/server) →
  replaced by serial options (port name e.g. /dev/ttyACM0, baud).

Kept, re-pointed to protocol events instead of `Zigbee2MqttMessage`:
- `Mappers/ToHaus/DevicesMapper` — now maps `IReadOnlyList<ZigbeeDevice>` (from
  `GetDevicesAsync`) → `DeviceDiscoveredEvent` per device. Same output payload/topic.
- `Mappers/ToHaus/InterviewSuccessfulMapper` — now driven by the `DeviceJoined` event →
  `DeviceDiscoveredEvent`. Same output.
- `Mappers/ToHaus/DeviceEvents/*` (Device/Sensor/Battery/Illuminance/Occupancy/Temperature)
  — now driven by `AttributeReported`. **NEW responsibility (significant):** the ZCL
  cluster/attribute → sensor-semantics decode that zigbee2mqtt used to do MOVES here.
  e.g. cluster 0x0400 attr 0x0000 → IlluminanceChangedModel; 0x0406/0x0000 → Occupancy;
  0x0402/0x0000 → Temperature; 0x0001 (Power Config) battery → BatteryChangedModel. The
  existing Haus output models (`IlluminanceChangedModel`, etc.) are untouched in shape.
- `Mappers/ToHaus/Resolvers/DeviceTypeResolver` + `DefaultDeviceTypeOptions.json` — KEPT
  UNCHANGED. Still vendor/model → `DeviceType`, now fed by ManufacturerName/ModelIdentifier
  from the Basic-cluster read instead of z2m's `meta.vendor`/`meta.model`.
- `Mappers/ToHaus/UnknownMessageMapper` — kept; fires for attribute reports the sensor
  mappers don't recognize (preserves the haus/idk safety-net behavior).
- `Mappers/ToZigbee/HausDiscoveryToZigbeeMapper` — StartDiscovery → `SetPermitJoinAsync(dur)`;
  StopDiscovery → `SetPermitJoinAsync(Zero)`; SyncDiscovery → `GetDevicesAsync` then feed the
  device-list mapper. (Was permit_join / config/devices/get MQTT messages.)
- `Mappers/ToZigbee/HausLightingToZigbeeMapper` — **NEW responsibility (significant):**
  translate `LightingModel` (state / level / color_temp / color) into ZCL commands:
  On/Off (0x0006) on/off; Level Control (0x0008) move-to-level-with-on-off; Color Control
  (0x0300) move-to-color-temp / move-to-color. Emits `ZclCommandRequest`(s) via the
  coordinator. This ZCL command construction MOVES here from zigbee2mqtt.
- `Mappers/MqttMessageMapper` + `Mappers/ToZigbee/HausToZigbeeMapper` + `ToHaus/*` mapper
  dispatch — keep the "collection of mappers, pick supported" structure; only the input type
  changes (protocol event/command object instead of `MqttApplicationMessage`).

Health: `Zigbee2MqttHealthCheck` (currently a stub returning Healthy) → `CoordinatorHealthCheck`
reporting real serial-connection state from `IZigbeeCoordinator`. Now meaningful, not a stub.

Config/DI (`ServiceCollectionExtensions`): drop the zigbee MQTT client factory + z2m options;
register `IZigbeeCoordinator` (singleton, owns the serial connection) + serial options; keep
Haus MQTT + the mapper registrations (re-pointed).

### Compatibility risk to carry into planning (device identity)

The MQTT contract to `Haus.Web.Host` must not change, which includes the device **ExternalId**.
Today z2m's `friendly_name` is the device id in `DeviceDiscoveredEvent` and the `/set` topic
segment (`HausLightingToZigbeeMapper` uses `device.ExternalId`). Unrenamed z2m devices default
`friendly_name` to the IEEE address string (e.g. `0x00158d0001abcd12`). So the Host must derive
ExternalId from `IeeeAddress` in **exactly the same string form** already persisted in
`Haus.Web.Host`'s SQLite for existing devices, and the ToZigbee path must parse ExternalId back
to `IeeeAddress` to address commands. This is the single biggest parity risk. It is a
requirements/data question (what string are existing rows keyed by?), not an architecture one —
flagging it here so intake/planning nail the exact IEEE formatting before the mapper increments;
`IeeeAddress` formatting must be pinned to match.

## Where it lives / project wiring

- New `src/Haus.Zigbee` (classlib) + `tests/Haus.Zigbee.Tests`, added via
  `./scripts/add-project.sh classlib Haus.Zigbee` (and `test` for the tests), registered in
  `Haus.slnx`.
- `Haus.Zigbee.Host.csproj` adds a ProjectReference to `Haus.Zigbee` and drops the zigbee-side
  MQTT usage. `Haus.Zigbee` references only `System.IO.Ports` — no Haus assemblies (this is the
  boundary, enforced by the compiler).
- `configuration.yaml` / appsettings: serial port (`/dev/ttyACM0`, deCONZ) replaces the z2m
  mqtt base-topic/server block; Haus MQTT settings stay. Docker: `haus_zigbee` container needs
  the serial device passed through (`--device`), and the zigbee2mqtt service is removed from
  compose — but compose/deployment edits are planning/verification concerns, noted not designed.

## Sequencing / layering (hard dependencies for planning to order increments)

Bottom-up in `Haus.Zigbee` (each needs the ones above it):

1. `ISerialTransport` + `SerialPortTransport` (the seam) — nothing depends on real hardware
   above this once the fake exists.
2. `DeconzFrameCodec` (SLIP + CRC + command frames) — **foundational; everything rides on it.**
3. `DeconzProtocol` req/resp correlation + device-state poll + APS data request/indication
   plumbing — needs 2.
4. Connect/handshake + read network params + read existing device/neighbor table — needs 3.
   (Can proceed in parallel with 5 — it's coordinator parameter/table reads, not ZCL.)
5. APS unit + `ZclFrameCodec` (attribute decode + foundation cmds) — needs 2/3 to carry it;
   **prerequisite for interview, attribute reports, AND command send.**
6. ZDP builders/parsers + `DeviceInterview` — needs 5 (+ APS send/receive).
7. Permit-join + Device_annce handling → `DeviceJoined` event — needs 6 (interview runs on join).
8. `AttributeReported` event (decode ZCL Report/Read-Attributes) — needs 5.
9. `SendZclCommandAsync` — needs 5 (+ APS data request).

Hard gates: frame codec (2) before all; ZCL codec (5) before interview/reports/send;
permit-join before the join→interview→DeviceJoined flow. Device-list read (4) can land early.

Cross-project parallelism: **fix `IZigbeeCoordinator` first (contract-first).** Once its shape
is frozen, two tracks run in parallel — (a) the `Haus.Zigbee` internals against
`ISerialTransport`, and (b) the Host mapper rework against a faked `IZigbeeCoordinator` (same
technique the current tests use with synthetic MQTT). Neither track blocks the other until
final wiring. Planning should mark these independent.

## Guardrail check

- No speculative ports: one seam (`ISerialTransport`) in the library, existing `IHausMqttClient`
  reused in the Host. No second transport designed "in case."
- No production code written here.
- Preferred existing shape: the Host keeps its ToHaus/ToZigbee mapper structure, DeviceType
  resolver, and MQTT contract; only the source of raw data changes.
