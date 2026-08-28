using Haus.Zigbee.Models;
using Haus.Zigbee.Zdp;
using Xunit;

namespace Haus.Zigbee.Tests.Zdp;

public class NwkAddrRequestTests
{
    [Fact]
    public void WhenEncodingRequestThenProducesTransactionSequenceNumberLittleEndianIeeeAddressSingleResponseTypeAndZeroStartIndex()
    {
        var request = new NwkAddrRequest(
            TransactionSequenceNumber: 0x42,
            IeeeAddress: new IeeeAddress(0x00124b0001aabbcc)
        );

        var bytes = NwkAddrRequestCodec.Encode(request);

        Assert.Equal(new byte[] { 0x42, 0xcc, 0xbb, 0xaa, 0x01, 0x00, 0x4b, 0x12, 0x00, 0x00, 0x00 }, bytes);
    }

    [Fact]
    public void WhenDecodingSuccessfulResponseThenRecoversTransactionSequenceNumberIeeeAddressAndLittleEndianNetworkAddress()
    {
        var payload = new byte[] { 0x42, 0x00, 0xcc, 0xbb, 0xaa, 0x01, 0x00, 0x4b, 0x12, 0x00, 0x2b, 0x1a };

        var response = NwkAddrResponseCodec.Decode(payload);

        Assert.NotNull(response);
        Assert.Equal(0x42, response.TransactionSequenceNumber);
        Assert.Equal(ZdoStatus.Success, response.Status);
        Assert.Equal(new IeeeAddress(0x00124b0001aabbcc), response.IeeeAddress);
        Assert.Equal(0x1a2b, response.NetworkAddress);
    }

    [Fact]
    public void WhenDecodingSuccessfulResponseWithTrailingExtendedResponseBytesThenIgnoresThemAndRecoversTheFixedPrefix()
    {
        var payload = new byte[]
        {
            0x42,
            0x00,
            0xcc,
            0xbb,
            0xaa,
            0x01,
            0x00,
            0x4b,
            0x12,
            0x00,
            0x2b,
            0x1a,
            0x02,
            0x00,
            0x34,
            0x12,
        };

        var response = NwkAddrResponseCodec.Decode(payload);

        Assert.NotNull(response);
        Assert.Equal(0x1a2b, response.NetworkAddress);
    }

    [Fact]
    public void WhenDecodingNonSuccessResponseThenRecoversStatusWithoutReadingFurtherBytesOrThrowing()
    {
        var payload = new byte[] { 0x42, (byte)ZdoStatus.DeviceNotFound };

        var response = NwkAddrResponseCodec.Decode(payload);

        Assert.NotNull(response);
        Assert.Equal(0x42, response.TransactionSequenceNumber);
        Assert.Equal(ZdoStatus.DeviceNotFound, response.Status);
    }

    private static readonly byte[] ValidSuccessPayload =
    [
        0x42,
        0x00,
        0xcc,
        0xbb,
        0xaa,
        0x01,
        0x00,
        0x4b,
        0x12,
        0x00,
        0x2b,
        0x1a,
    ];

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(10)]
    [InlineData(11)]
    public void WhenDecodingASuccessResponseTruncatedBeforeItsFieldsAreCompleteThenReturnsNullInsteadOfThrowing(
        int length
    )
    {
        var payload = ValidSuccessPayload[..length];

        var response = NwkAddrResponseCodec.Decode(payload);

        Assert.Null(response);
    }
}
