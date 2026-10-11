# Intake: Sleepy/battery end-device-aware outbound delivery (issue #81)

Design input: `docs/craft/design/2026-10-09-sleepy-device-aware-outbound-delivery.md`.
Split from #77 (per-device queue/retry, delivered in PR #85).

## Locked decisions (2026-10-09) — carried verbatim as constraints, not reopened

1. **TTL/expiry**: a held command's hold has a fixed configurable TTL, defaulting to
   herdsman's 1-day fallback. An expired hold is dropped and reported (same surfacing as a
   permanently-failed command today), never held unboundedly.
2. **Retry-path consolidation**: `ZigbeeOutboundRelay.SendWithApsAckRetryAsync` (the lighting
   APS-ACK retry path) is unified into the same `CommandSender`/`DeviceCommandQueue`
   sleepy-aware path rather than duplicating the capability check. No second retry/backoff
   implementation survives after this work.
3. **Node Descriptor scope**: ZDP Node Descriptor Request/Response (`0x0002`/`0x8002`) in
   `src/Haus.Zigbee/Zdp/` is in scope for this issue (not deferred to a prerequisite issue).

## Acceptance criteria

### AC1 — Node Descriptor Request/Response (locked decision 3)

- **Given** a network address of a device whose rx-on-when-idle capability is unknown,
  **when** a Node Descriptor Request (`0x0002`) is sent and a Node Descriptor Response
  (`0x8002`) with status Success is decoded,
  **then** the rx-on-when-idle bit (MAC Capability Flags byte, bit 3 — same bit position as
  `DeviceAnnounceParser`'s Device_annce capability byte) is extracted and exposed.
- **Given** a Node Descriptor Response with a non-Success status, or a payload too short to
  contain the MAC Capability Flags byte,
  **then** decoding returns a result the caller can treat as "unknown" without throwing
  (mirrors `NwkAddrResponseCodec`/`ActiveEndpointsResponseCodec`'s truncated-payload handling).

### AC2 — Thread the capability bit from announce through join

- **Given** a Device_annce whose MAC Capability byte has the rx-on-when-idle bit clear,
  **when** `DeviceInterview.InterviewAsync` completes the join,
  **then** the resulting `ZigbeeDevice` and `ZigbeeDeviceJoined` both carry `IsSleepy = true`,
  and `KnownDeviceTable` persists that flag against the device's IEEE address.
- **Given** the rx-on-when-idle bit is set (device is not sleepy),
  **then** `IsSleepy = false` on both records and in `KnownDeviceTable`.
- **Given** a device already in `KnownDeviceTable` whose network address is updated (existing
  `UpdateNetworkAddress` path),
  **then** its persisted `IsSleepy` value is preserved, not reset to a default.

### AC3 — On-demand re-query when a live announce hasn't re-fired (locked decision 3's purpose)

- **Given** a device known by IEEE address but whose sleepy classification needs refreshing
  (e.g. after a host restart, when `DevicesMapper`'s full sync or `DeviceBackfillService` runs
  without a fresh Device_annce),
  **when** the coordinator issues a Node Descriptor Request for that device's current network
  address and gets a Success response,
  **then** `KnownDeviceTable`'s persisted `IsSleepy` is updated from the response and the
  refreshed value flows into the `ZigbeeDeviceJoined` that backfill/full-sync publishes.
- **Given** no response arrives (timeout) or the device is unknown,
  **then** the previously-known `IsSleepy` value is kept as-is (no regression to a default).

### AC4 — Persist `IsSleepy` end-to-end into the domain model

- **Given** a `ZigbeeDeviceJoined` with `IsSleepy = true`,
  **when** `DeviceJoinedMapper.Map` builds the `DeviceDiscoveredEvent`,
  **then** the event carries `IsSleepy = true`, and `DeviceEntity.UpdateFromDiscoveredDevice`
  persists it onto a new `DeviceEntity.IsSleepy` column (EF migration included).
- **Given** the issue #48 diagnostics UI's `ZigbeeKnownDeviceModel`,
  **then** it also carries `IsSleepy`, sourced the same way `ManufacturerName`/`ModelIdentifier`
  already are (via `ZigbeeState.RecordDeviceJoined`/`ZigbeeDeviceJoinedEvent`).

### AC5 — Hold instead of blind-retry for a sleepy device (the core of #81)

- **Given** a command destined for a device `KnownDeviceTable` marks `IsSleepy = true`,
  **when** `CommandSender.SendCommandAsync` is called,
  **then** the command is held in a per-device pending slot rather than being handed to
  `DeviceCommandQueue.EnqueueAsync` for immediate dispatch.
- **Given** a mains-powered device (`IsSleepy = false`, or unknown to `KnownDeviceTable`),
  **then** behavior is unchanged — it flows through the existing #77
  `DeviceCommandQueue`/`CommandRetryHandler` path exactly as before.
- **Given** a command is held for a sleepy device,
  **when** any inbound APS-DATA indication arrives from that device's address (herdsman's
  `implicitCheckin` pattern — no formal Poll Control check-in required),
  **then** the held command is released into the existing `DeviceCommandQueue` path and sent.
- This hold/replay branch is implemented as a distinct concern from
  `CommandRetryHandler`'s existing backoff logic ("retry because delivery failed" stays
  separate from "wait because the device isn't listening yet"), per the design note.

### AC6 — TTL/expiry (locked decision 1, verbatim)

- A held command's hold has a **fixed configurable TTL**, defaulting to **herdsman's 1-day
  fallback**.
- **Given** a held command whose TTL elapses with no wake signal,
  **then** the hold is **dropped and reported** (surfaced the same way a permanently-failed
  command is today — i.e. via the existing transport-error/failure reporting path), and the
  command is **never held unboundedly**.

### AC7 — Unify the second retry path (locked decision 2, verbatim)

- `ZigbeeOutboundRelay`'s lighting APS-ACK retry path is **unified into the same
  `CommandSender`/`DeviceCommandQueue` sleepy-aware path** rather than duplicating the
  capability check.
- **Given** a lighting command whose first delivery attempt fails,
  **when** it is retried,
  **then** the retry escalates to request an APS-ACK, using the *same* retry mechanism
  (`CommandRetryHandler`, inside `CommandSender`) that every other command type already uses —
  not a second, independent resilience pipeline in `ZigbeeOutboundRelay`.
- **No second retry/backoff implementation survives**: `ZigbeeOutboundRelay`'s own
  `ResiliencePipelineBuilder`-based retry wrapper is removed; lighting commands get
  sleepy-device protection "for free" by going through the same `CommandSender.SendCommandAsync`
  entry point as every other command.

## Scope notes / judgment calls (recommended, not reopening the locked decisions)

- The APS-ACK-escalation-on-retry behavior, once unified into `CommandRetryHandler`, becomes a
  property of *every* command sent through `CommandSender` (not lighting-specific as it was
  before). This is the natural consequence of "no second retry implementation survives" and is
  recommended over trying to preserve a lighting-only special case inside the shared path.
- A sleepy device gets exactly one pending hold slot (not a bounded queue): a second command
  arriving for an already-held device supersedes (cancels) the first. This matches the design
  note's "a new per-device pending slot" (singular) and keeps scope bounded; a multi-slot queue
  per sleepy device is not required by #81 and is not built here.
- Re-querying the Node Descriptor on every full-sync/backfill call (AC3) is the recommended
  default; it is cheap (one ZDP round trip) and keeps classification fresh without relying on
  live announces.
