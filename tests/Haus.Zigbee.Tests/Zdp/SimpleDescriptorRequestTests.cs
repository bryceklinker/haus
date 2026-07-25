using Haus.Zigbee.Zdp;
using Xunit;

namespace Haus.Zigbee.Tests.Zdp;

public class SimpleDescriptorRequestTests
{
    [Fact]
    public void WhenEncodingRequestThenProducesTsnNetworkAddressLittleEndianAndEndpoint()
    {
        var request = new SimpleDescriptorRequest(
            TransactionSequenceNumber: 0x01,
            NetworkAddress: 0x1234,
            Endpoint: 0x0b
        );

        var bytes = SimpleDescriptorCodec.EncodeRequest(request);

        Assert.Equal(new byte[] { 0x01, 0x34, 0x12, 0x0b }, bytes);
    }
}
