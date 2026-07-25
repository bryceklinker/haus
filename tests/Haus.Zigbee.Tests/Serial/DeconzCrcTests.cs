using Haus.Zigbee.Serial;
using Xunit;

namespace Haus.Zigbee.Tests.Serial;

public class DeconzCrcTests
{
    [Fact]
    public void GivenKnownFrameBytesWhenComputingChecksumThenReturnsTwosComplementOfByteSum()
    {
        byte[] frame = [0x01, 0x02, 0x03, 0x04];

        var checksum = DeconzCrc.Compute(frame);

        Assert.Equal(0xFFF6, checksum);
    }

    [Fact]
    public void GivenFrameWithMatchingTrailingChecksumBytesWhenValidatingThenReportsValid()
    {
        byte[] frameWithChecksum = [0x01, 0x02, 0x03, 0x04, 0xF6, 0xFF];

        var isValid = DeconzCrc.IsValid(frameWithChecksum);

        Assert.True(isValid);
    }
}
