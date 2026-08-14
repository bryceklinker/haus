using Haus.Zigbee.Models;
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

    [Fact]
    public void WhenDecodingIeeeDestinationFrameThenReadsEightByteIeeeAddressAndEndpoint()
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
            0x03, // destAddrMode: Ieee
            0x01,
            0x02,
            0x03,
            0x04,
            0x05,
            0x06,
            0x07,
            0x08, // destIeee = 0x0807060504030201
            0x0a, // destEndpoint
            0x02, // srcAddrMode: Nwk
            0x34,
            0x12, // srcAddr16 = 0x1234
            0x03, // srcEndpoint
            0x04,
            0x01, // profileId = 0x0104
            0x06,
            0x00, // clusterId = 0x0006
            0x01,
            0x00, // asduLength = 1
            0xaa, // asduPayload
            0x00,
            0x00, // reserved
            0x7f, // lqi = 127
            0x9c, // rssi = -100
        };

        var decoded = ApsDataIndicationFrameCodec.Decode(frame);

        Assert.NotNull(decoded);
        Assert.Equal(DeconzAddressMode.Ieee, decoded!.DestinationAddressMode);
        Assert.Null(decoded.DestinationNwkAddress);
        Assert.Equal(new IeeeAddress(0x0807060504030201), decoded.DestinationIeeeAddress);
        Assert.Equal(0x0a, decoded.DestinationEndpoint);
        Assert.Equal((ushort)0x1234, decoded.SourceNwkAddress);
        Assert.Equal(new byte[] { 0xaa }, decoded.AsduPayload);
        Assert.Equal(127, decoded.LinkQualityIndicator);
    }

    [Fact]
    public void WhenDecodingNwkAndIeeeSourceFrameThenReadsBothSourceAddresses()
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
            0x04, // srcAddrMode: NwkAndIeee
            0x34,
            0x12, // srcAddr16 = 0x1234
            0x11,
            0x22,
            0x33,
            0x44,
            0x55,
            0x66,
            0x77,
            0x88, // srcIeee = 0x8877665544332211
            0x03, // srcEndpoint
            0x04,
            0x01, // profileId = 0x0104
            0x06,
            0x00, // clusterId = 0x0006
            0x02,
            0x00, // asduLength = 2
            0xde,
            0xad, // asduPayload
            0x00,
            0x00, // reserved
            0xf0, // lqi = 240
            0x9c, // rssi = -100
        };

        var decoded = ApsDataIndicationFrameCodec.Decode(frame);

        Assert.NotNull(decoded);
        Assert.Equal(DeconzAddressMode.NwkAndIeee, decoded!.SourceAddressMode);
        Assert.Equal((ushort)0x1234, decoded.SourceNwkAddress);
        Assert.Equal(new IeeeAddress(0x8877665544332211), decoded.SourceIeeeAddress);
        Assert.Equal(0x03, decoded.SourceEndpoint);
        Assert.Equal(new byte[] { 0xde, 0xad }, decoded.AsduPayload);
        Assert.Equal(240, decoded.LinkQualityIndicator);
    }

    [Fact]
    public void WhenDecodingGroupDestinationFrameThenStillReadsDestinationEndpoint()
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
            0x01, // destAddrMode: Group
            0xcd,
            0xab, // destGroup = 0xabcd
            0x07, // destEndpoint (present even for Group)
            0x02, // srcAddrMode: Nwk
            0x34,
            0x12, // srcAddr16 = 0x1234
            0x03, // srcEndpoint
            0x04,
            0x01, // profileId = 0x0104
            0x06,
            0x00, // clusterId = 0x0006
            0x01,
            0x00, // asduLength = 1
            0xaa, // asduPayload
            0x00,
            0x00, // reserved
            0x64, // lqi = 100
            0x9c, // rssi = -100
        };

        var decoded = ApsDataIndicationFrameCodec.Decode(frame);

        Assert.NotNull(decoded);
        Assert.Equal(DeconzAddressMode.Group, decoded!.DestinationAddressMode);
        Assert.Equal((ushort)0xabcd, decoded.DestinationNwkAddress);
        Assert.Equal(0x07, decoded.DestinationEndpoint);
        Assert.Equal(0x03, decoded.SourceEndpoint);
    }

    [Fact]
    public void WhenDecodingFrameWithoutTrailingRssiByteThenRssiIsAbsent()
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
            0x01,
            0x00, // asduLength = 1
            0xaa, // asduPayload
            0x00,
            0x00, // reserved
            0xff, // lqi = 255 (no rssi byte follows)
        };

        var decoded = ApsDataIndicationFrameCodec.Decode(frame);

        Assert.NotNull(decoded);
        Assert.Equal(255, decoded!.LinkQualityIndicator);
        Assert.Null(decoded.Rssi);
    }

    [Fact]
    public void WhenDecodingFrameWithNonSuccessStatusThenReturnsNull()
    {
        var frame = new byte[]
        {
            0x17, // command id
            0x05, // sequence number
            0x01, // status: not success
            0x00,
            0x00, // frameLength
            0x00,
            0x00, // payloadLength
            0x2a, // deviceState
        };

        var decoded = ApsDataIndicationFrameCodec.Decode(frame);

        Assert.Null(decoded);
    }

    [Fact]
    public void WhenDecodingFrameWithUnsupportedSourceAddressModeThenReturnsNull()
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
            0x03, // srcAddrMode: Ieee (unsupported for source)
        };

        var decoded = ApsDataIndicationFrameCodec.Decode(frame);

        Assert.Null(decoded);
    }

    // Progressively cuts off the valid Nwk/Nwk frame from WhenDecodingNwkDestinationAndNwkSourceFrameThenReadsEveryField
    // before each field is fully present, plus one case (23) where asduLength claims more payload than remains.
    private static readonly byte[] ValidNwkFrame =
    [
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
    ];

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(21)]
    [InlineData(23)]
    public void WhenDecodingAFrameTruncatedBeforeItsFieldsAreCompleteThenReturnsNullInsteadOfThrowing(int length)
    {
        var frame = ValidNwkFrame[..length];

        var decoded = ApsDataIndicationFrameCodec.Decode(frame);

        Assert.Null(decoded);
    }
}
