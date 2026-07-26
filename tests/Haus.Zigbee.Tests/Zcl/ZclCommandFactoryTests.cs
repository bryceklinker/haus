using Haus.Zigbee.Zcl;
using Xunit;

namespace Haus.Zigbee.Tests.Zcl;

public class ZclCommandFactoryTests
{
    [Fact]
    public void WhenEncodingCommandWithEmptyPayloadThenProducesJustTheClusterSpecificHeaderBytes()
    {
        var command = new ZclCommand(
            TransactionSequenceNumber: 0x42,
            CommandId: 0x00,
            Payload: new byte[0],
            DisableDefaultResponse: false
        );

        var bytes = ZclCommandFactory.Encode(command);

        Assert.Equal(new byte[] { 0b0000_0001, 0x42, 0x00 }, bytes);
    }

    [Fact]
    public void WhenEncodingCommandWithPayloadThenAppendsPayloadAfterTheHeaderCarryingTheCommandId()
    {
        var command = new ZclCommand(
            TransactionSequenceNumber: 0x11,
            CommandId: 0x04,
            Payload: new byte[] { 0xfe, 0x0a },
            DisableDefaultResponse: true
        );

        var bytes = ZclCommandFactory.Encode(command);

        Assert.Equal(new byte[] { 0b0001_0001, 0x11, 0x04, 0xfe, 0x0a }, bytes);
    }

    [Fact]
    public void WhenEncodingManufacturerSpecificCommandThenHeaderCarriesCommandIdAndManufacturerCodeWithPayloadFollowing()
    {
        var command = new ZclCommand(
            TransactionSequenceNumber: 0x05,
            CommandId: 0x0a,
            Payload: new byte[] { 0xbb, 0xcc },
            DisableDefaultResponse: false,
            ManufacturerCode: 0x100b
        );

        var bytes = ZclCommandFactory.Encode(command);

        var decoding = ZclFrameHeaderCodec.Decode(bytes);
        Assert.Equal(
            new ZclFrameHeader(
                ZclFrameType.ClusterSpecific,
                ZclDirection.ClientToServer,
                DisableDefaultResponse: false,
                TransactionSequenceNumber: 0x05,
                CommandId: 0x0a,
                ManufacturerCode: 0x100b
            ),
            decoding.Header
        );
        Assert.Equal(new byte[] { 0xbb, 0xcc }, bytes[decoding.ByteLength..]);
    }
}
