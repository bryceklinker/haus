using System;
using System.Collections.Generic;

namespace Haus.Zigbee.Zcl;

public sealed record ZclAttributeRecord(ushort AttributeId, ZclAttributeValue Value);

public sealed record ZclReportAttributesResult(IReadOnlyList<ZclAttributeRecord> Attributes, bool IsComplete);

public sealed record ZclReadAttributeRecord(ushort AttributeId, byte Status, ZclAttributeValue? Value);

public sealed record ZclReadAttributesResponseResult(
    IReadOnlyList<ZclReadAttributeRecord> Attributes,
    bool IsComplete
);

public static class ZclAttributeReportParser
{
    private const byte SuccessStatus = 0x00;

    public static ZclReportAttributesResult ParseReportAttributes(ReadOnlySpan<byte> payload)
    {
        var attributes = new List<ZclAttributeRecord>();
        var offset = 0;
        while (offset < payload.Length)
        {
            var attributeId = ReadUInt16(payload, offset);
            var dataType = payload[offset + 2];
            if (!ZclDataTypeWidths.TryGetWidth(dataType, out var width))
            {
                return new ZclReportAttributesResult(attributes, IsComplete: false);
            }

            var rawValue = ReadRawValue(payload, offset + 3, width);
            attributes.Add(new ZclAttributeRecord(attributeId, new ZclAttributeValue((ZclDataType)dataType, rawValue)));
            offset += 3 + width;
        }

        return new ZclReportAttributesResult(attributes, IsComplete: true);
    }

    public static ZclReadAttributesResponseResult ParseReadAttributesResponse(ReadOnlySpan<byte> payload)
    {
        var attributes = new List<ZclReadAttributeRecord>();
        var offset = 0;
        while (offset < payload.Length)
        {
            var attributeId = ReadUInt16(payload, offset);
            var status = payload[offset + 2];
            offset += 3;
            if (status != SuccessStatus)
            {
                attributes.Add(new ZclReadAttributeRecord(attributeId, status, Value: null));
                continue;
            }

            var dataType = payload[offset];
            if (!ZclDataTypeWidths.TryGetWidth(dataType, out var width))
            {
                return new ZclReadAttributesResponseResult(attributes, IsComplete: false);
            }

            var rawValue = ReadRawValue(payload, offset + 1, width);
            var value = new ZclAttributeValue((ZclDataType)dataType, rawValue);
            attributes.Add(new ZclReadAttributeRecord(attributeId, status, value));
            offset += 1 + width;
        }

        return new ZclReadAttributesResponseResult(attributes, IsComplete: true);
    }

    private static ushort ReadUInt16(ReadOnlySpan<byte> payload, int offset) =>
        (ushort)(payload[offset] | (payload[offset + 1] << 8));

    private static ulong ReadRawValue(ReadOnlySpan<byte> payload, int offset, int width)
    {
        var value = 0ul;
        for (var index = 0; index < width; index++)
        {
            value |= (ulong)payload[offset + index] << (index * 8);
        }

        return value;
    }
}
