using System;
using System.Threading;
using Haus.Zigbee.Connection;

namespace Haus.Zigbee.Coordinator;

// Mirrors zigbee-herdsman's implicitCheckin(): any inbound APS-DATA indication from a device's
// address is treated as that device waking up, not only a formal Poll Control check-in command
// (which Haus has no handling for and does not need to add just for this). One of several
// independent listeners on the shared IndicationReceived event -- an indication belonging to a
// device with no held command is simply a no-op release.
public class WakeSignalListener : IDisposable
{
    private readonly ApsPollLoop _pollLoop;
    private readonly SleepyCommandHold _sleepyHold;

    public WakeSignalListener(ApsPollLoop pollLoop, SleepyCommandHold sleepyHold)
    {
        _pollLoop = pollLoop;
        _sleepyHold = sleepyHold;
        _pollLoop.IndicationReceived += OnIndicationReceived;
    }

    public void Dispose()
    {
        _pollLoop.IndicationReceived -= OnIndicationReceived;
        GC.SuppressFinalize(this);
    }

    private void OnIndicationReceived(object? sender, ApsIndicationReceived received)
    {
        var key = DeviceKey.FromNwk(received.Indication.SourceNwkAddress);
        _sleepyHold.Release(key, CancellationToken.None);
    }
}
