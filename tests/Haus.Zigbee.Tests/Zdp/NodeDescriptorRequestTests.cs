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

    [Fact]
    public void WhenDecodingSuccessfulResponseThenRecoversEveryFieldIncludingBitPackedFields()
    {
        var payload = new byte[]
        {
            0x42, // transaction sequence number
            0x00, // status: success
            0x34,
            0x12, // NWKAddrOfInterest = 0x1234
            0x31, // byte1: logicalType=Router(1), fragmentation bit set, reserved bit4 set (ignored)
            0x42, // byte2: apsFlags=2, frequencyBand=8
            0x8e, // macCapabilityFlags (raw)
            0x0b,
            0x10, // manufacturerCode = 0x100b
            0x52, // maxBufferSize = 82
            0x80,
            0x00, // maxIncomingTransferSize = 0x0080
            0x00,
            0x2c, // serverMask = 0x2c00 (raw)
            0x80,
            0x00, // maxOutgoingTransferSize = 0x0080
            0x00, // deprecated1 (raw)
        };

        var response = NodeDescriptorResponseCodec.Decode(payload);

        Assert.NotNull(response);
        Assert.Equal(0x42, response!.TransactionSequenceNumber);
        Assert.Equal(ZdoStatus.Success, response.Status);
        Assert.Equal((ushort)0x1234, response.NetworkAddress);
        Assert.NotNull(response.Descriptor);
        Assert.Equal(LogicalType.Router, response.Descriptor!.LogicalType);
        Assert.True(response.Descriptor.FragmentationSupported);
        Assert.Equal(2, response.Descriptor.ApsFlags);
        Assert.Equal(8, response.Descriptor.FrequencyBand);
        Assert.Equal(0x8e, response.Descriptor.MacCapabilityFlags);
        Assert.Equal((ushort)0x100b, response.Descriptor.ManufacturerCode);
        Assert.Equal(82, response.Descriptor.MaxBufferSize);
        Assert.Equal((ushort)0x0080, response.Descriptor.MaxIncomingTransferSize);
        Assert.Equal((ushort)0x2c00, response.Descriptor.ServerMask);
        Assert.Equal((ushort)0x0080, response.Descriptor.MaxOutgoingTransferSize);
        Assert.Equal(0x00, response.Descriptor.Deprecated1);
    }
}
