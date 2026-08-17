# Postmortem: `AssignDevicesToRoomFlow` acceptance-test flake

**Date range:** 2026-08-16 → 2026-08-17
**Trigger:** PR #39 (CI: install `wasm-tools` workload) sped up the Blazor WASM build enough to expose two latent client-side races and one latent third-party-library race that a slower, unoptimized build had been accidentally masking.
**Status:** Fixed and merged (PR #43), with a follow-on regression fixed and merged (PR #46). One related hard blocker (#41) remains open.

## Timeline

1. **PR #39** — added the `wasm-tools` workload to CI so Blazor WASM publishes with real IL trimming/optimization instead of the slow fallback path. Purely a CI change; merged clean.
2. **First flake report** — `AssignDevicesToRoomFlow` (drag a light, then a sensor, into the same room) started failing intermittently on `main`. Root-caused (via dynamic + static sub-agent investigation) to two independent client-side races in `DeviceDiscoveryView.razor`:
   - `OnInitializedAsync` subscribed to `DeviceCreatedEvent` before fetching devices, then unconditionally overwrote `Devices` with the fetch result — dropping any device a concurrent SignalR event had already appended.
   - `HandleDeviceDropped` re-fetched and overwrote the entire `Devices` list after every drop — an out-of-order stale-refetch race when two drops resolved out of order.
   - **Incident during investigation:** a sub-agent ran `docker compose -f docker-compose.local.yml down` without an explicit project name; since neither compose file declares one, both default to the directory basename `haus`, and the command took down the live production stack. Recovered via `systemctl restart haus-app.service`. Filed as **issue #41** (still open — needs an explicit Compose `name:` on both files as a hard guard against recurrence).
   - Both races fixed, proven with deterministic bUnit regression tests, merged as **PR #43**.
3. **Recurrence** — CI on the PR #43 merge commit itself (i.e. a commit that already had both fixes) failed with the *same symptom*. Two investigation attempts were wasted here because the local orchestration checkout was stale (12 commits behind `origin/main`, missing the just-merged fix entirely at first, then still missing later commits) — the sub-agents were reading pre-fix code and reporting a "root cause" that didn't apply to what was actually on `main`. Caught by cross-checking `git log`/`git fetch` against GitHub directly.
4. **Real root cause** — traced to actual CI failure logs and MudBlazor 9.8.0's own source: `MudDropContainer<T>` tracks one in-flight drag transaction in a single field. Its `dragstart` handler overwrites that field unconditionally, and its commit-completion handler clears it unconditionally (an incomplete upstream fix for MudBlazor#6551 — it captures the transaction locally before awaiting, but still unconditionally nulls the shared field afterward instead of checking it's still the same one). Because `HandleDeviceDropped` awaits a real network round trip before that field gets cleared, a second drag started during that window clobbers the field, and the second drop's `ItemDropped` never fires at all — the assignment is never sent, client- or server-side.
5. **Fix iteration on PR #46** — four rounds, each judged only by real CI, not local test-passing:
   1. Guard flag toggling `MudDropContainer`'s `ItemDisabled`/`draggable` attribute, set from inside the drop handler — failed in CI, identical symptom.
   2. Same toggle, moved earlier to fire on drag-start instead of drop — still failed identically. This proved the `draggable` HTML attribute has no effect on whether MudBlazor's own drag-start plumbing (or Playwright's drag simulation) can begin a new drag; the library doesn't check it there.
   3. A real pointer-capturing overlay across the *entire* `MudDropContainer` (drop zones included) while a drop was in flight — this one didn't just fail, it **broke CI**: the overlay covered the drop zones too, so the in-flight drag's own drop got swallowed by it, the guard-clearing `finally` block never ran, and the overlay stayed up forever — a self-inflicted deadlock. Two CI runs hung for 15+ minutes on the acceptance-test step and had to be manually cancelled before this was diagnosed and reverted.
   4. `pointer-events: none` applied **per device**, only to devices *other than* the one currently being dragged, and never to any drop zone — verified clean and fast (~2 minutes, normal duration) on two independent CI runs of the same commit. This is the fix that shipped.

