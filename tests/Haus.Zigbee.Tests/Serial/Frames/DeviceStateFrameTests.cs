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
        var decoded = Decode(commandId: 0x07, deviceState: 0b0000_1110);

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
        var decoded = Decode(commandId: 0x07, deviceState: 0b0011_0001);

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

    [Fact]
    public void GivenUnsolicitedStatusChangeIndicationWhenDecodingThenDecodesIdenticallyAndExposesIndicationCommandId()
    {
        var decoded = Decode(commandId: 0x0E, deviceState: 0b0000_1110);

        Assert.Equal(
            new DeviceStateFrame(
                CommandId: 0x0E,
                NetworkState: NetworkState.Connected,
                ApsDataConfirmAvailable: true,
                ApsDataIndicationAvailable: true,
                ConfigurationChanged: false,
                ApsFreeSlotsAvailable: false
            ),
            decoded
        );
    }

    [Theory]
    [InlineData(new byte[] { })]
    [InlineData(new byte[] { 0x07 })]
    [InlineData(new byte[] { 0x07, 0x2A, 0x00, 0x08, 0x00 })]
    public void WhenDecodingAFrameTooShortForItsDeviceStateByteThenReturnsNullInsteadOfThrowing(byte[] frame)
    {
        Assert.Null(DeviceStateCodec.Decode(frame));
    }

    private static DeviceStateFrame? Decode(byte commandId, byte deviceState)
    {
        byte[] frame = [commandId, 0x2A, 0x00, 0x08, 0x00, deviceState, 0x00, 0x00];
        return DeviceStateCodec.Decode(frame);
    }
}
