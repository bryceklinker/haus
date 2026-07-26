using Haus.Zigbee.Serial;
using Xunit;

namespace Haus.Zigbee.Tests.Serial;

public class DeconzChecksumTests
{
    [Fact]
    public void GivenKnownFrameBytesWhenComputingChecksumThenReturnsTwosComplementOfByteSum()
    {
        byte[] frame = [0x01, 0x02, 0x03, 0x04];

        var checksum = DeconzChecksum.Compute(frame);

        Assert.Equal(0xFFF6, checksum);
    }

    [Fact]
    public void GivenFrameWithMatchingTrailingChecksumBytesWhenValidatingThenReportsValid()
    {
        byte[] frameWithChecksum = [0x01, 0x02, 0x03, 0x04, 0xF6, 0xFF];

        var isValid = DeconzChecksum.IsValid(frameWithChecksum);

        Assert.True(isValid);
    }

    [Fact]
    public void GivenFrameWithCorruptedTrailingChecksumBytesWhenValidatingThenReportsInvalid()
    {
        byte[] frameWithChecksum = [0x01, 0x02, 0x03, 0x04, 0x00, 0x00];

        var isValid = DeconzChecksum.IsValid(frameWithChecksum);

        Assert.False(isValid);
    }
}
