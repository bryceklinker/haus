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
}
