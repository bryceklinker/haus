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
}
