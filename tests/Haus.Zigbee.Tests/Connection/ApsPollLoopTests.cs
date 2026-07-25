using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Serial.Frames;
using Xunit;

namespace Haus.Zigbee.Tests.Connection;

public class ApsPollLoopTests
{
    private readonly ScriptedSerialTransport _transport = new();
    private readonly ApsPollLoop _loop;

    public ApsPollLoopTests()
    {
        _loop = new ApsPollLoop(new DeconzChannel(_transport));
    }

    private const byte NoApsDataAvailable = 0x00;
    private const byte IndicationAvailable = 0x08;

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

        var expected = new List<byte>();
        expected.AddRange(Framed(DeviceStateCodec.EncodePollRequest(0)));
        expected.AddRange(Framed(ReadIndicationRequest(sequenceNumber: 1)));
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
        return new byte[]
        {
            0x17,
            sequenceNumber,
            0x00, // command id, sequence number, success status
            0x00,
            0x00, // frame length (ignored by decoder)
            0x00,
            0x00, // payload length (ignored by decoder)
            0x00, // device state
            0x02,
            0x00,
            0x00,
            0x01, // dest: Nwk mode, addr 0x0000, endpoint 0x01
            0x02,
            0x34,
            0x12,
            0x01, // source: Nwk mode, addr 0x1234, endpoint 0x01
            0x04,
            0x01, // profile id 0x0104
            0x00,
            0x00, // cluster id 0x0000
            0x02,
            0x00, // asdu length 2
            0xAA,
            0xBB, // asdu payload
            0x00,
            0x00, // reserved
            0xFF, // link quality indicator
        };
    }

    private static byte[] Framed(byte[] frame)
    {
        var checksum = DeconzCrc.Compute(frame);
        var withChecksum = new List<byte>(frame) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
        return new SlipEncoder().Encode(withChecksum.ToArray());
    }
}
