using System;
using System.Collections.Generic;

namespace Haus.Zigbee.Zcl;

public enum ZclFrameType : byte
{
    Global = 0b00,
    ClusterSpecific = 0b01,
}

public enum ZclDirection : byte
{
    ClientToServer = 0,
    ServerToClient = 1,
}

public record ZclFrameHeader(
    ZclFrameType FrameType,
    ZclDirection Direction,
    bool DisableDefaultResponse,
    byte TransactionSequenceNumber,
    byte CommandId,
    ushort? ManufacturerCode = null
);

public record ZclFrameHeaderDecoding(ZclFrameHeader Header, int ByteLength);

public static class ZclFrameHeaderCodec
{
    private const int FrameTypeMask = 0b11;
    private const int ManufacturerSpecificBit = 1 << 2;
    private const int DirectionBitShift = 3;
    private const int DisableDefaultResponseBit = 1 << 4;

    public static byte[] Encode(ZclFrameHeader header)
    {
        List<byte> bytes = [BuildFrameControl(header)];
        if (header.ManufacturerCode is ushort manufacturerCode)
        {
            bytes.Add((byte)(manufacturerCode & 0xff));
            bytes.Add((byte)(manufacturerCode >> 8));
        }

        bytes.Add(header.TransactionSequenceNumber);
        bytes.Add(header.CommandId);
        return bytes.ToArray();
    }

    // The ASDU payload this decodes comes straight off the wire from a real device, not just from
    // our own encoder -- a short or truncated frame must produce a null result here rather than an
    // IndexOutOfRangeException, since a thrown exception on a shared IndicationReceived subscriber
    // silently stops every other subscriber (DeviceInterview included) from seeing the event too.
    public static ZclFrameHeaderDecoding? Decode(ReadOnlySpan<byte> frame)
    {
        if (frame.Length < 1)
            return null;

        var frameControl = frame[0];
        var isManufacturerSpecific = (frameControl & ManufacturerSpecificBit) != 0;
        var afterManufacturer = isManufacturerSpecific ? 3 : 1;
        if (frame.Length < afterManufacturer + 2)
            return null;

        var manufacturerCode = isManufacturerSpecific ? (ushort?)(frame[1] | (frame[2] << 8)) : null;

        var header = new ZclFrameHeader(
            (ZclFrameType)(frameControl & FrameTypeMask),
            (ZclDirection)((frameControl >> DirectionBitShift) & 1),
            DisableDefaultResponse: (frameControl & DisableDefaultResponseBit) != 0,
            TransactionSequenceNumber: frame[afterManufacturer],
            CommandId: frame[afterManufacturer + 1],
            ManufacturerCode: manufacturerCode
        );
        return new ZclFrameHeaderDecoding(header, afterManufacturer + 2);
    }

    private static byte BuildFrameControl(ZclFrameHeader header)
    {
        var frameControl = (int)header.FrameType;
        frameControl |= header.ManufacturerCode is null ? 0 : ManufacturerSpecificBit;
        frameControl |= (int)header.Direction << DirectionBitShift;
        frameControl |= header.DisableDefaultResponse ? DisableDefaultResponseBit : 0;
        return (byte)frameControl;
    }
}
