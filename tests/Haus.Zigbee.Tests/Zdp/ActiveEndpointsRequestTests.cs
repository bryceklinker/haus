using Haus.Zigbee.Zdp;
using Xunit;

namespace Haus.Zigbee.Tests.Zdp;

public class ActiveEndpointsRequestTests
{
    [Fact]
    public void WhenEncodingRequestThenProducesTransactionSequenceNumberFollowedByLittleEndianNetworkAddress()
    {
        var request = new ActiveEndpointsRequest(TransactionSequenceNumber: 0x42, NetworkAddress: 0x1234);

        var bytes = ActiveEndpointsRequestCodec.Encode(request);

        Assert.Equal(new byte[] { 0x42, 0x34, 0x12 }, bytes);
    }
}
