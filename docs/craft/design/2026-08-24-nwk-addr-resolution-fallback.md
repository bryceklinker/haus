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

**Revised after fresh-eyes review** (see "Not blocking the shared MQTT pump" below): the resolve
does *not* run inline on the command that discovered the stale address. `HandleLightingAsync`
still drops straight to `HandleMissingNetworkAddressAsync` (added by PR #66, unchanged) when
`device.NetworkAddress` is null, but now *also* fires the resolution off as a detached background
task so the *next* command for that device succeeds instead:

```
if device.NetworkAddress is null:
    HandleMissingNetworkAddressAsync(device)   // unchanged: log warning + ZigbeeCommandDroppedEvent
    TriggerBackgroundResolve(device)           // detached, deduped per ExternalId, doesn't block this call
    return
// device.NetworkAddress present -> proceed exactly as today
```

`TriggerBackgroundResolve` dedupes on `DeviceModel.ExternalId` via a `ConcurrentDictionary` so
repeated commands for the same still-unresolved device (e.g. several lighting commands queued up
right after a restart) don't each fire their own broadcast — only one resolution is in flight per
device at a time.

`ResolveNetworkAddressAsync(DeviceModel device, token)` (now `void`-returning, called only from the
background path):
1. `ExternalIdConverter.TryParseAddress(device.ExternalId, ...)` — mirrors the converter already
   used elsewhere in this codebase; an unparseable `ExternalId` is a no-op.
2. `coordinator.ResolveNetworkAddressAsync(ieeeAddress, token)`; a timeout/no-answer is also a
   no-op — the device stays unresolved until the next dropped command retriggers this.
3. On success: `addressRegistry.Register(...)` (same registry `SyncDevicesAsync` populates), then
   publish `DeviceDiscoveredEvent` so `Haus.Web.Host`'s existing
   `DeviceDiscoveredEventHandler`/`DeviceEntity.UpdateFromDiscoveredDevice` path persists the
   resolved `NetworkAddress` — no new DB-write path needed, this event already updates an existing
   `DeviceEntity` in place.

### Not blocking the shared MQTT pump

The original version of this design had `HandleLightingAsync` `await` the resolve inline and send
the original command using the resolved address in the same call. A fresh-eyes review caught that
this runs inside `HausMqttClient`'s single serialized message handler
(`MqttMessageHandler` → each subscription's `ExecuteAsync`, awaited via `Task.WhenAll` before the
managed MQTT client dispatches the next message) — so a stale-address command would block *every*
other incoming MQTT command/message for up to the resolver's 30s timeout. Firing the resolution
off detached (not awaited by `HandleCommandAsync`) removes that blocking without shortening the
timeout: the current command still drops immediately (same visible behavior as before this whole
feature — PR #66's drop-and-log), and the resolved address becomes available for the *next*
command via the same `DeviceDiscoveredEvent` → persisted `DeviceEntity.NetworkAddress` path.

### RequestId collision (re-checked, not fixed here)

`ApsSender._pendingConfirms` correlates every outstanding APS confirm by a single `byte RequestId`
across *all* callers of `ApsSender.SendAsync` (`CommandSender`, `DeviceInterview`, and now
`NetworkAddressResolver`), but each caller owns an *independent* `ByteSequenceCounter` for that
field (see the explicit comment in `CommandSender`: "the ZCL transaction sequence number and the
APS request id are distinct concerns, so each layer owns its own counter here"). Two concurrent
in-flight sends from different collaborators can in principle land on the same byte value and
overwrite each other's pending-confirm entry. This is a pre-existing property of the codebase
(already shared between `CommandSender` and `DeviceInterview` before this PR); `NetworkAddressResolver`
becomes a third participant in the same shared keyspace but does not introduce a new failure mode.
Centralizing RequestId issuance in `ApsSender` would close this properly, but touches two
established collaborators outside this PR's scope — left as a follow-up rather than folded in here.
The new device-backfill sweep (below) mitigates its own added exposure by resolving devices
sequentially rather than firing many broadcasts concurrently at startup.

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
(Step 4 in the original plan — "return the resolved address so the caller proceeds to send the
original command in the same handler invocation" — no longer applies; see "Not blocking the shared
MQTT pump" above. The background resolve's only remaining job is steps 1-3.)

`HandleMissingNetworkAddressAsync` is untouched — diagnostics visibility from PR #66 is preserved
verbatim for the genuine-timeout / unparseable-IEEE case.

## Startup sweep: resolving already-known devices proactively

Added scope beyond the original request: rather than waiting for a lighting command to discover a
stale address reactively, `DeviceBackfillService` — already invoked once per connect, off the
connect hot path (`ZigbeeHausBridge.TryConnectAsync` → detached `BackfillSafelyAsync`) — now also
resolves each already-known device's `NetworkAddress` before re-reading its Basic cluster, reusing
`IZigbeeCoordinator.ResolveNetworkAddressAsync` (no new resolution logic; this is the same
`NetworkAddressResolver` the reactive path uses).

`BackfillDeviceAsync` per device, in order (unchanged: `BackfillAsync` iterates sequentially, not
concurrently — see the RequestId-collision note above for why that matters):
1. `coordinator.ResolveNetworkAddressAsync(device.IeeeAddress, token) ?? device.NetworkAddress` — a
   successful resolve also updates `KnownDeviceTable` as a side effect (`NetworkAddressResolver`'s
   existing behavior), so the subsequent `ReadDeviceInfoAsync` call (which looks the device back up
   in `KnownDeviceTable` by IEEE address) automatically uses the freshened address too. No answer
   falls back to the address already on record, exactly like `NetworkAddressToReturn: null` in the
   reactive path — best-effort, not a hard failure.
2. `ReadDeviceInfoAsync` and the publish-if-classified logic are unchanged from the existing
   backfill behavior; DeviceType/Metadata still come from a fresh Basic-cluster read (not "Unknown"
   overwriting a known device), so the same landmine documented above for the reactive path doesn't
   apply here either — this path was already safe against it.

Correction to the RequestId-collision note above: "resolving devices sequentially" only guarantees
no two broadcasts are in flight *within the sweep itself*. It does not prevent the sweep's resolve
for one device from overlapping a *reactive* background resolve (`ZigbeeOutboundRelay`'s
`TriggerBackgroundResolve`) for a different device racing in from a queued MQTT command at the same
time — those two collaborators don't share a dedupe set. That overlap is a redundant broadcast at
worst (both still go through `NetworkAddressResolver`'s single `Interlocked`-based counters, so
their transaction sequence numbers and request IDs never collide with each other), not a
correctness problem, so it's left as-is rather than adding a second dedupe layer across collaborators.

### Second fresh-eyes review pass: lost-update race in `KnownDeviceTable` (fixed)

A second review round, run after the pump-blocking/IEEE-verification/RequestId findings above were
already addressed, found one more real gap: `NetworkAddressResolver.RecordResolvedAddress` did a
`TryGet` (read the existing entry's `Endpoints`) followed by a separate `AddOrUpdate` (write a
rebuilt `ZigbeeDevice`) — two independent `ConcurrentDictionary` operations, not one atomic step.
`DeviceInterview.InterviewAsync` does its own unsynchronized `AddOrUpdate` with a freshly-discovered
endpoint list. If a device's own re-announce interview completed *between* the resolver's `TryGet`
and its `AddOrUpdate` for that same device, the resolver's write would silently clobber the
interview's newly-discovered endpoints with the stale ones it read a moment earlier. The resolved
*address* was always correct either way (the resolver only ever writes the value it just verified),
but endpoints could be lost — and the new startup sweep meaningfully raises the odds of exactly
this interleaving, since it proactively resolves every known device right at connect, precisely
when devices are most likely to be re-announcing after the same restart.

Fixed by giving `KnownDeviceTable` an atomic `UpdateNetworkAddress(ieeeAddress, networkAddress)`
method built on `ConcurrentDictionary.AddOrUpdate`'s factory overload — the read of the existing
entry and the write of the updated one now happen as a single dictionary operation (retried against
the latest value on contention, per `ConcurrentDictionary`'s documented semantics), so there is no
window between them for a concurrent `AddOrUpdate` to land in. `NetworkAddressResolver` now calls
this instead of doing its own `TryGet` + `AddOrUpdate`. `DeviceInterview.InterviewAsync` still does
a plain `AddOrUpdate` for its own full-device write (network address, endpoints, and everything
else it just discovered) — that write is intentionally the fresher, complete replacement in this
race, and is unaffected by this fix.

Why fold this into `DeviceBackfillService` instead of a new collaborator: a separate sweep that
resolved-then-published its own `DeviceDiscoveredEvent` would need its own answer to "what
DeviceType do I publish," and `ZigbeeDevice` (from `KnownDeviceTable`) carries no DeviceType at
all — reusing `DeviceBackfillService`'s existing Basic-cluster-read-then-classify-then-publish flow
avoids reinventing that safeguard.

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
