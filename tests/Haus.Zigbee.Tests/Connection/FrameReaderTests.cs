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

    [Fact]
    public async Task WhenAFrameFailsChecksumValidationThenItIsDropped()
    {
        _transport.FeedIncoming(FramedWithCorruptedChecksum(new byte[] { 0x07, 0x03, 0xAA }));

        var frames = await _reader.ReadFramesAsync(CancellationToken.None);

        Assert.Empty(frames);
    }

    [Fact]
    public async Task WhenAFrameArrivesSplitAcrossTwoReadsThenItSurfacesOnceCompleted()
    {
        var framed = Framed(new byte[] { 0x07, 0x03, 0xAA });
        _transport.FeedIncoming(framed[..3]);

        var firstFrames = await _reader.ReadFramesAsync(CancellationToken.None);

        _transport.FeedIncoming(framed[3..]);
        var secondFrames = await _reader.ReadFramesAsync(CancellationToken.None);

        Assert.Empty(firstFrames);
        Assert.Equal(new[] { new byte[] { 0x07, 0x03, 0xAA } }, secondFrames);
    }

    private static byte[] Framed(byte[] frame)
    {
        var checksum = DeconzCrc.Compute(frame);
        var withChecksum = new List<byte>(frame) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
        return new SlipEncoder().Encode(withChecksum.ToArray());
    }

    private static byte[] FramedWithCorruptedChecksum(byte[] frame)
    {
        var withChecksum = new List<byte>(frame) { 0x00, 0x00 };
        return new SlipEncoder().Encode(withChecksum.ToArray());
    }
}
