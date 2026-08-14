using Haus.Zigbee.Serial;
using Xunit;

namespace Haus.Zigbee.Tests.Serial;

public class SlipRoundTripTests
{
    private const byte End = 0xC0;
    private const byte Esc = 0xDB;

    private readonly SlipEncoder _encoder = new();
    private readonly SlipDecoder _decoder = new();

    [Fact]
    public void WhenSimpleFrameIsEncodedThenDecodingRecoversTheOriginalFrame()
    {
        var frame = new byte[] { 0x01, 0x02, 0x03 };

        var decoded = _decoder.Decode(_encoder.Encode(frame));

        Assert.Equal(new[] { frame }, decoded);
    }

    [Fact]
    public void WhenFrameWithLiteralEndAndEscBytesIsEncodedThenDecodingRecoversTheOriginalFrame()
    {
        var frame = new byte[] { 0x01, End, 0x02, Esc, 0x03 };

        var decoded = _decoder.Decode(_encoder.Encode(frame));

        Assert.Equal(new[] { frame }, decoded);
    }
}
