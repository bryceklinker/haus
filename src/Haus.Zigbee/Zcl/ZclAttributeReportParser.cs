using System;
using System.Buffers.Binary;
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
    private const int AttributeIdLength = 2;
    private const int StatusLength = 1;

    public static ZclReportAttributesResult ParseReportAttributes(ReadOnlySpan<byte> payload)
    {
        var attributes = new List<ZclAttributeRecord>();
        var offset = 0;
        while (offset < payload.Length)
        {
            if (offset + AttributeIdLength > payload.Length)
            {
                return new ZclReportAttributesResult(attributes, IsComplete: false);
            }

            var attributeId = BinaryPrimitives.ReadUInt16LittleEndian(payload[offset..]);
            if (!TryDecodeValue(payload, offset + AttributeIdLength, out var value, out var valueLength))
            {
                return new ZclReportAttributesResult(attributes, IsComplete: false);
            }

            attributes.Add(new ZclAttributeRecord(attributeId, value));
            offset += AttributeIdLength + valueLength;
        }

        return new ZclReportAttributesResult(attributes, IsComplete: true);
    }

    public static ZclReadAttributesResponseResult ParseReadAttributesResponse(ReadOnlySpan<byte> payload)
    {
        var attributes = new List<ZclReadAttributeRecord>();
        var offset = 0;
        while (offset < payload.Length)
        {
            if (offset + AttributeIdLength + StatusLength > payload.Length)
            {
                return new ZclReadAttributesResponseResult(attributes, IsComplete: false);
            }

            var attributeId = BinaryPrimitives.ReadUInt16LittleEndian(payload[offset..]);
            var status = payload[offset + AttributeIdLength];
            offset += AttributeIdLength + StatusLength;
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
        if (offset >= payload.Length)
        {
            value = null;
            length = 0;
            return false;
        }

        var dataType = payload[offset];
        if (!ZclDataTypeWidths.TryGetWidth(dataType, out var width) || offset + 1 + width > payload.Length)
        {
            value = null;
            length = 0;
            return false;
        }

        value = new ZclAttributeValue((ZclDataType)dataType, ReadRawValue(payload, offset + 1, width));
        length = 1 + width;
        return true;
    }

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
