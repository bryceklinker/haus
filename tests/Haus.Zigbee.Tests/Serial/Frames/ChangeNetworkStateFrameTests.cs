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

    [Theory]
    [InlineData(NetworkState.Connecting, 0x01)]
    [InlineData(NetworkState.Connected, 0x02)]
    [InlineData(NetworkState.Disconnecting, 0x03)]
    public void WhenEncodingRequestForNetworkStateThenLastByteCarriesThatStateValue(
        NetworkState networkState,
        byte expectedStateByte
    )
    {
        var bytes = ChangeNetworkStateFrameCodec.Encode(networkState, sequenceNumber: 0x11);

        Assert.Equal(new byte[] { 0x08, 0x11, 0x00, 0x06, 0x00, expectedStateByte }, bytes);
    }
}
