# Design note — Sleepy/battery end-device-aware outbound delivery (issue #81)

Status: architecture scoped from investigation (including the herdsman/Zigbee2MQTT parity
comparison issue #81 explicitly asked for) — ready for intake/planning/implementation. The
three open design questions below (TTL/expiry, retry-path consolidation, Node Descriptor scope)
have been decided (2026-10-09) and are recorded under "Decisions" rather than left open.
Scope basis: issue #81 (split from #77, which stays scoped to the core per-device
queue/retry manager, delivered in PR #85). Investigation conducted 2026-10-09 against the
current `Haus.Zigbee`/`Haus.Zigbee.Host` source plus a cross-vendor comparison against
zigbee-herdsman/Zigbee2MQTT and the `com.zsmartsystems.zigbee` reference cited by #77.

## Problem (restated)

#77 gave every outbound Zigbee command a per-device queue and configurable backoff retry,
but a sleepy/battery end device that is radio-off between polls doesn't fail delivery the
way a mains-powered device does — it simply isn't listening, so a raw immediate retry is
likely to miss the same wake window the original send missed. #81 asks whether Haus can
detect that a device is sleepy and, if so, hold a command and replay it on the device's
next wake instead of retrying blindly.

## Current state in Haus (from investigation)

1. **deCONZ exposes no wake/poll-timing push signal, but the capability flag IS available on
   demand.** The implemented frame set (`src/Haus.Zigbee/Serial/Frames/`) covers APS-DATA
   confirm/indication/request, the coordinator's own `DeviceStateFrame`, and generic firmware
   parameter get/set. There's no frame for a poll timer or "about to wake" callback, and no ZDP
   Node Descriptor Request/Response (`0x0002`/`0x8002`) is implemented in
   `src/Haus.Zigbee/Zdp/` today — but that ZDP message (which deCONZ's protocol does carry) is
   exactly where the rx-on-when-idle MAC capability bit lives, so it's addable, not missing from
   the wire.

2. **The sleepy capability bit is parsed once at join, then dropped.**
   `DeviceAnnounceParser.cs` decodes the Device_annce MAC Capability byte into
   `DeviceAnnounce.Capabilities` (bit 3 = rx-on-when-idle). `DeviceInterview.InterviewAsync` reads
   `announce.Capabilities` off the parsed announce but never forwards it: neither `ZigbeeDevice`
   nor `ZigbeeDeviceJoined` carry a capability field, and nothing downstream (`KnownDeviceTable`,
   `ZigbeeKnownDeviceModel`, `DeviceEntity`) persists it. `DeviceEntity.BatteryLevel` is an
   unrelated sensor reading (battery percentage), not this flag. `LastSeenAt` exists but is
   diagnostics-only (issue #48's activity feed) and isn't consulted on any send path today.

3. **#77 already built the plug point this needs.** Every outbound command flows through
   `CommandSender.SendCommandAsync` → `DeviceCommandQueue.EnqueueAsync` (per-device,
   `SemaphoreSlim`-serialized, keyed by `IeeeAddress`/short address) →
   `CommandRetryHandler.ExecuteWithRetryAsync` (pure exponential backoff + jitter, device-blind).
   `CommandSender.SendCommandAsync` is the one place that already knows which device a command
   targets before it's queued — the natural branch point for a sleepy-aware policy.

4. **A second, independent retry path exists above the coordinator.**
   `ZigbeeOutboundRelay.SendWithApsAckRetryAsync` (lighting commands specifically) escalates to
   APS-ACK on first failure, bypassing `CommandRetryHandler`'s backoff entirely. Any sleepy-aware
   change at the coordinator layer won't be visible to this path unless it's addressed too.

## Herdsman / Zigbee2MQTT parity comparison (confirmed 2026-10-09)

Parity is closer than #77 flagged as "unconfirmed" — herdsman does hold-and-replay, not raw
retry:

- **Sleepy detection**: herdsman does **not** use the Node Descriptor `rxOnWhenIdle` bit either —
  `device.ts`'s `updateNodeDescriptor()` has an open `// TODO: make use of: capabilities.rxOnWhenIdle...`.
  It infers sleepiness structurally instead: presence of the Poll Control cluster
  (`genPollCtrl`) on an endpoint, an explicit `checkinInterval` (read from that cluster or set by
  a device-specific quirk override for devices lacking it), or reactively — if a send fails
  outright, it assumes the device is asleep and queues.
- **Queue/replay mechanism**: each `Endpoint` owns a `RequestQueue` (`pendingRequests`). A failed
  immediate send falls into that queue (deduplicated/coalesced per request's `sendPolicy`, not a
  raw FIFO). The queue is flushed by **`implicitCheckin()`** — triggered by *any* incoming frame
  from the device (not just the formal Poll Control check-in command) — as well as explicitly when
  a `genPollCtrl.checkin` arrives (which also requests a fast-poll window from the device).
- **Expiry**: each queued request carries `expires = now + pendingRequestTimeout`, where
  `pendingRequestTimeout` defaults to one checkin interval (or 1 day if only the Poll Control
  cluster's presence is known without a read interval). Devices with neither a known interval nor
  the cluster get `pendingRequestTimeout = 0` → no queueing, send-once-and-fail. Expired entries are
  purged lazily on the next send attempt.
- **Zigbee2MQTT**: doesn't expose this as end-user config; it surfaces a related but distinct
  `availability.passive.timeout` (when to mark a battery device offline, not when to flush its
  queue) and documents the queue-and-replay behavior transparently on generated device pages
  ("requests... will be queued and sent on the next occasion").
- **Known gap acknowledged upstream**: `Koenkk/zigbee-herdsman#445` — herdsman doesn't fully
  exploit the Poll Control fast-poll window to drain the queue before the device returns to its
  slow interval; the wake-flush is opportunistic, not proactively scheduled.

**Takeaway for Haus**: both herdsman and the zigbee-java `ZigBeeTransactionManager` referenced in
#77 hold-and-replay rather than raw-retry, confirming #81's premise is worth building. Herdsman's
"wake" signal is exactly the implicit-checkin pattern independently proposed below (any incoming
frame from the device). Herdsman deliberately leaves `rxOnWhenIdle` unused — but that's a gap in
herdsman, not a reason to skip it in Haus: deCONZ's Node Descriptor response carries that bit and
Haus doesn't have herdsman's Poll Control-cluster-detection alternative built, so consuming
`rxOnWhenIdle` is Haus's most direct path to sleepy detection rather than a "nice to match" extra.

## Proposed design

### Persist the capability bit, not just parse it

Thread `DeviceAnnounce.Capabilities`'s rx-on-when-idle bit through `DeviceInterview.InterviewAsync`
into `ZigbeeDevice` and `ZigbeeDeviceJoined`, store it in `KnownDeviceTable`, and surface it on
`DeviceEntity` (a new persisted `IsSleepy`/power-source field, mirrored into
`ZigbeeKnownDeviceModel` for diagnostics parity with the existing #48 UI). This is the join-time
path. Because a device's announce isn't guaranteed to re-fire after every host restart (see the
existing NWK-address-resolution design note), also implement ZDP Node Descriptor Request/Response
(`0x0002`/`0x8002`, not currently implemented in `src/Haus.Zigbee/Zdp/`) so the bit can be
re-queried on demand rather than depending solely on a live announce.

### Define "next wake" operationally — confirmed against herdsman

Mirror herdsman's `implicitCheckin()`: treat **any inbound APS-DATA indication from that device's
IEEE address** as the wake signal, not only a formal Poll Control check-in (Haus has no Poll
Control cluster handling today and doesn't need to add one just for this). This reuses the same
event source `ZigbeeDeviceJoinedEventHandler`-style handlers already consume — just a subscriber
keyed by device address — with no new wire capability required.

### Where the hold logic lives

Branch in `CommandSender.SendCommandAsync`: if the target device's persisted capability says
sleepy, don't hand the command to `DeviceCommandQueue.EnqueueAsync` for immediate dispatch —
instead hold it in a new per-device pending slot and release it into the existing queue when the
wake signal fires (or drop/report it on TTL expiry — see below). Mains-powered devices are
unaffected and keep flowing through the existing #77 queue/retry path unchanged. This keeps the
hold/replay concern separate from `CommandRetryHandler`'s existing backoff logic rather than
conflating "retry because delivery failed" with "wait because the device isn't listening yet."

### TTL/expiry — decided: fixed configurable TTL, herdsman-style default

Herdsman computes a per-device expiry from a learned checkin interval, falling back to "no hold"
when no interval is known. Haus has no Poll Control cluster handling and no equivalent interval
source, so a direct port isn't possible. **Decision: a fixed configurable TTL** (matching #77's
existing "configurable" precedent for retry/backoff), defaulting to herdsman's fallback value
(1 day) absent a learned interval. A held command that outlives its TTL is dropped and reported
(surfaced the same way a permanently-failed command is today), not held unboundedly.

### The second retry layer — decided: unify

`ZigbeeOutboundRelay.SendWithApsAckRetryAsync` sends lighting commands through a path that doesn't
go through the sleepy-aware branch above unless it's rewired. **Decision: unify** —
`ZigbeeOutboundRelay` is rewired to route through `CommandSender`/`DeviceCommandQueue` (or the
sleepy-aware branch is lifted beneath both call sites) rather than duplicating the capability
check in a second place. Lighting commands get sleepy-device protection from the same single
code path as every other command type; no second retry/backoff implementation survives.

### Node Descriptor Request/Response — decided: in scope

Implementing ZDP Node Descriptor Request/Response (`0x0002`/`0x8002`) in `src/Haus.Zigbee/Zdp/`
is **in scope for this issue**, not split into a prerequisite issue — it's the on-demand path to
the rx-on-when-idle capability bit when a live Device_annce hasn't re-fired (e.g. after a host
restart), and #81 depends on being able to query it rather than relying solely on join-time
announce capture.

## Decisions (locked 2026-10-09)

- **TTL/expiry**: fixed configurable TTL, defaulting to herdsman's 1-day fallback; expired holds
  are dropped and reported, never held unboundedly.
- **Retry-path consolidation**: unify — `ZigbeeOutboundRelay` routes through the same
  `CommandSender`/`DeviceCommandQueue` sleepy-aware path instead of duplicating it.
- **Node Descriptor Request/Response**: in scope for this issue.

## Next steps

This note is scoping input now resolved into decisions. The next action is running this issue
through intake → planning → implementation (`craft-code:dev-workflow`) in a coding sub-agent's
own worktree, with the decisions above carried in as locked acceptance-criteria constraints.
