# Plan: Replace Zigbee2MQTT with first-party Haus.Zigbee (issue #9)

Intake and architecture are agreed (see task brief). This decomposes the change into
small, independently-testable increments in the architect's hard sequencing order:
transport → frame codec → read config/device-list + permit-join → APS send/receive →
ZCL send + attribute-report parse → ZDP + interview → façade → host restructure → deploy.

## Ground rules baked into this decomposition
- Style constraint (non-negotiable): event/message **data** is separate from **handling**;
  one small single-purpose class per message/frame/handler. That is why each deCONZ command
  frame, each ZCL/ZDP payload, and each coordinator capability is its own increment/file
  rather than fused into a "codec" or "relay" god-class.
- `ISerialTransport` is THE test seam. Every library increment above the transport is driven
  by a `FakeSerialTransport` in-process (classicist TDD, no mocking our own code). No real
  hardware is needed to build or test the library — only for production runtime.
- Independence rule: two increments are `[independent]` only if they touch **disjoint files**.
  When unsure, marked dependent.
- File paths below are the intended new files; `Serial/`, `Zcl/`, `Zdp/`, `Coordinator/`,
  `Connection/`, `Transport/` are folders inside `src/Haus.Zigbee/` (tests mirror under
  `tests/Haus.Zigbee.Tests/`).

---

## Phase 0 — scaffolding

1. **Scaffold projects** `[root — everything depends on this]`
   Create `Haus.Zigbee` classlib and `Haus.Zigbee.Tests` via `./scripts/add-project.sh`,
   add both to `Haus.slnx`, reference `System.IO.Ports`, wire test deps + `Haus.Testing.Support`.
   criteria: enabling structure for the whole `Haus.Zigbee` library.
   files: `Haus.slnx`, `src/Haus.Zigbee/Haus.Zigbee.csproj`, `tests/Haus.Zigbee.Tests/Haus.Zigbee.Tests.csproj`

## Phase 1 — pure foundational codecs (all parallel after #1)

