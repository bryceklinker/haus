using Haus.Zigbee.Zcl;
using Xunit;

namespace Haus.Zigbee.Tests.Zcl;

public class ZclFrameHeaderTests
{
    [Fact]
    public void WhenEncodingHeaderWithoutManufacturerCodeThenProducesFrameControlSequenceAndCommandBytes()
    {
        var header = new ZclFrameHeader(
            ZclFrameType.Global,
            ZclDirection.ClientToServer,
            DisableDefaultResponse: false,
            TransactionSequenceNumber: 0x42,
            CommandId: 0x00
        );

        var bytes = ZclFrameHeaderCodec.Encode(header);

        Assert.Equal(new byte[] { 0x00, 0x42, 0x00 }, bytes);
    }

    [Fact]
    public void WhenEncodingHeaderThenFrameControlPacksFrameTypeDirectionAndDisableDefaultResponseBits()
    {
        var header = new ZclFrameHeader(
            ZclFrameType.ClusterSpecific,
            ZclDirection.ServerToClient,
            DisableDefaultResponse: true,
            TransactionSequenceNumber: 0x11,
            CommandId: 0x0a
        );

        var bytes = ZclFrameHeaderCodec.Encode(header);

        Assert.Equal(new byte[] { 0b0001_1001, 0x11, 0x0a }, bytes);
    }

    [Fact]
    public void WhenEncodingHeaderWithManufacturerCodeThenSetsFlagAndInsertsLittleEndianCode()
    {
        var header = new ZclFrameHeader(
            ZclFrameType.Global,
            ZclDirection.ClientToServer,
            DisableDefaultResponse: false,
            TransactionSequenceNumber: 0x42,
            CommandId: 0x00,
            ManufacturerCode: 0x1234
        );

        var bytes = ZclFrameHeaderCodec.Encode(header);

        Assert.Equal(new byte[] { 0b0000_0100, 0x34, 0x12, 0x42, 0x00 }, bytes);
    }

    [Fact]
    public void WhenDecodingHeaderWithoutManufacturerCodeThenReadsFieldsAndReportsThreeBytesConsumed()
    {
        var frame = new byte[] { 0b0001_1001, 0x11, 0x0a, 0xaa, 0xbb };

        var decoding = ZclFrameHeaderCodec.Decode(frame);

        Assert.Equal(3, decoding.ByteLength);
        Assert.Equal(
            new ZclFrameHeader(
                ZclFrameType.ClusterSpecific,
                ZclDirection.ServerToClient,
                DisableDefaultResponse: true,
                TransactionSequenceNumber: 0x11,
                CommandId: 0x0a
            ),
            decoding.Header
        );
    }
}
