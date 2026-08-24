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

public class NetworkAddressResolverTests
{
    private const ushort ZdpProfile = 0x0000;
    private const ushort NwkAddrResponseCluster = 0x8000;
    private const ushort ActiveEndpointsResponseCluster = 0x8005;

    private readonly FakeDeconzDongle _dongle = new();
    private readonly ApsPollLoop _pollLoop;
    private readonly ApsSender _sender;
    private readonly KnownDeviceTable _knownDeviceTable = new();
    private readonly NetworkAddressResolver _resolver;

    public NetworkAddressResolverTests()
    {
        _pollLoop = new ApsPollLoop(new DeconzChannel(_dongle.PollTransport));
        _sender = new ApsSender(_pollLoop, new DeconzChannel(_dongle.SendTransport));
        _resolver = new NetworkAddressResolver(_pollLoop, _sender, _knownDeviceTable);
    }

    [Fact]
    public async Task WhenAResponseArrivesForTheBroadcastRequestThenItResolvesTheNetworkAddressAndRecordsTheDeviceInTheKnownDeviceTable()
    {
        var ieee = new IeeeAddress(0x00124b0001aabbcc);
        _dongle.ReleaseAfterSend(sendIndex: 0, NwkAddrResponse(ieee, networkAddress: 0x1a2b, tsn: 0x00));

        var resolved = await RunToCompletion(_resolver.ResolveAsync(ieee, CancellationToken.None));

        Assert.Equal((ushort?)0x1a2b, resolved);
        var known = Assert.Single(_knownDeviceTable.GetDevices());
        Assert.Equal(ieee, known.IeeeAddress);
        Assert.Equal(0x1a2b, known.NetworkAddress);
    }

    [Fact]
    public async Task WhenTheIeeeAddressIsAlreadyKnownWithEndpointsThenResolvingUpdatesItsNetworkAddressWhilePreservingThoseEndpoints()
    {
        var ieee = new IeeeAddress(0x00124b0001aabbcc);
        var endpoint = new ZigbeeEndpoint(0x01, 0x0104, 0x0100, new ushort[] { 0x0000 }, Array.Empty<ushort>());
        _knownDeviceTable.AddOrUpdate(new ZigbeeDevice(ieee, NetworkAddress: 0x0001, new[] { endpoint }));
        _dongle.ReleaseAfterSend(sendIndex: 0, NwkAddrResponse(ieee, networkAddress: 0x1a2b, tsn: 0x00));

        var resolved = await RunToCompletion(_resolver.ResolveAsync(ieee, CancellationToken.None));

        Assert.Equal((ushort?)0x1a2b, resolved);
        var known = Assert.Single(_knownDeviceTable.GetDevices());
        Assert.Equal(0x1a2b, known.NetworkAddress);
        var preserved = Assert.Single(known.Endpoints);
        Assert.Equal(0x01, preserved.EndpointId);
    }

    [Fact]
    public async Task WhenNoDeviceAnswersTheBroadcastRequestThenResolvingReturnsNullWithoutRecordingAnything()
    {
        using var resolver = new NetworkAddressResolver(
            _pollLoop,
            _sender,
            _knownDeviceTable,
            responseTimeout: TimeSpan.FromMilliseconds(50)
        );
        var ieee = new IeeeAddress(0x00124b0001aabbcc);

        var resolved = await RunToCompletion(resolver.ResolveAsync(ieee, CancellationToken.None));

        Assert.Null(resolved);
        Assert.Empty(_knownDeviceTable.GetDevices());
    }

    [Fact]
    public async Task WhenAnIndicationOnAnUnrelatedClusterArrivesFirstThenItIsIgnoredAndTheResolverKeepsWaitingForItsResponse()
    {
        var ieee = new IeeeAddress(0x00124b0001aabbcc);
        // Same transaction sequence number as the request, but a different cluster -- proves the
        // resolver filters on cluster and does not complete on this indication.
        _dongle.InjectIndication(
            new IndicationBody(
                SourceNwk: 0x1a2b,
                SourceEndpoint: 0x00,
                ZdpProfile,
                ActiveEndpointsResponseCluster,
                Asdu: new byte[] { 0x00, 0x00, 0x00 }
            )
        );
        _dongle.ReleaseAfterSend(sendIndex: 0, NwkAddrResponse(ieee, networkAddress: 0x1a2b, tsn: 0x00));

        var resolved = await RunToCompletion(_resolver.ResolveAsync(ieee, CancellationToken.None));

        Assert.Equal((ushort?)0x1a2b, resolved);
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

    private static IndicationBody NwkAddrResponse(
        IeeeAddress ieee,
        ushort networkAddress,
        byte tsn,
        ZdoStatus status = ZdoStatus.Success
    )
    {
        var asdu = new List<byte> { tsn, (byte)status };
        AddUInt64(asdu, ieee.Value);
        AddUInt16(asdu, networkAddress);
        return new IndicationBody(
            networkAddress,
            SourceEndpoint: 0x00,
            ZdpProfile,
            NwkAddrResponseCluster,
            asdu.ToArray()
        );
    }

    private static void AddUInt16(List<byte> bytes, ushort value)
    {
        bytes.Add((byte)(value & 0xff));
        bytes.Add((byte)(value >> 8));
    }

    private static void AddUInt64(List<byte> bytes, ulong value)
    {
        for (var shift = 0; shift < 64; shift += 8)
            bytes.Add((byte)((value >> shift) & 0xff));
    }
}
