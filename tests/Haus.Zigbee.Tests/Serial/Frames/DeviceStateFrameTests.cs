using Haus.Zigbee.Serial.Frames;
using Xunit;

namespace Haus.Zigbee.Tests.Serial.Frames;

public class DeviceStateFrameTests
{
    [Fact]
    public void GivenSequenceNumberWhenEncodingPollRequestThenProducesFixedEightByteStatusFrame()
    {
        var request = DeviceStateCodec.EncodePollRequest(0x2A);

        Assert.Equal(new byte[] { 0x07, 0x2A, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00 }, request);
    }
}
