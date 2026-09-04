using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Models;

namespace Haus.Zigbee.Coordinator;

public class DeviceCommandQueue
{
    private readonly ConcurrentDictionary<IeeeAddress, SemaphoreSlim> _deviceLocks = new();

    public async Task<T> EnqueueAsync<T>(
        IeeeAddress device,
        Func<CancellationToken, Task<T>> operation,
        CancellationToken token
    )
    {
        var deviceLock = _deviceLocks.GetOrAdd(device, _ => new SemaphoreSlim(1, 1));

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
}
