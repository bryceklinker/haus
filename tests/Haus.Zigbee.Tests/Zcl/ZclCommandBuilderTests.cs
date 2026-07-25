using Haus.Zigbee.Zcl;
using Xunit;

namespace Haus.Zigbee.Tests.Zcl;

public class ZclCommandBuilderTests
{
    [Fact]
    public void WhenBuildingCommandWithEmptyPayloadThenProducesJustTheClusterSpecificHeaderBytes()
    {
        var command = new ZclCommand(
            TransactionSequenceNumber: 0x42,
            CommandId: 0x00,
            Payload: new byte[0],
            DisableDefaultResponse: false
        );

        var bytes = ZclCommandBuilder.Build(command);

        Assert.Equal(new byte[] { 0b0000_0001, 0x42, 0x00 }, bytes);
    }
}
