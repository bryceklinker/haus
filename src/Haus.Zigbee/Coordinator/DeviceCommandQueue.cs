using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Coordinator;

public class DeviceCommandQueue
{
    private readonly ConcurrentDictionary<DeviceKey, SemaphoreSlim> _deviceLocks = new();

    public async Task<T> EnqueueAsync<T>(
        IeeeAddress device,
        Func<CancellationToken, Task<T>> operation,
        CancellationToken token
    )
    {
        return await EnqueueAsync(DeviceKey.FromIeee(device), operation, token);
    }

    public async Task<T> EnqueueAsync<T>(
        ApsDestination destination,
        Func<CancellationToken, Task<T>> operation,
        CancellationToken token
    )
    {
        var key = DeviceKey.FromDestination(destination);
        if (key is null)
            return await operation(token);

        return await EnqueueAsync(key.Value, operation, token);
    }

    private async Task<T> EnqueueAsync<T>(
        DeviceKey key,
        Func<CancellationToken, Task<T>> operation,
        CancellationToken token
    )
    {
        var deviceLock = _deviceLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

        await deviceLock.WaitAsync(token);
        try
        {
            return await operation(token);
        }
        finally
        {
            deviceLock.Release();
        }
    }

    private readonly record struct DeviceKey(bool IsIeee, ulong Address)
    {
        public static DeviceKey FromIeee(IeeeAddress ieee) => new(true, ieee.Value);

        public static DeviceKey? FromDestination(ApsDestination destination)
        {
            return destination.Mode switch
            {
                DeconzAddressMode.Ieee => new DeviceKey(true, destination.IeeeAddress.Value),
                DeconzAddressMode.Nwk => new DeviceKey(false, destination.ShortAddress),
                _ => null,
            };
        }
    }
}
