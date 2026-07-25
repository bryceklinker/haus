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

    [Fact]
    public async Task WhenPollingThenTheDeviceStatePollRequestIsSent()
    {
        _transport.QueueResponse(Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: 0x00)));

        await _loop.PollOnceAsync(CancellationToken.None);

        Assert.Equal(Framed(DeviceStateCodec.EncodePollRequest(0)), _transport.WrittenBytes);
    }

    private static byte[] DeviceStateResponse(byte sequenceNumber, byte deviceState)
    {
        return new byte[] { 0x07, sequenceNumber, 0x00, 0x00, 0x00, deviceState };
    }

    private static byte[] Framed(byte[] frame)
    {
        var checksum = DeconzCrc.Compute(frame);
        var withChecksum = new List<byte>(frame) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
        return new SlipEncoder().Encode(withChecksum.ToArray());
    }
}
