using Haus.Zigbee.Serial.Frames;
using Xunit;

namespace Haus.Zigbee.Tests.Serial.Frames;

public class ReadParameterFrameTests
{
    [Fact]
    public void GivenParameterIdWithNoArgumentsWhenEncodingRequestThenProducesHeaderPayloadLengthAndParameterId()
    {
        var request = new ReadParameterRequest(SequenceNumber: 0x10, ParameterId: 0x05, Arguments: []);

        var bytes = ReadParameterFrame.Encode(request);

        Assert.Equal(new byte[] { 0x0A, 0x10, 0x00, 0x08, 0x00, 0x01, 0x00, 0x05 }, bytes);
    }

    [Fact]
    public void GivenParameterIdWithArgumentBytesWhenEncodingRequestThenExtendsLengthsAndAppendsArguments()
    {
        var request = new ReadParameterRequest(SequenceNumber: 0x22, ParameterId: 0x09, Arguments: [0xAB]);

        var bytes = ReadParameterFrame.Encode(request);

        Assert.Equal(new byte[] { 0x0A, 0x22, 0x00, 0x09, 0x00, 0x02, 0x00, 0x09, 0xAB }, bytes);
    }

    [Fact]
    public void GivenSuccessfulResponseWithValueBytesWhenDecodingThenExposesStatusParameterIdAndRawValue()
    {
        byte[] frame = [0x0A, 0x33, 0x00, 0x0B, 0x00, 0x05, 0x00, 0x07, 0xDE, 0xAD, 0xBE, 0xEF];

        var response = ReadParameterFrame.Decode(frame);

        Assert.Equal(0x00, response.Status);
        Assert.Equal(0x07, response.ParameterId);
        Assert.Equal(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }, response.Value);
    }
}
