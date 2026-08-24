# Design note — ZDP NWK_addr_req resolution fallback for stale NetworkAddress (issue #9)

Status: architecture decided, ready for implementation.
Scope basis: locked requirements from the work request (see below); do not re-litigate.
Primary protocol reference: Zigbee spec **05-3474-23-csg** (CSA IoT), ZDP command reference —
`NWK_addr_req` / `NWK_addr_rsp` (cluster 0x0000 request / 0x8000 response, ZDP profile 0x0000).
Same reference class already cited by the deCONZ design note.

## Problem (restated)

`KnownDeviceTable` is rebuilt empty on every `Haus.Zigbee.Host` restart and is only populated by
`DeviceInterview.InterviewAsync`, which fires solely on a live ZDP Device Announce. A previously
paired device that hasn't re-announced since the last restart has `DeviceEntity.NetworkAddress ==
null`, so `ZigbeeOutboundRelay.HandleLightingAsync` silently (now visibly, via
`ZigbeeCommandDroppedEvent`) drops every lighting command to it. The deCONZ serial protocol has no
bulk device-table read, so there's nothing to hydrate from at connect time — the fix has to be a
per-command, on-demand resolution.

## Wire format (NWK_addr_req / NWK_addr_rsp)

Mirrors this codebase's existing ZDP codec shape (`Zdp/ActiveEndpointsRequest.cs`,
`Zdp/SimpleDescriptorRequest.cs`): a leading `TransactionSequenceNumber` byte (the ZDP header byte
every request/response in this codebase already includes), then the cluster-specific payload,
all multi-byte fields little-endian.

**Request** (cluster `0x0000`, 11 bytes):
| Field | Bytes | Notes |
|---|---|---|
| TransactionSequenceNumber | 1 | |
| IEEEAddr | 8 | target device's 64-bit IEEE address, LE |
| RequestType | 1 | `0x00` = Single Device Response (only value this codebase needs — we only want the one NWK address, never the associated-device list) |
| StartIndex | 1 | only meaningful for Extended Response; send `0x00` |

**Response** (cluster `0x8000`, ≥12 bytes on success):
| Field | Bytes | Notes |
|---|---|---|
| TransactionSequenceNumber | 1 | echoes the request |
| Status | 1 | `ZdoStatus` (existing enum) |
| IEEEAddrRemoteDevice | 8 | LE |
| NWKAddrRemoteDevice | 2 | LE — the address we want |
| NumAssocDev / StartIndex / AssocDevList | variable, optional | only present for Extended Response; a Single Device Response request must not receive them, but decode defensively (ignore trailing bytes) in case a stack sends them anyway |

Decode returns `null` on a too-short payload, matching `ActiveEndpointsResponseCodec`'s existing
defensive-truncation convention (a malformed frame from a real device must never throw and stop
delivery to the other `IndicationReceived` subscribers).

Request is sent as an APS broadcast to `0xFFFF` ("all devices, including sleeping end devices" —
NWK broadcast address table, spec §3.6.5), ZDP endpoint `0x00` on both ends, profile `0x0000` —
identical addressing shape to `DeviceInterview.SendZdpAsync`, just to a broadcast destination
instead of a known unicast NWK address.

## Where the logic lives

New class `Haus.Zigbee/Coordinator/NetworkAddressResolver.cs`, sibling to `DeviceInterview`, not a
method bolted onto it. `DeviceInterview` owns "orchestrate everything that happens when a device
joins"; this is a narrower, single job ("given an IEEE address, ask the network for its current
NWK address") with a different correlation shape (broadcast request → no NWK address known until
the response arrives, so it cannot key on `(NetworkAddress, ClusterId, SequenceNumber)` the way
`DeviceInterview.ResponseKey` does — only `SequenceNumber` is known up front). A separate small
collaborator keeps that difference from leaking into `DeviceInterview` and keeps each class's
correlation model simple, matching [[feedback_architecture_style]] (avoid god-like classes,
separate event data from handling) and the existing pattern of small focused collaborators wired
into `TransportComponents` (`PermitJoinController`, `AttributeReportListener`, `DeviceInterview`
are all already separate, single-purpose classes).

```csharp
public class NetworkAddressResolver : IDisposable
{
    // ApsPollLoop.IndicationReceived, ApsSender, KnownDeviceTable — same collaborators
    // DeviceInterview already takes.
    public Task<ushort?> ResolveAsync(IeeeAddress ieeeAddress, CancellationToken token);
}
```

- Registers a `TaskCompletionSource<ApsDataIndicationFrame>` keyed by its own ZDP transaction
  sequence number (own `ByteSequenceCounter`, own dictionary — independent of `DeviceInterview`'s).
- On `IndicationReceived`, claims the indication only when `ProfileId == 0x0000 && ClusterId ==
  0x8000` (NWK_addr_rsp) and the decoded transaction sequence number matches a pending entry;
  otherwise ignores it. `AttributeReportListener` and `DeviceInterview` already coexist as
  independent subscribers on this same shared event, each filtering to what they own — this is a
  third, following the same convention, not a new pattern.
- Awaits the pending response bounded by a timeout (constructor param, default matching
  `DeviceInterview.DefaultResponseTimeout` = 30s — this is the same weight of ZDP round trip),
  using the same `CancellationTokenSource` + `WaitAsync(linked.Token)` shape as
  `DeviceInterview.SendAsync` / `ApsSender.SendAsync`.
- **Catches its own timeout internally and returns `null`** rather than throwing
  `OperationCanceledException` out to the caller. This is a deliberate departure from
  `DeviceInterview.SendAsync`/`ApsSender.SendAsync` (which do throw on timeout): those calls sit
  mid-interview where a timeout aborts the whole interview (exception is the right signal).
  Here, "no answer" is an expected, first-class outcome the caller (`ZigbeeOutboundRelay`) branches
  on to fall back to today's drop-and-log behavior — a `Task<ushort?>` result reads as intent
  directly at the call site instead of `try/catch` used for control flow.
- On success (`Status == Success`), updates `KnownDeviceTable` — preserving any already-known
  endpoints for that IEEE address (`TryGet` first; empty list if none) — the same
  "resolve → update the table" step `DeviceInterview.InterviewAsync` already performs.

`IZigbeeCoordinator` gains:
```csharp
Task<ushort?> ResolveNetworkAddressAsync(IeeeAddress ieeeAddress, CancellationToken token);
```
`ZigbeeCoordinator` delegates to `CurrentComponents().NetworkAddressResolver.ResolveAsync(...)`.
`TransportComponents` gains a `NetworkAddressResolver` field, built in `BuildTransportComponents`
and disposed in `DisposeComponents`, exactly like `DeviceInterview` today.

## Wiring in `ZigbeeOutboundRelay`

`HandleLightingAsync` currently drops straight to `HandleMissingNetworkAddressAsync` (added by
PR #66, unchanged) when `device.NetworkAddress` is null. New flow:

```
networkAddress = device.NetworkAddress ?? await ResolveNetworkAddressAsync(device, token)
null  -> HandleMissingNetworkAddressAsync(device)   // unchanged: log warning + ZigbeeCommandDroppedEvent
value -> proceed exactly as today, using the resolved address
```

`ResolveNetworkAddressAsync(DeviceModel device, token)`:
1. `ExternalIdConverter.TryParseAddress(device.ExternalId, ...)` — mirrors the converter already
   used elsewhere in this codebase; an unparseable `ExternalId` resolves to `null` (falls back to
   drop, same as a resolution timeout).
2. `coordinator.ResolveNetworkAddressAsync(ieeeAddress, token)`.
3. On success: `addressRegistry.Register(...)` (same registry `SyncDevicesAsync` populates), then
   publish `DeviceDiscoveredEvent` so `Haus.Web.Host`'s existing
   `DeviceDiscoveredEventHandler`/`DeviceEntity.UpdateFromDiscoveredDevice` path persists the
   resolved `NetworkAddress` — no new DB-write path needed, this event already updates an existing
   `DeviceEntity` in place.

**Important correctness point found during this design pass:** `DeviceDiscoveredEvent` carries
`DeviceType`, and `DeviceEntity.UpdateFromDiscoveredDevice` **unconditionally overwrites**
`DeviceType` (and derives `LightType` from it) on every event, no merge. `DevicesMapper` (used by
the full discovery sync path) always sets `DeviceType.Unknown` because at that call site the
coordinator genuinely doesn't know the type. This fallback path is different: it fires on *every*
lighting command to a stale device, and the triggering MQTT message already carries the device's
current `DeviceModel` — including its already-classified `DeviceType`, `LightType`-implying
metadata, and `Metadata`. Publishing `DeviceType.Unknown` here would silently reclassify an
already-known light back to Unknown on the very first stale-address command after every restart —
a regression this fix must not introduce. So the published event must carry `device.DeviceType`
and `device.Metadata` from the inbound command's `DeviceModel`, not `Unknown`:
```csharp
new DeviceDiscoveredEvent(device.ExternalId, device.DeviceType, device.Metadata, networkAddress)
```
4. Return the resolved address so the caller proceeds to send the original lighting command in the
   same handler invocation (no second round trip through MQTT).

`HandleMissingNetworkAddressAsync` is untouched — diagnostics visibility from PR #66 is preserved
verbatim for the genuine-timeout / unparseable-IEEE case.

## Test seams (no real hardware, no real DB — per hard constraint)

- **`Haus.Zigbee.Tests`**: `NwkAddrRequestCodec`/`NwkAddrResponseCodec` encode/decode round-trips
  and truncation handling (mirror `ActiveEndpointsResponseCodec`'s test shape). `NetworkAddressResolver`
  tests reuse `FakeDeconzDongle`/`FakeSerialTransport` exactly like `DeviceInterviewTests` — inject
  an `IndicationBody` on cluster `0x8000` released after the broadcast request is sent, plus a
  timeout case with no `ReleaseAfterSend` registered (mirrors
  `WhenTheActiveEndpointsRequestNeverReceivesAResponseThenTheInterviewAbortsWithoutJoiningTheDevice`).
- **`Haus.Zigbee.Host.Tests`**: extend `FakeZigbeeCoordinator` with a scriptable
  `ResolveNetworkAddressAsync` (e.g. `NetworkAddressToReturn`, recorded call list) — same pattern
  as its existing `DeviceInfoToReturn`/`ConfirmToReturn`. New `ZigbeeOutboundRelayTests` cases:
  resolve-success sends the original command using the resolved address and publishes
  `DeviceDiscoveredEvent` with the *original* `DeviceType`/`Metadata` (not `Unknown`); resolve-null
  falls back to the existing drop+`ZigbeeCommandDroppedEvent` behavior unchanged.
- Both suites run entirely against the existing fake serial transport / in-memory fakes — no serial
  port, no MQTT broker, no EF/database involved in either project's tests.
