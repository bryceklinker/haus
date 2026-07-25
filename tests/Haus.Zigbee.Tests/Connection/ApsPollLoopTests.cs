using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Serial.Frames;
using Xunit;

namespace Haus.Zigbee.Tests.Connection;

public class ApsPollLoopTests
{
    private const byte NoApsDataAvailable = 0x00;
    private const byte IndicationAvailable = 0x08;

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
        _transport.QueueResponse(Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: NoApsDataAvailable)));

        await _loop.PollOnceAsync(CancellationToken.None);

        Assert.Equal(Framed(DeviceStateCodec.EncodePollRequest(0)), _transport.WrittenBytes);
    }

    [Fact]
    public async Task WhenAnIndicationIsAvailableThenTheReadIndicationRequestIsSent()
    {
        _transport.QueueResponse(Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: IndicationAvailable)));
        _transport.QueueResponse(Framed(IndicationResponse(sequenceNumber: 1)));

        await _loop.PollOnceAsync(CancellationToken.None);

        var expected = Framed(DeviceStateCodec.EncodePollRequest(0))
            .Concat(Framed(ReadIndicationRequest(sequenceNumber: 1)))
            .ToArray();
        Assert.Equal(expected, _transport.WrittenBytes);
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

    private static byte[] Concat(params byte[][] segments)
    {
        return segments.SelectMany(segment => segment).ToArray();
    }

    private static byte[] Framed(byte[] frame)
    {
        var checksum = DeconzCrc.Compute(frame);
        var withChecksum = new List<byte>(frame) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
        return new SlipEncoder().Encode(withChecksum.ToArray());
    }
}
