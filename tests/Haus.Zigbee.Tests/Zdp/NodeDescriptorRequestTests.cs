using Haus.Zigbee.Zdp;
using Xunit;

namespace Haus.Zigbee.Tests.Zdp;

public class NodeDescriptorRequestTests
{
    [Fact]
    public void WhenEncodingRequestThenProducesTransactionSequenceNumberAndLittleEndianNetworkAddress()
    {
        var request = new NodeDescriptorRequest(TransactionSequenceNumber: 0x42, NetworkAddressOfInterest: 0x1a2b);

        var bytes = NodeDescriptorRequestCodec.Encode(request);

        Assert.Equal(new byte[] { 0x42, 0x2b, 0x1a }, bytes);
    }

    [Fact]
    public void WhenDecodingSuccessfulResponseWithReceiverOnWhenIdleSetThenIsSleepyIsFalse()
    {
        var payload = SuccessPayload(macCapabilityFlags: 0x08);

        var response = NodeDescriptorResponseCodec.Decode(payload);

        Assert.NotNull(response);
        Assert.Equal(0x42, response.TransactionSequenceNumber);
        Assert.Equal(ZdoStatus.Success, response.Status);
        Assert.Equal(0x1a2b, response.NetworkAddressOfInterest);
        Assert.False(response.IsSleepy);
    }

    [Fact]
    public void WhenDecodingSuccessfulResponseWithReceiverOnWhenIdleClearThenIsSleepyIsTrue()
    {
        var payload = SuccessPayload(macCapabilityFlags: 0x00);

        var response = NodeDescriptorResponseCodec.Decode(payload);

        Assert.NotNull(response);
        Assert.True(response.IsSleepy);
    }

    [Fact]
    public void WhenDecodingNonSuccessResponseThenRecoversStatusWithoutReadingFurtherBytesOrThrowing()
    {
        var payload = new byte[] { 0x42, (byte)ZdoStatus.DeviceNotFound };

        var response = NodeDescriptorResponseCodec.Decode(payload);

        Assert.NotNull(response);
        Assert.Equal(0x42, response.TransactionSequenceNumber);
        Assert.Equal(ZdoStatus.DeviceNotFound, response.Status);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public void WhenDecodingASuccessResponseTruncatedBeforeTheMacCapabilityFlagsByteThenReturnsNullInsteadOfThrowing(
        int length
    )
    {
        var payload = SuccessPayload(macCapabilityFlags: 0x08)[..length];

        var response = NodeDescriptorResponseCodec.Decode(payload);

        Assert.Null(response);
    }

    private static byte[] SuccessPayload(byte macCapabilityFlags)
    {
        return new byte[]
        {
            0x42, // TSN
            0x00, // Status: Success
            0x2b,
            0x1a, // NWKAddrOfInterest (u16 LE) => 0x1a2b
            0x00, // NodeDescriptor byte0: logical type + flags
            0x00, // NodeDescriptor byte1: APS flags + frequency band
            macCapabilityFlags, // NodeDescriptor byte2: MAC Capability Flags
        };
    }
}
