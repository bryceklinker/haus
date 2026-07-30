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
        responder.ReleaseAfterApsRequest(0, body);

        var beforeState = responder.HandleRequest([0x07, 0x00, 0x00, 0x00, 0x00, 0x00]);
        Assert.Equal((byte)0x00, beforeState[5]);

        responder.HandleRequest([0x12, 0x00, 0x02, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00]);

        var afterState = responder.HandleRequest([0x07, 0x01, 0x00, 0x00, 0x00, 0x00]);
        Assert.Equal((byte)0x08, afterState[5]);
    }
}
