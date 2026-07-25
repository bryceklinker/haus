using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Tests.Transport;
using Xunit;

namespace Haus.Zigbee.Tests.Connection;

public class FrameReaderTests
{
    private readonly FakeSerialTransport _transport = new();
    private readonly FrameReader _reader;

    public FrameReaderTests()
    {
        _reader = new FrameReader(_transport);
    }

    [Fact]
    public async Task WhenAValidFramedMessageArrivesThenItIsSurfacedWithoutItsChecksum()
    {
        _transport.FeedIncoming(Framed(new byte[] { 0x07, 0x03, 0xAA }));

        var frames = await _reader.ReadFramesAsync(CancellationToken.None);

        Assert.Equal(new[] { new byte[] { 0x07, 0x03, 0xAA } }, frames);
    }

    private static byte[] Framed(byte[] frame)
    {
        var checksum = DeconzCrc.Compute(frame);
        var withChecksum = new List<byte>(frame) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
        return new SlipEncoder().Encode(withChecksum.ToArray());
    }
}
