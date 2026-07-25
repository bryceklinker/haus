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

    [Fact]
    public void WhenDecodingGroupModeFrameThenReadsShortAddressWithoutDestinationEndpoint()
    {
        var frame = new byte[]
        {
            0x04, // command id
            0x11, // sequence number
            0x00, // status: success
            0x0d,
            0x00, // frame length
            0x06,
            0x00, // payload length
            0x08, // device state
            0x0a, // request id
            0x01, // destination address mode: Group
            0xcd,
            0xab, // group address 0xabcd
            0x05, // source endpoint (no destination endpoint present)
            0xa7, // confirm status: no-ack
        };

        var decoding = ApsDataConfirmCodec.Decode(frame);

        Assert.True(decoding.IsSuccessful);
        Assert.Equal(
            new ApsDataConfirm(
                SequenceNumber: 0x11,
                DeviceState: 0x08,
                RequestId: 0x0a,
                DestinationAddressMode: DeconzAddressMode.Group,
                DestinationShortAddress: 0xabcd,
                DestinationIeeeAddress: null,
                DestinationEndpoint: null,
                SourceEndpoint: 0x05,
                ConfirmStatus: 0xa7
            ),
            decoding.Confirm
        );
    }

    [Fact]
    public void WhenDecodingIeeeModeFrameThenReadsIeeeAddressWithDestinationEndpoint()
    {
        var frame = new byte[]
        {
            0x04, // command id
            0x33, // sequence number
            0x00, // status: success
            0x13,
            0x00, // frame length
            0x0c,
            0x00, // payload length
            0x22, // device state
            0x0c, // request id
            0x03, // destination address mode: Ieee
            0x67,
            0x45,
            0x23,
            0x01,
            0x00,
            0x4b,
            0x12,
            0x00, // IEEE address 0x00124b0001234567
            0x0b, // destination endpoint
            0x01, // source endpoint
            0x00, // confirm status
        };

        var decoding = ApsDataConfirmCodec.Decode(frame);

        Assert.True(decoding.IsSuccessful);
        Assert.Equal(
            new ApsDataConfirm(
                SequenceNumber: 0x33,
                DeviceState: 0x22,
                RequestId: 0x0c,
                DestinationAddressMode: DeconzAddressMode.Ieee,
                DestinationShortAddress: null,
                DestinationIeeeAddress: new IeeeAddress(0x00124b0001234567),
                DestinationEndpoint: 0x0b,
                SourceEndpoint: 0x01,
                ConfirmStatus: 0x00
            ),
            decoding.Confirm
        );
    }
}
