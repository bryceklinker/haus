using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Simulator;
using Xunit;

namespace Haus.Zigbee.Simulator.Tests;

public class DeconzResponderTests
{
    [Fact]
    public void ReleaseAfterApsRequest_EnqueuesIndicationOnceThatDevicesRequestStepIsSent()
    {
        var responder = new DeconzResponder();
        const ushort networkAddress = 0x1234;
        var body = new IndicationBody(networkAddress, 0x01, 0x0104, 0x0000, [0x01]);
        responder.ReleaseAfterApsRequest(networkAddress, step: 0, _ => body);

        var beforeState = responder.HandleRequest([0x07, 0x00, 0x00, 0x00, 0x00, 0x00]);
        Assert.Equal((byte)0x00, beforeState[5]);

        responder.HandleRequest(ApsDataRequestFrameCodec.Encode(Request(networkAddress)));

        // Sending the request also makes a delivery confirm available (bit 0x04), on top of the
        // scripted indication (bit 0x08) -- a real coordinator confirms every request it sends.
        var afterState = responder.HandleRequest([0x07, 0x01, 0x00, 0x00, 0x00, 0x00]);
        Assert.Equal((byte)0x0c, afterState[5]);
    }

    [Fact]
    public void ReleaseAfterApsRequest_ScheduledForOneDevice_DoesNotFireWhenAnotherDevicesRequestArrives()
    {
        var responder = new DeconzResponder();
        const ushort deviceA = 0x1111;
        const ushort deviceB = 0x2222;
        var body = new IndicationBody(deviceA, 0x01, 0x0104, 0x0000, [0x01]);
        responder.ReleaseAfterApsRequest(deviceA, step: 0, _ => body);

        responder.HandleRequest(ApsDataRequestFrameCodec.Encode(Request(deviceB)));

        // deviceB's own request still makes its delivery confirm available (bit 0x04); only the
        // scripted indication (bit 0x08), which was registered for deviceA, must stay unset.
        var state = responder.HandleRequest([0x07, 0x00, 0x00, 0x00, 0x00, 0x00]);
        Assert.Equal((byte)0x04, state[5]);
    }

    private static ApsDataRequestFrame Request(ushort networkAddress)
    {
        return new ApsDataRequestFrame(
            SequenceNumber: 0,
            RequestId: 0,
            Destination: ApsDestination.Nwk(networkAddress, 0x01),
            ProfileId: 0x0104,
            ClusterId: 0x0006,
            SourceEndpoint: 0x01,
            AsduPayload: new byte[] { 0x00 },
            TxOptions: 0x00,
            Radius: 0x00
        );
    }

    [Fact]
    public void ApsDataRequestResponse_MakesAMatchingConfirmAvailableOnTheNextDeviceStatePoll()
    {
        var responder = new DeconzResponder();
        var request = new ApsDataRequestFrame(
            SequenceNumber: 0,
            RequestId: 0x2a,
            Destination: ApsDestination.Nwk(0x1234, 0x01),
            ProfileId: 0x0104,
            ClusterId: 0x0006,
            SourceEndpoint: 0x05,
            AsduPayload: new byte[] { 0x00 },
            TxOptions: 0x00,
            Radius: 0x00
        );

        responder.HandleRequest(ApsDataRequestFrameCodec.Encode(request));

        var state = responder.HandleRequest([0x07, 0x00, 0x00, 0x00, 0x00, 0x00]);
        Assert.Equal((byte)0x04, state[5]);

        var confirmResponse = responder.HandleRequest([0x04, 0x01, 0x00, 0x07, 0x00, 0x00, 0x00]);
        var decoding = ApsDataConfirmCodec.Decode(confirmResponse);

        Assert.True(decoding.IsSuccessful);
        Assert.Equal(request.RequestId, decoding.Confirm!.RequestId);
        Assert.Equal(request.Destination.ShortAddress, decoding.Confirm.DestinationShortAddress);
        Assert.Equal(request.Destination.Endpoint, decoding.Confirm.DestinationEndpoint);
        Assert.Equal(request.SourceEndpoint, decoding.Confirm.SourceEndpoint);
        Assert.Equal((byte)0x00, decoding.Confirm.ConfirmStatus);

        var stateAfterDrain = responder.HandleRequest([0x07, 0x02, 0x00, 0x00, 0x00, 0x00]);
        Assert.Equal((byte)0x00, stateAfterDrain[5]);
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
