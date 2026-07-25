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

public sealed record ZclFrameHeader(
    ZclFrameType FrameType,
    ZclDirection Direction,
    bool DisableDefaultResponse,
    byte TransactionSequenceNumber,
    byte CommandId,
    ushort? ManufacturerCode = null
);

public sealed record ZclFrameHeaderDecoding(ZclFrameHeader Header, int ByteLength);

public static class ZclFrameHeaderCodec
{
    private const int ManufacturerSpecificBit = 1 << 2;

    public static byte[] Encode(ZclFrameHeader header)
    {
        var bytes = new List<byte> { BuildFrameControl(header) };
        if (header.ManufacturerCode is ushort manufacturerCode)
        {
            bytes.Add((byte)(manufacturerCode & 0xff));
            bytes.Add((byte)(manufacturerCode >> 8));
        }

        bytes.Add(header.TransactionSequenceNumber);
        bytes.Add(header.CommandId);
        return bytes.ToArray();
    }

    public static ZclFrameHeaderDecoding Decode(ReadOnlySpan<byte> frame)
    {
        var frameControl = frame[0];
        var isManufacturerSpecific = (frameControl & ManufacturerSpecificBit) != 0;
        var afterManufacturer = isManufacturerSpecific ? 3 : 1;
        var manufacturerCode = isManufacturerSpecific
            ? (ushort?)(frame[1] | (frame[2] << 8))
            : null;

        var header = new ZclFrameHeader(
            (ZclFrameType)(frameControl & 0b11),
            (ZclDirection)((frameControl >> 3) & 1),
            DisableDefaultResponse: (frameControl & (1 << 4)) != 0,
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
        frameControl |= (int)header.Direction << 3;
        frameControl |= header.DisableDefaultResponse ? 1 << 4 : 0;
        return (byte)frameControl;
    }
}
