using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Haus.Zigbee.Simulator;

// Lets a test script a device-interview response sequence: the Nth APS data-request this
// responder sees FOR THAT SPECIFIC DEVICE (0-based) is exactly when a real device would have
// replied, so queuing the response there keeps request/response order faithful to a real
// interview. Keyed by (destination device, that device's own request step) rather than a single
// shared request counter: two devices' interviews can have their individual ZDP/ZCL steps
// interleave on the wire (each is its own detached task on the coordinator side), so a flat
// global index cannot tell two devices' 1st/2nd/3rd requests apart. Per-device counting
// sidesteps that entirely -- it needs no reservation or locking beyond what ConcurrentDictionary
// already gives.
public class DeconzInterviewScript
{
    private readonly ConcurrentDictionary<
        (ushort NetworkAddress, int Step),
        Func<byte[], IndicationBody>
    > _releaseOnApsRequest = new();
    private readonly ConcurrentDictionary<ushort, int> _apsRequestCountByDevice = new();

    // The factory receives that request's raw bytes so it can echo back the request's own
    // transaction sequence number -- DeviceInterview correlates responses on that number, and a
    // real device discovers it from the request rather than knowing it in advance.
    public void ReleaseAfterApsRequest(ushort networkAddress, int step, Func<byte[], IndicationBody> bodyFactory)
    {
        _releaseOnApsRequest[(networkAddress, step)] = bodyFactory;
    }

    public int GetApsRequestCountForDevice(ushort networkAddress)
    {
        return _apsRequestCountByDevice.GetValueOrDefault(networkAddress);
    }

    public IndicationBody? RecordApsRequestAndTryRelease(ushort networkAddress, byte[] request)
    {
        var countForDevice = _apsRequestCountByDevice.AddOrUpdate(networkAddress, 1, (_, count) => count + 1);
        var step = countForDevice - 1;
        return _releaseOnApsRequest.TryRemove((networkAddress, step), out var bodyFactory)
            ? bodyFactory(request)
            : null;
    }
}
