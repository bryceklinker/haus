using Haus.Zigbee.Zcl;
using Xunit;

namespace Haus.Zigbee.Tests.Zcl;

public class ZclReadAttributesRequestTests
{
    [Fact]
    public void WhenEncodingSingleAttributeThenAppendsGlobalHeaderAndLittleEndianAttributeId()
    {
        var request = new ZclReadAttributesRequest(
            TransactionSequenceNumber: 0x42,
            AttributeIds: new ushort[] { 0x0005 }
        );

        var bytes = ZclReadAttributesRequestCodec.Encode(request);

        Assert.Equal(new byte[] { 0x00, 0x42, 0x00, 0x05, 0x00 }, bytes);
    }

    [Fact]
    public void WhenEncodingMultipleAttributesThenAppendsEachLittleEndianInOrderWithNoCountPrefix()
    {
        var request = new ZclReadAttributesRequest(
            TransactionSequenceNumber: 0x07,
            AttributeIds: new ushort[] { 0x0004, 0x0005 }
        );

        var bytes = ZclReadAttributesRequestCodec.Encode(request);

        Assert.Equal(new byte[] { 0x00, 0x07, 0x00, 0x04, 0x00, 0x05, 0x00 }, bytes);
    }

    [Fact]
    public void WhenEncodingRequestThenDecodedHeaderIsGlobalClientToServerReadAttributesCommand()
    {
        var request = new ZclReadAttributesRequest(
            TransactionSequenceNumber: 0x07,
            AttributeIds: new ushort[] { 0x0004 }
        );

        var bytes = ZclReadAttributesRequestCodec.Encode(request);

        var header = ZclFrameHeaderCodec.Decode(bytes).Header;
        Assert.Equal(ZclFrameType.Global, header.FrameType);
        Assert.Equal(ZclDirection.ClientToServer, header.Direction);
        Assert.Equal(0x00, header.CommandId);
    }
}
