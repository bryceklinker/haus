using Haus.Zigbee.Serial.Frames;
using Xunit;

namespace Haus.Zigbee.Tests.Serial.Frames;

public class ChangeNetworkStateFrameTests
{
    [Fact]
    public void WhenEncodingRequestForDisconnectedThenProducesSixByteFrameWithStateZero()
    {
        var bytes = ChangeNetworkStateFrameCodec.Encode(NetworkState.Disconnected, sequenceNumber: 0x42);

        Assert.Equal(new byte[] { 0x08, 0x42, 0x00, 0x06, 0x00, 0x00 }, bytes);
    }
}
