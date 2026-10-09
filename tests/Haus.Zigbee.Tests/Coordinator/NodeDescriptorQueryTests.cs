using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Models;
using Haus.Zigbee.Simulator;
using Haus.Zigbee.Zdp;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

public class NodeDescriptorQueryTests
{
    private const ushort ZdpProfile = 0x0000;
    private const ushort NodeDescriptorResponseCluster = 0x8002;

    private readonly FakeDeconzDongle _dongle = new();
    private readonly ApsPollLoop _pollLoop;
    private readonly ApsSender _sender;
    private readonly KnownDeviceTable _knownDeviceTable = new();
    private readonly NodeDescriptorQuery _query;

    public NodeDescriptorQueryTests()
    {
        _pollLoop = new ApsPollLoop(new DeconzChannel(_dongle.PollTransport));
        _sender = new ApsSender(_pollLoop, new DeconzChannel(_dongle.SendTransport));
        _query = new NodeDescriptorQuery(_pollLoop, _sender, _knownDeviceTable);
    }

    [Fact]
    public async Task WhenAResponseArrivesWithReceiverOnWhenIdleClearThenItReturnsSleepyAndUpdatesTheKnownDeviceTable()
    {
        var ieee = new IeeeAddress(0x00124b0001aabbcc);
        _knownDeviceTable.AddOrUpdate(new ZigbeeDevice(ieee, 0x1a2b, Array.Empty<ZigbeeEndpoint>(), IsSleepy: false));
        _dongle.ReleaseAfterSend(sendIndex: 0, NodeDescriptorResponse(0x1a2b, tsn: 0x00, macCapabilityFlags: 0x00));

        var result = await RunToCompletion(_query.QueryIsSleepyAsync(ieee, CancellationToken.None));

        Assert.True(result);
        _knownDeviceTable.TryGet(ieee, out var updated);
        Assert.True(updated!.IsSleepy);
    }

    [Fact]
    public async Task WhenAResponseArrivesWithReceiverOnWhenIdleSetThenItReturnsNotSleepy()
    {
        var ieee = new IeeeAddress(0x00124b0001aabbcc);
        _knownDeviceTable.AddOrUpdate(new ZigbeeDevice(ieee, 0x1a2b, Array.Empty<ZigbeeEndpoint>(), IsSleepy: true));
        _dongle.ReleaseAfterSend(sendIndex: 0, NodeDescriptorResponse(0x1a2b, tsn: 0x00, macCapabilityFlags: 0x08));

        var result = await RunToCompletion(_query.QueryIsSleepyAsync(ieee, CancellationToken.None));

        Assert.False(result);
        _knownDeviceTable.TryGet(ieee, out var updated);
        Assert.False(updated!.IsSleepy);
    }

    [Fact]
    public async Task WhenTheDeviceIsUnknownThenReturnsNullWithoutSendingAnything()
    {
        var result = await _query.QueryIsSleepyAsync(new IeeeAddress(0x1), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task WhenNoResponseArrivesThenReturnsNullWithoutUpdatingTheTable()
    {
        using var query = new NodeDescriptorQuery(
            _pollLoop,
            _sender,
            _knownDeviceTable,
            responseTimeout: TimeSpan.FromMilliseconds(50)
        );
        var ieee = new IeeeAddress(0x00124b0001aabbcc);
        _knownDeviceTable.AddOrUpdate(new ZigbeeDevice(ieee, 0x1a2b, Array.Empty<ZigbeeEndpoint>(), IsSleepy: false));

        var result = await RunToCompletion(query.QueryIsSleepyAsync(ieee, CancellationToken.None));

        Assert.Null(result);
        _knownDeviceTable.TryGet(ieee, out var unchanged);
        Assert.False(unchanged!.IsSleepy);
    }

    [Fact]
    public async Task WhenTheResponseStatusIsNonSuccessThenReturnsNullWithoutUpdatingTheTable()
    {
        var ieee = new IeeeAddress(0x00124b0001aabbcc);
        _knownDeviceTable.AddOrUpdate(new ZigbeeDevice(ieee, 0x1a2b, Array.Empty<ZigbeeEndpoint>(), IsSleepy: false));
        _dongle.ReleaseAfterSend(
            sendIndex: 0,
            new IndicationBody(0x1a2b, SourceEndpoint: 0x00, ZdpProfile, NodeDescriptorResponseCluster, [0x00, 0x80])
        );

        var result = await RunToCompletion(_query.QueryIsSleepyAsync(ieee, CancellationToken.None));

        Assert.Null(result);
    }

    private async Task<T> RunToCompletion<T>(Task<T> task)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        while (!task.IsCompleted && !timeout.IsCancellationRequested)
        {
            await _pollLoop.PollOnceAsync(CancellationToken.None);
            if (!task.IsCompleted)
                await Task.Delay(1);
        }

        Assert.True(task.IsCompleted, "did not complete within the timeout");
        return await task;
    }

    private static IndicationBody NodeDescriptorResponse(ushort networkAddress, byte tsn, byte macCapabilityFlags)
    {
        var asdu = new List<byte> { tsn, (byte)ZdoStatus.Success };
        AddUInt16(asdu, networkAddress);
        asdu.Add(0x00); // NodeDescriptor byte0
        asdu.Add(0x00); // NodeDescriptor byte1
        asdu.Add(macCapabilityFlags); // NodeDescriptor byte2: MAC Capability Flags
        return new IndicationBody(
            networkAddress,
            SourceEndpoint: 0x00,
            ZdpProfile,
            NodeDescriptorResponseCluster,
            asdu.ToArray()
        );
    }

    private static void AddUInt16(List<byte> bytes, ushort value)
    {
        bytes.Add((byte)(value & 0xff));
        bytes.Add((byte)(value >> 8));
    }
}
