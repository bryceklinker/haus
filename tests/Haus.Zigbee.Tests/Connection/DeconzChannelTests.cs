using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Tests.Transport;
using Xunit;

namespace Haus.Zigbee.Tests.Connection;

public class DeconzChannelTests
{
    private readonly FakeSerialTransport _transport = new();
    private readonly DeconzChannel _channel;

    public DeconzChannelTests()
    {
        _channel = new DeconzChannel(_transport);
    }

    [Fact]
    public async Task WhenSendingAFrameThenItIsChecksumAppendedAndSlipEncodedOntoTheTransport()
    {
        var command = new byte[] { 0x0A, 0x05, 0x01 };
        _transport.FeedIncoming(Framed(new byte[] { 0x0A, 0x05, 0x00 }));

        await _channel.SendAndReceiveAsync(command, CancellationToken.None);

        Assert.Equal(Framed(command), _transport.WrittenBytes);
    }

    [Fact]
    public async Task WhenResponsesArriveThenTheOneMatchingTheSentSequenceNumberIsReturned()
    {
        var command = new byte[] { 0x0A, 0x05, 0x01 };
        _transport.FeedIncoming(Framed(new byte[] { 0x0A, 0x09, 0xFF }));
        _transport.FeedIncoming(Framed(new byte[] { 0x0A, 0x05, 0x42 }));

        var response = await _channel.SendAndReceiveAsync(command, CancellationToken.None);

        Assert.Equal(new byte[] { 0x0A, 0x05, 0x42 }, response);
    }

    private static byte[] Framed(byte[] frame)
    {
        var checksum = DeconzCrc.Compute(frame);
        var withChecksum = new List<byte>(frame) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
        return new SlipEncoder().Encode(withChecksum.ToArray());
    }
}
