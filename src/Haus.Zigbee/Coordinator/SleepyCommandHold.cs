using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Coordinator;

public class CommandHoldExpiredException : Exception
{
    public CommandHoldExpiredException()
        : base("Held command expired before the device woke") { }
}

// Per-device pending-command slot for a sleepy end device: SendCommandAsync holds a command here
// instead of enqueuing it immediately, and it is released either by a wake signal (Release) or by
// its own TTL expiry -- whichever comes first. Separate from CommandRetryHandler: this answers
// "wait because the device isn't listening yet", not "retry because delivery failed".
public class SleepyCommandHold
{
    private readonly TimeSpan _ttl;
    private readonly Func<TimeSpan, Task> _delayFunc;
    private readonly ConcurrentDictionary<DeviceKey, Entry> _holds = new();

    public SleepyCommandHold(SleepyHoldOptions? options = null)
        : this(options, delay => Task.Delay(delay)) { }

    public SleepyCommandHold(SleepyHoldOptions? options, Func<TimeSpan, Task> delayFunc)
    {
        _ttl = (options ?? new SleepyHoldOptions()).Ttl;
        _delayFunc = delayFunc;
    }

    public Task<ApsDataConfirm> HoldAsync(
        DeviceKey key,
        Func<CancellationToken, Task<ApsDataConfirm>> operation,
        CancellationToken token
    )
    {
        var entry = new Entry(operation);
        if (_holds.TryRemove(key, out var superseded))
            superseded.Tcs.TrySetCanceled();
        _holds[key] = entry;

        Forget(ExpireAfterTtlAsync(key, entry, token));
        return entry.Tcs.Task;
    }

    public void Release(DeviceKey key, CancellationToken token)
    {
        if (!_holds.TryRemove(key, out var entry))
            return;

        Forget(RunAsync(entry, token));
    }

    private async Task RunAsync(Entry entry, CancellationToken token)
    {
        try
        {
            entry.Tcs.TrySetResult(await entry.Operation(token));
        }
        catch (Exception ex)
        {
            entry.Tcs.TrySetException(ex);
        }
    }

    private async Task ExpireAfterTtlAsync(DeviceKey key, Entry entry, CancellationToken token)
    {
        await _delayFunc(_ttl);

        var removed = ((ICollection<KeyValuePair<DeviceKey, Entry>>)_holds).Remove(
            new KeyValuePair<DeviceKey, Entry>(key, entry)
        );
        if (removed)
            entry.Tcs.TrySetException(new CommandHoldExpiredException());
    }

    // Each hold's expiry timer and its eventual release both run detached off the caller's await
    // (the caller is awaiting HoldAsync's returned task, not these), so a faulted/canceled one
    // must be observed here rather than surfacing later as an unobserved-task exception.
    private static void Forget(Task task)
    {
        task.ContinueWith(
            _ => { },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default
        );
    }

    private class Entry(Func<CancellationToken, Task<ApsDataConfirm>> operation)
    {
        public Func<CancellationToken, Task<ApsDataConfirm>> Operation { get; } = operation;
        public TaskCompletionSource<ApsDataConfirm> Tcs { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
