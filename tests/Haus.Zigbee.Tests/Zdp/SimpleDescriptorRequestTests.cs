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

    [Fact]
    public void WhenDecodingSuccessfulResponseThenRecoversAllDescriptorFieldsAndClusterListsInOrder()
    {
        var payload = new byte[]
        {
            0x05, // transaction sequence number
            0x00, // status: success
            0x34,
            0x12, // network address 0x1234
            0x0e, // descriptor length
            0x0b, // endpoint
            0x04,
            0x01, // profile id 0x0104
            0x00,
            0x01, // device id 0x0100
            0x01, // device version
            0x02, // in cluster count
            0x06,
            0x00, // in cluster 0x0006
            0x08,
            0x00, // in cluster 0x0008
            0x01, // out cluster count
            0x19,
            0x00, // out cluster 0x0019
        };

        var response = SimpleDescriptorCodec.DecodeResponse(payload);

        Assert.Equal(0x05, response.TransactionSequenceNumber);
        Assert.Equal(ZdoStatus.Success, response.Status);
        var descriptor = Assert.IsType<SimpleDescriptor>(response.Descriptor);
        Assert.Equal(0x1234, descriptor.NetworkAddress);
        Assert.Equal(0x0b, descriptor.Endpoint);
        Assert.Equal(0x0104, descriptor.ProfileId);
        Assert.Equal(0x0100, descriptor.DeviceId);
        Assert.Equal(0x01, descriptor.DeviceVersion);
        Assert.Equal(new ushort[] { 0x0006, 0x0008 }, descriptor.InClusters);
        Assert.Equal(new ushort[] { 0x0019 }, descriptor.OutClusters);
    }
}
