using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Tests.Coordinator;
using Xunit;

namespace Haus.Zigbee.Tests.Connection;

public class ApsPollLoopTests
{
    private const byte NoApsDataAvailable = 0x00;
    private const byte IndicationAvailable = 0x08;
    private const byte ConfirmAvailable = 0x04;

    private const byte SuccessStatus = 0x00;
    private const byte NwkAddressMode = 0x02;

    private readonly ScriptedSerialTransport _transport = new();
    private readonly ApsPollLoop _loop;

    public ApsPollLoopTests()
    {
        _loop = new ApsPollLoop(new DeconzChannel(_transport));
    }

    [Fact]
    public async Task WhenPollingThenTheDeviceStatePollRequestIsSent()
    {
        _transport.QueueResponse(
            DeconzFrames.Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: NoApsDataAvailable))
        );

        await _loop.PollOnceAsync(CancellationToken.None);

        Assert.Equal(DeconzFrames.Framed(DeviceStateCodec.EncodePollRequest(0)), _transport.WrittenBytes);
    }

    [Fact]
    public async Task WhenAnIndicationIsAvailableThenTheReadIndicationRequestIsSent()
    {
        _transport.QueueResponse(
            DeconzFrames.Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: IndicationAvailable))
        );
        _transport.QueueResponse(DeconzFrames.Framed(IndicationResponse(sequenceNumber: 1)));

        await _loop.PollOnceAsync(CancellationToken.None);

        var expected = DeconzFrames
            .Framed(DeviceStateCodec.EncodePollRequest(0))
            .Concat(DeconzFrames.Framed(ReadIndicationRequest(sequenceNumber: 1)))
            .ToArray();
        Assert.Equal(expected, _transport.WrittenBytes);
    }

    [Fact]
    public async Task WhenAnIndicationIsReadThenItIsRaisedAsAnIndicationReceivedEvent()
    {
        _transport.QueueResponse(
            DeconzFrames.Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: IndicationAvailable))
        );
        _transport.QueueResponse(DeconzFrames.Framed(IndicationResponse(sequenceNumber: 1)));
        var received = new List<ApsIndicationReceived>();
        _loop.IndicationReceived += (_, indication) => received.Add(indication);

        await _loop.PollOnceAsync(CancellationToken.None);

        var indication = Assert.Single(received).Indication;
        Assert.Equal(0x1234, indication.SourceNwkAddress);
        Assert.Equal(0x0000, indication.ClusterId);
        Assert.Equal(new byte[] { 0xAA, 0xBB }, indication.AsduPayload);
    }

    [Fact]
    public async Task WhenAConfirmIsAvailableThenTheReadConfirmRequestIsSent()
    {
        _transport.QueueResponse(
            DeconzFrames.Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: ConfirmAvailable))
        );
        _transport.QueueResponse(DeconzFrames.Framed(ConfirmResponse(sequenceNumber: 1, requestId: 0x42)));

        await _loop.PollOnceAsync(CancellationToken.None);

        var expected = DeconzFrames
            .Framed(DeviceStateCodec.EncodePollRequest(0))
            .Concat(DeconzFrames.Framed(ReadConfirmRequest(sequenceNumber: 1)))
            .ToArray();
        Assert.Equal(expected, _transport.WrittenBytes);
    }

    [Fact]
    public async Task WhenAConfirmIsReadThenItIsRaisedAsAConfirmReceivedEvent()
    {
        _transport.QueueResponse(
            DeconzFrames.Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: ConfirmAvailable))
        );
        _transport.QueueResponse(DeconzFrames.Framed(ConfirmResponse(sequenceNumber: 1, requestId: 0x42)));
        var received = new List<ApsDataConfirm>();
        _loop.ConfirmReceived += (_, confirm) => received.Add(confirm);

        await _loop.PollOnceAsync(CancellationToken.None);

        Assert.Equal(0x42, Assert.Single(received).RequestId);
    }

    [Fact]
    public async Task WhenTheDeviceStateResponseIsTruncatedThenPollingCompletesWithoutThrowingOrDrainingAnything()
    {
        _transport.QueueResponse(DeconzFrames.Framed(new byte[] { 0x07, 0x00, 0x00 }));

        await _loop.PollOnceAsync(CancellationToken.None);

        Assert.Equal(DeconzFrames.Framed(DeviceStateCodec.EncodePollRequest(0)), _transport.WrittenBytes);
    }

    private static byte[] DeviceStateResponse(byte sequenceNumber, byte deviceState)
    {
        return new byte[] { 0x07, sequenceNumber, 0x00, 0x00, 0x00, deviceState };
    }

    private static byte[] ReadIndicationRequest(byte sequenceNumber)
    {
        return new byte[] { 0x17, sequenceNumber, 0x00, 0x08, 0x00, 0x01, 0x00, 0x01 };
    }

    private static byte[] IndicationResponse(byte sequenceNumber)
    {
        var header = new byte[] { 0x17, sequenceNumber, SuccessStatus, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var destination = new byte[] { NwkAddressMode, 0x00, 0x00, 0x01 };
        var source = new byte[] { NwkAddressMode, 0x34, 0x12, 0x01 };
        var profileAndCluster = new byte[] { 0x04, 0x01, 0x00, 0x00 };
        var asdu = new byte[] { 0x02, 0x00, 0xAA, 0xBB };
        var reservedAndLinkQuality = new byte[] { 0x00, 0x00, 0xFF };
        return Concat(header, destination, source, profileAndCluster, asdu, reservedAndLinkQuality);
    }

    private static byte[] ReadConfirmRequest(byte sequenceNumber)
    {
        return new byte[] { 0x04, sequenceNumber, 0x00, 0x07, 0x00, 0x00, 0x00 };
    }

    private static byte[] ConfirmResponse(byte sequenceNumber, byte requestId)
    {
        var header = new byte[] { 0x04, sequenceNumber, SuccessStatus, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var requestAndAddress = new byte[] { requestId, NwkAddressMode, 0x34, 0x12 };
        var endpointsAndStatus = new byte[] { 0x01, 0x01, 0x00 };
        return Concat(header, requestAndAddress, endpointsAndStatus);
    }

    private static byte[] Concat(params byte[][] segments)
    {
        return segments.SelectMany(segment => segment).ToArray();
    }
}
