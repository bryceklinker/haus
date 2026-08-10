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

    [Fact]
    public void WhenDecodingSuccessfulResponseThenRecoversTransactionSequenceNumberNetworkAddressAndEndpointList()
    {
        var payload = new byte[] { 0x42, 0x00, 0x34, 0x12, 0x02, 0x01, 0x0b };

        var response = ActiveEndpointsResponseCodec.Decode(payload);

        Assert.NotNull(response);
        Assert.Equal(0x42, response.TransactionSequenceNumber);
        Assert.Equal(ZdoStatus.Success, response.Status);
        Assert.Equal(0x1234, response.NetworkAddress);
        Assert.Equal(new byte[] { 0x01, 0x0b }, response.EndpointIds);
    }

    [Fact]
    public void WhenDecodingNonSuccessResponseThenRecoversStatusWithoutReadingFurtherBytesOrThrowing()
    {
        var payload = new byte[] { 0x42, (byte)ZdoStatus.DeviceNotFound };

        var response = ActiveEndpointsResponseCodec.Decode(payload);

        Assert.NotNull(response);
        Assert.Equal(0x42, response.TransactionSequenceNumber);
        Assert.Equal(ZdoStatus.DeviceNotFound, response.Status);
        Assert.Empty(response.EndpointIds);
    }

    private static readonly byte[] ValidSuccessPayload = [0x42, 0x00, 0x34, 0x12, 0x02, 0x01, 0x0b];

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(6)]
    public void WhenDecodingASuccessResponseTruncatedBeforeItsFieldsAreCompleteThenReturnsNullInsteadOfThrowing(
        int length
    )
    {
        var payload = ValidSuccessPayload[..length];

        var response = ActiveEndpointsResponseCodec.Decode(payload);

        Assert.Null(response);
    }
}
