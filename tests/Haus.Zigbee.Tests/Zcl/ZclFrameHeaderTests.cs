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
}
