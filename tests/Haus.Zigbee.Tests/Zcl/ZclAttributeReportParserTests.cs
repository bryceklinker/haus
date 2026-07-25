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

    [Fact]
    public void WhenParsingReportAttributesWithBoolValueThenDecodesTheBooleanFromASingleByte()
    {
        var payload = new byte[] { 0x00, 0x00, 0x10, 0x01 };

        var result = ZclAttributeReportParser.ParseReportAttributes(payload);

        Assert.True(result.IsComplete);
        var attribute = Assert.Single(result.Attributes);
        Assert.Equal(ZclDataType.Bool, attribute.Value.DataType);
        Assert.True(attribute.Value.AsBool());
    }

    [Fact]
    public void WhenParsingReadAttributesResponseWithSuccessfulInt16RecordThenDecodesSignedValueAfterStatus()
    {
        var payload = new byte[] { 0x00, 0x00, 0x00, 0x29, 0x9c, 0xff };

        var result = ZclAttributeReportParser.ParseReadAttributesResponse(payload);

        Assert.True(result.IsComplete);
        var attribute = Assert.Single(result.Attributes);
        Assert.Equal(0x0000, attribute.AttributeId);
        Assert.Equal(0x00, attribute.Status);
        Assert.NotNull(attribute.Value);
        Assert.Equal(ZclDataType.Int16, attribute.Value.DataType);
        Assert.Equal(-100, attribute.Value.AsSigned());
    }

    [Fact]
    public void WhenParsingReadAttributesResponseWithFailedStatusRecordThenSkipsDataAndContinuesToNextRecord()
    {
        var payload = new byte[] { 0x01, 0x00, 0x86, 0x00, 0x00, 0x00, 0x29, 0x9c, 0xff };

        var result = ZclAttributeReportParser.ParseReadAttributesResponse(payload);

        Assert.True(result.IsComplete);
        Assert.Equal(2, result.Attributes.Count);

        var failed = result.Attributes[0];
        Assert.Equal(0x0001, failed.AttributeId);
        Assert.Equal(0x86, failed.Status);
        Assert.Null(failed.Value);

        var succeeded = result.Attributes[1];
        Assert.Equal(0x0000, succeeded.AttributeId);
        Assert.Equal(0x00, succeeded.Status);
        Assert.NotNull(succeeded.Value);
        Assert.Equal(-100, succeeded.Value.AsSigned());
    }

    [Fact]
    public void WhenParsingReportAttributesWithUnsupportedDataTypeThenReturnsRecordsDecodedBeforeItWithoutThrowing()
    {
        var payload = new byte[] { 0x00, 0x00, 0x21, 0x34, 0x12, 0x01, 0x00, 0x42, 0x03, 0x61, 0x62, 0x63 };

        var result = ZclAttributeReportParser.ParseReportAttributes(payload);

        Assert.False(result.IsComplete);
        var attribute = Assert.Single(result.Attributes);
        Assert.Equal(0x0000, attribute.AttributeId);
        Assert.Equal(ZclDataType.Uint16, attribute.Value.DataType);
    }
}
