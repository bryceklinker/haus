using Haus.Zigbee.Serial;
using Xunit;

namespace Haus.Zigbee.Tests.Serial;

public class SlipDecoderTests
{
    private const byte End = 0xC0;

    private readonly SlipDecoder _decoder = new();

    [Fact]
    public void WhenDecodingChunkContainingCompleteFrameThenYieldsDecodedFrame()
    {
        var frames = _decoder.Decode(new byte[] { End, 0x01, 0x02, 0x03, End });

        Assert.Equal(new[] { new byte[] { 0x01, 0x02, 0x03 } }, frames);
    }
}
