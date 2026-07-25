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

    [Fact]
    public void GivenStatusResponseWhenDecodingThenRecoversNetworkStateAndFlagsFromDeviceStateByte()
    {
        byte[] frame = [0x07, 0x2A, 0x00, 0x08, 0x00, 0b0000_1110, 0x00, 0x00];

        var decoded = DeviceStateCodec.Decode(frame);

        Assert.Equal(
            new DeviceStateFrame(
                CommandId: 0x07,
                NetworkState: NetworkState.Connected,
                ApsDataConfirmAvailable: true,
                ApsDataIndicationAvailable: true,
                ConfigurationChanged: false,
                ApsFreeSlotsAvailable: false
            ),
            decoded
        );
    }

    [Fact]
    public void GivenStatusResponseWithConfigAndFreeSlotBitsWhenDecodingThenRecoversConnectingStateAndThoseFlags()
    {
        byte[] frame = [0x07, 0x2A, 0x00, 0x08, 0x00, 0b0011_0001, 0x00, 0x00];

        var decoded = DeviceStateCodec.Decode(frame);

        Assert.Equal(
            new DeviceStateFrame(
                CommandId: 0x07,
                NetworkState: NetworkState.Connecting,
                ApsDataConfirmAvailable: false,
                ApsDataIndicationAvailable: false,
                ConfigurationChanged: true,
                ApsFreeSlotsAvailable: true
            ),
            decoded
        );
    }
}
