using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Host.Storage;
using Haus.Zigbee.Models;
using Microsoft.Extensions.Logging;

namespace Haus.Zigbee.Host.Zigbee;

// The in-memory cache (networkAddress -> externalId) stays the only thing the attribute-report hot
// path (DeviceEventMapper.Map, ZigbeeDiagnosticsPublisher) ever reads, synchronously, with no DB
// round trip. Every write-through to the store runs detached (Forget), so a slow or failing
// persist never blocks or breaks the join/discovery flow that triggered it -- durability is a
// best-effort side effect of registration here, not a precondition for it.
public class DeviceAddressRegistry(IKnownZigbeeDeviceStore store, ILogger<DeviceAddressRegistry> logger)
{
    private readonly ConcurrentDictionary<ushort, string> _externalIdsByNetworkAddress = new();

    // Primes the cache from durable storage so devices that haven't re-announced since a restart
    // still resolve immediately on their next attribute report, instead of logging "no known
    // device" until they rejoin. Called once at host startup, after migrate-on-boot.
    public async Task LoadAsync(CancellationToken token = default)
    {
        var devices = await store.GetAllAsync(token);
        foreach (var device in devices)
            _externalIdsByNetworkAddress[device.NetworkAddress] = device.ExternalId;
    }

    public void Register(ushort networkAddress, string externalId)
    {
        _externalIdsByNetworkAddress[networkAddress] = externalId;
        Forget(PersistNetworkAddressAsync(networkAddress, externalId));
    }

    // Used by callers that already have the full joined/known-device info (a fresh join, or a
    // backfilled/synced already-known device), so endpoints and sleepy classification are
    // persisted alongside the address rather than only the address itself.
    public void RegisterKnownDevice(
        IeeeAddress ieeeAddress,
        ushort networkAddress,
        IReadOnlyList<ZigbeeEndpoint> endpoints,
        bool isSleepy
    )
    {
        var externalId = ExternalIdConverter.ToExternalId(ieeeAddress);
        _externalIdsByNetworkAddress[networkAddress] = externalId;
        Forget(PersistKnownDeviceAsync(externalId, networkAddress, endpoints, isSleepy));
    }

    public bool TryGetExternalId(ushort networkAddress, [MaybeNullWhen(false)] out string externalId)
    {
        return _externalIdsByNetworkAddress.TryGetValue(networkAddress, out externalId);
    }

    private async Task PersistNetworkAddressAsync(ushort networkAddress, string externalId)
    {
        try
        {
            await store.UpsertNetworkAddressAsync(new UpsertNetworkAddressArgs(externalId, networkAddress));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to persist network address for {@ExternalId}", externalId);
        }
    }

    private async Task PersistKnownDeviceAsync(
        string externalId,
        ushort networkAddress,
        IReadOnlyList<ZigbeeEndpoint> endpoints,
        bool isSleepy
    )
    {
        try
        {
            await store.UpsertAsync(
                new UpsertKnownZigbeeDeviceArgs(
                    externalId,
                    networkAddress,
                    JsonSerializer.Serialize(endpoints),
                    isSleepy
                )
            );
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to persist known device state for {@ExternalId}", externalId);
        }
    }

    // Each registration's persistence write runs detached off the caller (Register/RegisterKnownDevice
    // stay synchronous so every existing join/attribute-report call site is unaffected), so a faulted
    // task must be observed here rather than surfacing later as an unobserved-task exception.
    private static void Forget(Task task)
    {
        task.ContinueWith(
            _ => { },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default
        );
    }
}