2. **`IeeeAddress` value type** `[depends: 1]` — HIGH-VALUE parity primitive (risk #1 root)
   Parse/format 8-byte IEEE address ↔ `0x`-prefixed lowercase hex string; value equality.
   criteria: raw device identity; underpins host ExternalId parity.
   files: `IeeeAddress.cs`

3. **SLIP framing codec** `[depends: 1]`
   Encode/decode SLIP frame boundaries + byte-stuffing over a raw byte stream, including
   partial-read frame accumulation. Encoder and decoder as separate small classes.
   criteria: deCONZ serial frame codec (SLIP).
   files: `Serial/SlipEncoder.cs`, `Serial/SlipDecoder.cs`

4. **deCONZ CRC** `[depends: 1]`
   Compute/validate the deCONZ frame checksum. Pure.
   criteria: deCONZ serial frame codec (CRC).
   files: `Serial/DeconzCrc.cs`

5. **ZCL frame header codec** `[depends: 1]`
   Encode/decode ZCL frame-control + sequence + command-id header. Pure.
   criteria: ZCL frame header encode/decode.
   files: `Zcl/ZclFrameHeader.cs`

## Phase 2 — deCONZ command/response frame codecs (all parallel after #3, #4)

Each is a data-only frame record + its encoder/decoder, tested against known byte fixtures.

6. **Read/Write firmware-parameter frames** `[depends: 3, 4]`
   files: `Serial/Frames/ReadParameterFrame.cs`, `Serial/Frames/WriteParameterFrame.cs`
7. **Device-state frame** `[depends: 3, 4]` — query flags (APS data indication/confirm available)
   files: `Serial/Frames/DeviceStateFrame.cs`
8. **Change-network-state frame** `[depends: 3, 4]` — network state / permit-join
   files: `Serial/Frames/ChangeNetworkStateFrame.cs`
9. **APS data-request frame (send)** `[depends: 3, 4]`
   files: `Serial/Frames/ApsDataRequestFrame.cs`
10. **APS data-indication frame (receive)** `[depends: 3, 4]`
    files: `Serial/Frames/ApsDataIndicationFrame.cs`
11. **APS data-confirm frame** `[depends: 3, 4]`
    files: `Serial/Frames/ApsDataConfirmFrame.cs`

criteria for 6–11: deCONZ command/response frames (read/write param, device state,
change network state/permit-join, APS request/indication/confirm).

## Phase 3 — ZCL & ZDP payload codecs (parallel; disjoint files)

12. **ZCL attribute-report / read-response parser** `[depends: 5]`
    Decode Report-Attributes (0x0a) and Read-Attributes-Response (0x01) into
    `(attributeId, typed value)` list. Pure.
    criteria: receive attribute reports/reads from any cluster (generic).
    files: `Zcl/ZclAttributeReportParser.cs`, `Zcl/ZclAttributeValue.cs`
13. **ZCL generic-command builder** `[depends: 5]`
    Build outbound ZCL command payload for arbitrary cluster/command/payload.
    criteria: send a ZCL command generically.
    files: `Zcl/ZclCommandBuilder.cs`
14. **ZCL read-attributes request builder** `[depends: 5]`
    Build Read-Attributes (0x00) request for given attribute ids (Basic-cluster read).
    criteria: interview Basic-cluster read; already-joined backfill (risk #2).
    files: `Zcl/ZclReadAttributesRequest.cs`
15. **ZDP active-endpoints request/response** `[depends: 1]`
    files: `Zdp/ActiveEndpointsRequest.cs`
16. **ZDP simple-descriptor request/response** `[depends: 1]`
    Parse endpoint id, profile id, device id, in/out cluster lists.
    files: `Zdp/SimpleDescriptorRequest.cs`
17. **ZDP device-announce parser** `[depends: 2]`
    Parse device-announce (nwkAddr + IEEE).
    files: `Zdp/DeviceAnnounceParser.cs`
18. **ZDP node-descriptor request/response** `[depends: 1]`
    files: `Zdp/NodeDescriptorRequest.cs`

criteria for 15–18: ZDP interview (active-endpoint + simple-descriptor discovery, announce).

## Phase 4 — transport seam + framed channel

19. **Serial transport seam** `[depends: 1]`
    `ISerialTransport` port (open/close/read/write raw bytes); thin `SerialPortTransport`
    real adapter over `System.IO.Ports` (the seam — not unit-tested); `FakeSerialTransport`
    in-memory double in test support (feed bytes in, capture bytes out).
    criteria: connect over a configured serial port (seam for all tests).
    files: `Transport/ISerialTransport.cs`, `Transport/SerialPortTransport.cs`,
    `tests/.../Transport/FakeSerialTransport.cs`
20. **Framed deCONZ command channel** `[depends: 19, 3, 4]`
    Write a deCONZ command frame (SLIP+CRC encode) to transport and read framed responses
    back, correlating by sequence number; surface decoded response frames. Lifecycle/IO only,
    no protocol semantics.
    criteria: handshake + all command/response exchange plumbing.
    files: `Connection/DeconzChannel.cs`, `Connection/FrameReader.cs`

## Phase 5 — connection & APS plumbing

21. **Connect handshake + read network config (no re-form)** `[depends: 20, 6]`
    Open channel, read firmware/network parameters to confirm a coordinator and read back
    existing PAN/channel/MAC without re-forming the network; expose connected state + config.
    criteria: connect + read existing network config without re-forming.
    files: `Connection/DeconzConnection.cs`, `Coordinator/NetworkConfig.cs`
22. **APS receive poll loop** `[depends: 20, 7, 10, 11]`
    Poll device-state; when the data-indication flag is set, issue read-received-data and
    emit APS data-indication payloads; drain confirms. Lifecycle only; raises data records.
    criteria: receive path for attribute reports + ZDP responses.
    files: `Connection/ApsPollLoop.cs`, `Connection/ApsIndicationReceived.cs`
23. **APS send** `[depends: 20, 9, 11]`
    Send an APS data-request; track its confirm.
    criteria: send path for ZCL commands + ZDP requests.
    files: `Connection/ApsSender.cs`

## Phase 6 — coordinator capabilities

24. **Read known-device list** `[depends: 21, 15, 16, 2]`
    Read coordinator's known-device table → `ZigbeeDevice` (ieee, nwkAddr, endpoints via
    active-endpoints/simple-descriptor). Raw shape only.
    criteria: read back already-joined device list; return device list on request.
    files: `Coordinator/KnownDeviceTable.cs`, `ZigbeeDevice.cs`, `ZigbeeEndpoint.cs`
25. **Permit-join controller** `[depends: 20, 8, 6]`
    Enable/disable permit-join via change-network-state / write-parameter.
    criteria: enable/disable permit-join.
    files: `Coordinator/PermitJoinController.cs`
26. **Generic ZCL send** `[depends: 23, 13]`
    `SendCommandAsync(ZigbeeCommandRequest)` → build ZCL command → APS data-request.
    criteria: send a ZCL command generically.
    files: `Coordinator/CommandSender.cs`, `ZigbeeCommandRequest.cs`
27. **Attribute-report listener → event** `[depends: 22, 12, 2]`
    Subscribe APS receive, parse ZCL attribute reports, raise generic `ZigbeeAttributeReport`
    (endpoint, cluster, attribute, value). No sensor semantics.
    criteria: raise generic attribute-report event.
    files: `Coordinator/AttributeReportListener.cs`, `ZigbeeAttributeReport.cs`
28. **Device-interview orchestration → event** `[depends: 22, 23, 15, 16, 17, 14, 12, 2]`
    On device-announce during permit-join: active-endpoints → simple-descriptor per endpoint →
    Basic-cluster (0x0000) read of ManufacturerName(0x0004)/ModelIdentifier(0x0005) → raise
    `ZigbeeDeviceJoined` with raw info. No vendor/model→DeviceType mapping.
    criteria: on new join, run ZDP interview and raise protocol-level "device joined".
    files: `Coordinator/DeviceInterview.cs`, `ZigbeeDeviceJoined.cs`

## Phase 7 — public façade

29. **`IZigbeeCoordinator` façade + DI** `[depends: 21, 24, 25, 26, 27, 28]`
    Compose capabilities into the agreed API: `ConnectAsync`, `GetDevicesAsync`,
    `SetPermitJoinAsync`, `SendCommandAsync`, `DeviceJoined`/`AttributeReported` events,
    connection-status property; `AddHausZigbee` DI registration binding serial-port config.
    criteria: entire public façade surface.
    files: `IZigbeeCoordinator.cs`, `ZigbeeCoordinator.cs`, `ServiceCollectionExtensions.cs`

---

## Phase 8 — Host restructure (`Haus.Zigbee.Host`)

Mostly sequential: several increments touch `ServiceCollectionExtensions.cs` / the relay.
The mapper reshapes (32,33,34,36) are file-disjoint and parallel once #31 lands.

30. **Reference library, rename folder, serial config** `[depends: 29]`
    Add `Haus.Zigbee` project reference; `git mv Zigbee2Mqtt/ → Zigbee/`; replace
    `Zigbee2MqttConfiguration`/`ZigbeeOptions` (MQTT-broker shape) with serial-port config
    (`SerialPort: "/dev/ttyACM0"`); register `AddHausZigbee`. Structural — touches many files
    once, so it goes first.
    criteria: host uses `Haus.Zigbee` façade; config is serial-port shaped.
    files: `Haus.Zigbee.Host.csproj`, `ServiceCollectionExtensions.cs`, `Zigbee/**` (moved),
    `appsettings.json`, new `Zigbee/Configuration/SerialOptions.cs`
31. **ExternalId ↔ IeeeAddress map** `[depends: 30]` — HIGHEST-RISK parity item (#1)
    Derive Haus `ExternalId` in the legacy `friendly_name` (`0x`+IEEE) format from an
    `IeeeAddress`, and maintain an `ExternalId → address` map for outbound command addressing
    (previously implicit in the MQTT topic).
    criteria: paired devices keep stable ExternalId; outbound commands resolve to an address.
    files: `Zigbee/ExternalIdMap.cs`
32. **Inbound: device list → DeviceDiscoveredEvents** `[depends: 31]` — `[independent]` of 33,34,36
    Reshape `DevicesMapper` to consume `GetDevicesAsync` results, resolve DeviceType via
    `DeviceTypeResolver` from raw Basic attributes, ExternalId from #31.
    criteria: sync device list to Haus (contract unchanged).
    files: `Zigbee/Mappers/ToHaus/DevicesMapper.cs`
33. **Inbound: device joined → DeviceDiscoveredEvent** `[depends: 31]` — `[independent]` of 32,34,36
    Reshape `InterviewSuccessfulMapper` to consume `ZigbeeDeviceJoined`.
    criteria: new join surfaces to Haus with resolved DeviceType.
    files: `Zigbee/Mappers/ToHaus/DeviceJoinedMapper.cs`
34. **Inbound: attribute report → sensor changed events** `[depends: 31]` — `[independent]` of 32,33,36
    Reshape `DeviceEventMapper` + `SensorChangedMapper` + Battery/Illuminance/Occupancy/
    Temperature sub-mappers to translate `ZigbeeAttributeReport` (cluster/attribute/value) into
    Haus sensor events. The illuminance/occupancy/temperature/battery semantics correctly live
    HERE, not in the library.
    criteria: sensor state reaches Haus (contract unchanged).
    files: `Zigbee/Mappers/ToHaus/DeviceEvents/*.cs`
35. **Outbound: discovery → permit-join / get-devices** `[depends: 30]` — `[independent]` of 32,33,34,36
    Reshape `HausDiscoveryToZigbeeMapper`: Start/Stop → `SetPermitJoinAsync`; Sync →
    `GetDevicesAsync`.
    criteria: discovery commands drive the coordinator.
    files: `Zigbee/Mappers/ToZigbee/HausDiscoveryToZigbeeMapper.cs`
36. **Outbound: lighting → generic ZCL send** `[depends: 31]` — `[independent]` of 32,33,34,35
    Reshape `HausLightingToZigbeeMapper`: `DeviceLightingChangedEvent` → `ZigbeeCommandRequest`
    (resolve ExternalId→address via #31; on/off + level + color as ZCL commands).
    criteria: lighting commands reach the device.
    files: `Zigbee/Mappers/ToZigbee/HausLightingToZigbeeMapper.cs`
37. **Backfill vendor/model for already-joined devices** `[depends: 32, 26]` — risk #2
    On connect, for each already-known device, trigger a Basic-cluster (0x0000) read via
    `SendCommandAsync` and feed the returned attributes into `DeviceTypeResolver` (interview
    only covers NEW devices).
    criteria: already-joined devices resolve a DeviceType, not Unknown.
    files: `Zigbee/Services/DeviceBackfillService.cs`
38. **Relay rewrite** `[depends: 32, 33, 34, 35, 36]`
    `ZigbeeToHausRelay`: subscribe coordinator `DeviceJoined`/`AttributeReported` → inbound
    mappers → publish to Haus MQTT; subscribe Haus MQTT commands → outbound mappers →
    coordinator calls. Drop the second (zigbee) MQTT client and topic-routing `MqttMessageMapper`.
    Keep connection-lifecycle, decoding, and mapping as separate collaborators (style).
    criteria: MQTT contract to Haus.Web.Host unchanged; source/sink is now the façade.
    files: `Zigbee/Services/ZigbeeToHausRelay.cs`, `Zigbee/Mappers/MqttMessageMapper.cs`,
    `Zigbee/Mqtt/MqttClientFactory.cs`
39. **Health check reflects coordinator connection** `[depends: 30]`
    Replace `Zigbee2MqttHealthCheck` (always-Healthy) with one reading `IZigbeeCoordinator`
    status. Shares `ServiceCollectionExtensions.cs` → sequence, not parallel with 30/38/40.
    criteria: health check reflects real coordinator connection state.
    files: `Zigbee/Health/ZigbeeCoordinatorHealthCheck.cs`, `ServiceCollectionExtensions.cs`
40. **Remove z2m plumbing + finalize DI** `[depends: 38, 39, 37]`
    Delete `Zigbee2MqttMessage`/`Meta`, `Zigbee2MqttMessageFactory`, z2m routing, second MQTT
    client, `Zigbee2MqttConfiguration`/`ZigbeeOptions`; finalize `ServiceCollectionExtensions`.
    criteria: zigbee2mqtt-broker plumbing fully removed.
    files: `ServiceCollectionExtensions.cs`, deletions across `Zigbee/`

---

## Phase 9 — deployment

41. **docker-compose: drop zigbee2mqtt, grant serial access** `[depends: 40]`
    Remove `zigbee2mqtt` service + `configuration.yaml` volume from `docker-compose.yml`; add
    `devices: /dev/ttyACM0` + serial env to `haus_zigbee`; drop `Zigbee__Config__Mqtt` env;
    reconcile `Haus.slnx` `configuration.yaml` reference.
    criteria: zigbee2mqtt service + configuration.yaml removed; haus_zigbee gets serial access.
    files: `docker-compose.yml`, `docker-compose.local.yml`, `configuration.yaml`, `Haus.slnx`
42. **Simulated deCONZ endpoint for CI/local + outer acceptance test** `[depends: 29, 40]`
    — NEEDS A DESIGN DECISION BEFORE STARTING (see below)
    Provide a simulated deCONZ coordinator so CI/local (no hardware) can exercise the full
    stack, and add/adjust the outer acceptance test that drives discovery + a command flow
    through the real stack against it.
    criteria: CI/local runs against a simulated deCONZ endpoint.
    files: TBD by design (simulator wiring + `docker-compose.local.yml` + CI setup action)

---

## Parallelism summary (what the orchestrator can fan out)
- After #1: {2, 3, 4, 5} in parallel.
- After #3,#4: {6, 7, 8, 9, 10, 11} in parallel (six frame codecs).
- After #5 / #1 / #2: {12, 13, 14} and {15, 16, 17, 18} in parallel (ZCL + ZDP payloads).
- #19 can run in parallel with all of Phases 1–3 (only depends on #1).
- Phase 5/6 capabilities join on their specific deps; {24, 25, 26, 27, 28} largely parallel
  once their inputs exist (disjoint files, but heavy shared dep on the channel — check deps).
- Host: after #31, {32, 33, 34, 36} parallel; #35 parallel after #30. #38/#39/#40 serialize
  on `ServiceCollectionExtensions.cs` / the relay.

## Needs design before implementation
- **Increment 42 only.** The architecture note nails the seam (`ISerialTransport`) but not the
  CI/local **simulation mechanism**: whether the acceptance stack points `SerialPortTransport`
  at a simulated serial device (e.g. a pty/socat-backed pseudo-terminal in Docker) or swaps
  `ISerialTransport` for a networked fake deCONZ endpoint via config. This is a genuine
  structural decision — recommend dispatching `craft-architect` for a short spike on the sim
  endpoint before increment 42, and authoring the outer acceptance test (craft:acceptance-testing)
  once that mechanism exists. Everything else (1–41) is fully specified by the agreed design.
- No UI design needed — this change is headless (backend + protocol only).
