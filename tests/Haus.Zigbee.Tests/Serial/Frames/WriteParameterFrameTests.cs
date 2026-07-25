using Haus.Zigbee.Serial.Frames;
using Xunit;

namespace Haus.Zigbee.Tests.Serial.Frames;

public class WriteParameterFrameTests
{
    [Fact]
    public void GivenParameterIdWithValueBytesWhenEncodingRequestThenProducesHeaderLengthsParameterIdAndValue()
    {
        var request = new WriteParameterRequest(SequenceNumber: 0x12, ParameterId: 0x26, Value: [0x1E]);

        var bytes = WriteParameterFrame.Encode(request);

        Assert.Equal(new byte[] { 0x0B, 0x12, 0x00, 0x09, 0x00, 0x02, 0x00, 0x26, 0x1E }, bytes);
    }

    [Fact]
    public void GivenSuccessfulResponseWhenDecodingThenExposesStatusAndParameterId()
    {
        byte[] frame = [0x0B, 0x12, 0x00, 0x08, 0x00, 0x01, 0x00, 0x26];

        var response = WriteParameterFrame.Decode(frame);

        Assert.Equal(0x00, response.Status);
        Assert.Equal(0x26, response.ParameterId);
    }
}
