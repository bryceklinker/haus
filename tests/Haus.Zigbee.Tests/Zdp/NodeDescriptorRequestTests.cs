using Haus.Zigbee.Zdp;
using Xunit;

namespace Haus.Zigbee.Tests.Zdp;

public class NodeDescriptorRequestTests
{
    [Fact]
    public void WhenEncodingRequestThenPrefixesTransactionSequenceNumberThenNetworkAddressLittleEndian()
    {
        var request = new NodeDescriptorRequest(TransactionSequenceNumber: 0x42, NetworkAddress: 0x1234);

        var encoded = NodeDescriptorRequestCodec.Encode(request);

        Assert.Equal(new byte[] { 0x42, 0x34, 0x12 }, encoded);
    }
}
