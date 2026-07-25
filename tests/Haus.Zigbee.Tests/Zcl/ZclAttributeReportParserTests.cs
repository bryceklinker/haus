using Haus.Zigbee.Zcl;
using Xunit;

namespace Haus.Zigbee.Tests.Zcl;

public class ZclAttributeReportParserTests
{
    [Fact]
    public void WhenParsingReportAttributesWithUint16ValueThenReadsAttributeIdTypeAndLittleEndianValue()
    {
        var payload = new byte[] { 0x00, 0x00, 0x21, 0x34, 0x12 };

        var result = ZclAttributeReportParser.ParseReportAttributes(payload);

        Assert.True(result.IsComplete);
        var attribute = Assert.Single(result.Attributes);
        Assert.Equal(0x0000, attribute.AttributeId);
        Assert.Equal(ZclDataType.Uint16, attribute.Value.DataType);
        Assert.Equal(0x1234ul, attribute.Value.AsUnsigned());
    }
}
