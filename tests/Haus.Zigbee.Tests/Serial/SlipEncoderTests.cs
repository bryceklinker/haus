using Haus.Zigbee.Serial;
using Xunit;

namespace Haus.Zigbee.Tests.Serial;

public class SlipEncoderTests
{
    private const byte End = 0xC0;
    private const byte Esc = 0xDB;
    private const byte EscEnd = 0xDC;
    private const byte EscEsc = 0xDD;

    private readonly SlipEncoder _encoder = new();

    [Fact]
    public void WhenEncodingSimpleFrameThenWrapsPayloadWithEndDelimiters()
    {
        var encoded = _encoder.Encode(new byte[] { 0x01, 0x02, 0x03 });

        Assert.Equal(new byte[] { End, 0x01, 0x02, 0x03, End }, encoded);
    }

    [Fact]
    public void WhenEncodingFrameContainingEndAndEscBytesThenByteStuffsThem()
    {
        var encoded = _encoder.Encode(new byte[] { 0x01, End, 0x02, Esc, 0x03 });

        Assert.Equal(
            new byte[] { End, 0x01, Esc, EscEnd, 0x02, Esc, EscEsc, 0x03, End },
            encoded
        );
    }
}
