using Haus.Zigbee.Serial;
using Xunit;

namespace Haus.Zigbee.Tests.Serial;

public class SlipEncoderTests
{
    private const byte End = 0xC0;

    private readonly SlipEncoder _encoder = new();

    [Fact]
    public void WhenEncodingSimpleFrameThenWrapsPayloadWithEndDelimiters()
    {
        var encoded = _encoder.Encode(new byte[] { 0x01, 0x02, 0x03 });

        Assert.Equal(new byte[] { End, 0x01, 0x02, 0x03, End }, encoded);
    }
}
