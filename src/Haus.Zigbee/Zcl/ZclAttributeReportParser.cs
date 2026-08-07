using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

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

    // 0xff in the length byte is the ZCL convention for an invalid/absent CharacterString value --
    // it carries no following string bytes at all, unlike every other length value.
    private const byte InvalidStringLength = 0xff;

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
        if (dataType == (byte)ZclDataType.CharacterString)
            return TryDecodeCharacterString(payload, offset, out value, out length);

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

    private static bool TryDecodeCharacterString(
        ReadOnlySpan<byte> payload,
        int offset,
        [NotNullWhen(true)] out ZclAttributeValue? value,
        out int length
    )
    {
        var stringLengthOffset = offset + 1;
        if (stringLengthOffset >= payload.Length)
        {
            value = null;
            length = 0;
            return false;
        }

        var stringLength = payload[stringLengthOffset];
        if (stringLength == InvalidStringLength)
        {
            value = new ZclAttributeValue(ZclDataType.CharacterString, RawValue: 0, StringValue: string.Empty);
            length = 2;
            return true;
        }

        var stringOffset = stringLengthOffset + 1;
        if (stringOffset + stringLength > payload.Length)
        {
            value = null;
            length = 0;
            return false;
        }

        var stringValue = Encoding.ASCII.GetString(payload.Slice(stringOffset, stringLength));
        value = new ZclAttributeValue(ZclDataType.CharacterString, RawValue: 0, stringValue);
        length = 2 + stringLength;
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
