using Haus.Zigbee;
using Haus.Zigbee.Serial.Frames;
using Xunit;

namespace Haus.Zigbee.Tests.Serial.Frames;

public class ApsDataIndicationFrameTests
{
    [Fact]
    public void WhenDecodingNwkDestinationAndNwkSourceFrameThenReadsEveryField()
    {
        var frame = new byte[]
        {
            0x17, // command id
            0x05, // sequence number
            0x00, // status: success
            0x00,
            0x00, // frameLength
            0x00,
            0x00, // payloadLength
            0x2a, // deviceState
            0x02, // destAddrMode: Nwk
            0xcd,
            0xab, // destAddr16 = 0xabcd
            0x01, // destEndpoint
            0x02, // srcAddrMode: Nwk
            0x34,
            0x12, // srcAddr16 = 0x1234
            0x03, // srcEndpoint
            0x04,
            0x01, // profileId = 0x0104
            0x06,
            0x00, // clusterId = 0x0006
            0x03,
            0x00, // asduLength = 3
            0x11,
            0x22,
            0x33, // asduPayload
            0x00,
            0x00, // reserved
            0xff, // lqi = 255
            0x9c, // rssi = -100
        };

        var decoded = ApsDataIndicationFrameCodec.Decode(frame);

        Assert.NotNull(decoded);
        Assert.Equal(0x2a, decoded!.DeviceState);
        Assert.Equal(DeconzAddressMode.Nwk, decoded.DestinationAddressMode);
        Assert.Equal((ushort)0xabcd, decoded.DestinationNwkAddress);
        Assert.Null(decoded.DestinationIeeeAddress);
        Assert.Equal(0x01, decoded.DestinationEndpoint);
        Assert.Equal(DeconzAddressMode.Nwk, decoded.SourceAddressMode);
        Assert.Equal((ushort)0x1234, decoded.SourceNwkAddress);
        Assert.Null(decoded.SourceIeeeAddress);
        Assert.Equal(0x03, decoded.SourceEndpoint);
        Assert.Equal((ushort)0x0104, decoded.ProfileId);
        Assert.Equal((ushort)0x0006, decoded.ClusterId);
        Assert.Equal(new byte[] { 0x11, 0x22, 0x33 }, decoded.AsduPayload);
        Assert.Equal(255, decoded.LinkQualityIndicator);
        Assert.Equal((sbyte)-100, decoded.Rssi);
    }
}
