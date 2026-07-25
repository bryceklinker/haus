using Haus.Zigbee.Serial;
using Xunit;

namespace Haus.Zigbee.Tests.Serial;

public class SlipDecoderTests
{
    private const byte End = 0xC0;
    private const byte Esc = 0xDB;
    private const byte EscEnd = 0xDC;
    private const byte EscEsc = 0xDD;

    private readonly SlipDecoder _decoder = new();

    [Fact]
    public void WhenDecodingChunkContainingCompleteFrameThenYieldsDecodedFrame()
    {
        var frames = _decoder.Decode(new byte[] { End, 0x01, 0x02, 0x03, End });

        Assert.Equal(new[] { new byte[] { 0x01, 0x02, 0x03 } }, frames);
    }

    [Fact]
    public void WhenDecodingChunkWithEscapedBytesThenUnescapesThemToLiteralEndAndEsc()
    {
        var frames = _decoder.Decode(
            new byte[] { End, 0x01, Esc, EscEnd, 0x02, Esc, EscEsc, 0x03, End }
        );

        Assert.Equal(new[] { new byte[] { 0x01, End, 0x02, Esc, 0x03 } }, frames);
    }

    [Fact]
    public void WhenFrameIsSplitAcrossPartialReadsThenYieldsItOnlyOnceTheTerminatingEndArrives()
    {
        var beforeEscape = _decoder.Decode(new byte[] { End, 0x01, Esc });
        var afterEscape = _decoder.Decode(new byte[] { EscEnd, 0x02 });
        var afterEnd = _decoder.Decode(new byte[] { End });

        Assert.Empty(beforeEscape);
        Assert.Empty(afterEscape);
        Assert.Equal(new[] { new byte[] { 0x01, End, 0x02 } }, afterEnd);
    }
}
