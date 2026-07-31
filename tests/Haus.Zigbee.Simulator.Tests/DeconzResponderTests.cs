using Haus.Zigbee.Simulator;
using Xunit;

namespace Haus.Zigbee.Simulator.Tests;

public class DeconzResponderTests
{
    [Fact]
    public void ReleaseAfterApsRequest_EnqueuesIndicationOnceThatRequestIndexIsSent()
    {
        var responder = new DeconzResponder();
        var body = new IndicationBody(0x1234, 0x01, 0x0104, 0x0000, [0x01]);
        responder.ReleaseAfterApsRequest(0, _ => body);

        var beforeState = responder.HandleRequest([0x07, 0x00, 0x00, 0x00, 0x00, 0x00]);
        Assert.Equal((byte)0x00, beforeState[5]);

        responder.HandleRequest([0x12, 0x00, 0x02, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00]);

        var afterState = responder.HandleRequest([0x07, 0x01, 0x00, 0x00, 0x00, 0x00]);
        Assert.Equal((byte)0x08, afterState[5]);
    }

    [Fact]
    public void AllocateNetworkAddress_NeverReturnsTheSameAddressTwice()
    {
        var responder = new DeconzResponder();

        var addresses = new System.Collections.Generic.HashSet<ushort>();
        for (var i = 0; i < 1000; i++)
            Assert.True(addresses.Add(responder.AllocateNetworkAddress()));
    }

    [Fact]
    public void AllocateNetworkAddress_NeverReturnsTheCoordinatorsOwnAddress()
    {
        var responder = new DeconzResponder();

        Assert.NotEqual((ushort)0x0000, responder.AllocateNetworkAddress());
    }

    [Theory]
    [InlineData(0x0a, "ReadParameter")]
    [InlineData(0x0b, "WriteParameter")]
    [InlineData(0x07, "DeviceState")]
    [InlineData(0x17, "ReadIndication")]
    [InlineData(0x12, "ApsDataRequest")]
    [InlineData(0xff, "Unknown(0xFF)")]
    public void DescribeCommand_ReturnsAReadableNameForKnownCommandsAndFallsBackForUnknownOnes(
        byte commandId,
        string expected
    )
    {
        Assert.Equal(expected, DeconzResponder.DescribeCommand(commandId));
    }
}
