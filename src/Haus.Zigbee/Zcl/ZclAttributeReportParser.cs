using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Haus.Zigbee.Zcl;

public record ZclAttributeRecord(ushort AttributeId, ZclAttributeValue Value);

public record ZclReportAttributesResult(IReadOnlyList<ZclAttributeRecord> Attributes, bool IsComplete);

public record ZclReadAttributeRecord(ushort AttributeId, byte Status, ZclAttributeValue? Value);

public record ZclReadAttributesResponseResult(IReadOnlyList<ZclReadAttributeRecord> Attributes, bool IsComplete);

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
            if (!TryDecodeValue(payload, offset + 2, out var value, out var valueLength))
            {
                return new ZclReportAttributesResult(attributes, IsComplete: false);
            }

            attributes.Add(new ZclAttributeRecord(attributeId, value));
            offset += 2 + valueLength;
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

            if (!TryDecodeValue(payload, offset, out var value, out var valueLength))
            {
                return new ZclReadAttributesResponseResult(attributes, IsComplete: false);
            }

            attributes.Add(new ZclReadAttributeRecord(attributeId, status, value));
            offset += valueLength;
        }

        return new ZclReadAttributesResponseResult(attributes, IsComplete: true);
    }

    private static bool TryDecodeValue(
        ReadOnlySpan<byte> payload,
        int offset,
        [NotNullWhen(true)] out ZclAttributeValue? value,
        out int length
    )
    {
        var dataType = payload[offset];
        if (!ZclDataTypeWidths.TryGetWidth(dataType, out var width))
        {
            value = null;
            length = 0;
            return false;
        }

        value = new ZclAttributeValue((ZclDataType)dataType, ReadRawValue(payload, offset + 1, width));
        length = 1 + width;
        return true;
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
