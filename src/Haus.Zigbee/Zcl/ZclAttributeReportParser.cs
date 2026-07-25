using System;
using System.Collections.Generic;

namespace Haus.Zigbee.Zcl;

public sealed record ZclAttributeRecord(ushort AttributeId, ZclAttributeValue Value);

public sealed record ZclReportAttributesResult(IReadOnlyList<ZclAttributeRecord> Attributes, bool IsComplete);

public static class ZclAttributeReportParser
{
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
