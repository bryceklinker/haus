using Haus.Zigbee.Serial.Frames;
using Xunit;

namespace Haus.Zigbee.Tests.Serial.Frames;

public class ApsDataRequestFrameTests
{
    [Fact]
    public void WhenEncodingGroupModeRequestThenOmitsEndpointByteAndMatchesLengthFormulas()
    {
        var frame = new ApsDataRequestFrame(
            SequenceNumber: 0x05,
            RequestId: 0x2A,
            Destination: ApsDestination.Group(0xABCD),
            ProfileId: 0x0104,
            ClusterId: 0x0006,
            SourceEndpoint: 0x01,
            AsduPayload: new byte[] { 0x11, 0x22 },
            TxOptions: 0x04,
            Radius: 0x00
        );

        var encoded = ApsDataRequestFrameCodec.Encode(frame);

        Assert.Equal(
            new byte[]
            {
                0x12,
                0x05,
                0x00,
                0x17,
                0x00,
                0x10,
                0x00,
                0x2A,
                0x00,
                0x01,
                0xCD,
                0xAB,
                0x04,
                0x01,
                0x06,
                0x00,
                0x01,
                0x02,
                0x00,
                0x11,
                0x22,
                0x04,
                0x00,
            },
            encoded
        );
    }

    [Fact]
    public void WhenEncodingNwkModeRequestThenIncludesEndpointByteAfterNetworkAddress()
    {
        var frame = new ApsDataRequestFrame(
            SequenceNumber: 0x07,
            RequestId: 0x33,
            Destination: ApsDestination.Nwk(0x1234, endpoint: 0x0B),
            ProfileId: 0x0104,
            ClusterId: 0x0006,
            SourceEndpoint: 0x01,
            AsduPayload: new byte[] { 0xAA },
            TxOptions: 0x04,
            Radius: 0x00
        );

        var encoded = ApsDataRequestFrameCodec.Encode(frame);

        Assert.Equal(
            new byte[]
            {
                0x12,
                0x07,
                0x00,
                0x17,
                0x00,
                0x10,
                0x00,
                0x33,
                0x00,
                0x02,
                0x34,
                0x12,
                0x0B,
                0x04,
                0x01,
                0x06,
                0x00,
                0x01,
                0x01,
                0x00,
                0xAA,
                0x04,
                0x00,
            },
            encoded
        );
    }
}
