using Haus.Zigbee;
using Haus.Zigbee.Serial.Frames;
using Xunit;

namespace Haus.Zigbee.Tests.Serial.Frames;

public class ApsDataConfirmFrameTests
{
    [Fact]
    public void WhenDecodingNwkModeFrameThenReadsShortAddressWithDestinationEndpoint()
    {
        var frame = new byte[]
        {
            0x04, // command id
            0x27, // sequence number
            0x00, // status: success
            0x0e,
            0x00, // frame length
            0x07,
            0x00, // payload length
            0x2b, // device state
            0x09, // request id
            0x02, // destination address mode: Nwk
            0x34,
            0x12, // network address 0x1234
            0x01, // destination endpoint
            0x03, // source endpoint
            0x00, // confirm status
        };

        var decoding = ApsDataConfirmCodec.Decode(frame);

        Assert.True(decoding.IsSuccessful);
        Assert.Equal(
            new ApsDataConfirm(
                SequenceNumber: 0x27,
                DeviceState: 0x2b,
                RequestId: 0x09,
                DestinationAddressMode: DeconzAddressMode.Nwk,
                DestinationShortAddress: 0x1234,
                DestinationIeeeAddress: null,
                DestinationEndpoint: 0x01,
                SourceEndpoint: 0x03,
                ConfirmStatus: 0x00
            ),
            decoding.Confirm
        );
    }
}