## What actually shipped

- **PR #43** (merged): fixed the two original client-side races (fetch-vs-SignalR-event merge; drag-drop optimistic local update instead of re-fetch).
- **PR #46** (ready for merge as of this writing): fixed the MudBlazor drag-transaction race by blocking interaction with every *other* device while one drop is in flight, without ever weakening the guarantee that a room assignment is only considered done once the server confirms it (the human explicitly rejected making the network call non-blocking/fire-and-forget as a shortcut).
- **Issue #41** (open): the Compose project-name collision that caused the mid-investigation production outage. Not yet fixed.

## What made this take so many rounds

None of these are about the underlying bug being unusually obscure — they're about how the investigation and fix cycle should have been run tighter:

1. **Stale checkout wasted two investigation rounds.** The orchestrating session's local clone drifted behind `origin/main` mid-investigation, and two sub-agents spent their whole budget reading pre-fix code without either side confirming the checkout was current first.
2. **Two fix attempts were shipped on "should work" reasoning about a third-party library's public API (`ItemDisabled`), not on having read what that API actually wires up internally.** Both failed identically in CI. The library's actual source should have been read *before* the first attempt, not after two failures.
3. **One fix attempt had an unreasoned blast radius.** The pointer-events overlay was designed to block "everything," without explicitly asking "does the thing I'm blocking include the drop target that the in-flight operation itself needs?" That specific question would have caught the self-deadlock before it ever reached CI, rather than after two hung, manually-cancelled runs.
4. **A CI run silently running far outside its own historical baseline (15+ minutes vs. ~1 minute) wasn't immediately treated as a hang signal** — it took a while (and the human's prompt) to stop treating "pending" as "still fine" and start treating an abnormal duration as its own kind of failure worth investigating on its own.
5. **The coding sub-agent's CLI harness silently stalled three separate times** (a prompt visibly typed into the terminal but never submitted, leaving the process alive but doing zero CPU work indefinitely) — session-status reporting alone (`idle`/`running`) was not reliable enough to detect this; it took direct process/terminal inspection (`ps`, `tmux capture-pane`) each time to tell a genuinely stuck dispatch apart from one that was just quiet.

## Proposed process changes (for agreement, not yet adopted as policy)

- **Evidence gate for UI/race fixes:** a fix touching real browser/DOM/third-party-widget interaction is not "done" on a local test pass. It needs at least one real CI (or equivalent real-environment) run before being reported as fixed — this was mostly honored here, but only after the first two attempts had already been declared fixed prematurely based on reasoning alone.
- **Read the library before guessing at it.** Before relying on a third-party component's parameter/attribute to change its behavior, read (or have a sub-agent read) the actual source for the mechanism, not just the parameter's name/docs. Two rounds here were spent on an attribute that sounded right but was never actually wired to the behavior needed.
- **Blast-radius question before any "blocking" UI change ships:** explicitly answer "could this guard block the very operation it's guarding?" This is a cheap question that would have caught the deadlock before it hit CI at all.
- **Treat abnormal CI duration as a signal, not noise.** If a step is running meaningfully longer than its own recent baseline, investigate/cancel rather than continuing to wait on "pending."
- **Don't trust sub-agent session status alone when nothing has changed for a while.** Cross-check with a direct liveness probe (process list / terminal capture) before assuming a quiet dispatch is a working one.
- **Verify checkout freshness before delegating any investigation into an existing local clone.** A `git fetch`/`git log` sanity check costs seconds and would have prevented two wasted investigation rounds here.

## Follow-up still open

- **Issue #41** — Compose project-name collision — not yet fixed. Recommended fix (explicit `name:` on both `docker-compose.yml` and `docker-compose.local.yml`) is already scoped in the issue; just needs to be dispatched.
