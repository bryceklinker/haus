using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace Haus.Zigbee.Serial.Frames;

public record ReadParameterRequest(byte SequenceNumber, byte ParameterId, byte[] Arguments);

public record ReadParameterResponse(byte Status, byte ParameterId, byte[] Value);

public static class ReadParameterFrame
{
    private const byte CommandId = 0x0A;
    private const byte RequestStatus = 0x00;
    private const int StatusIndex = 2;
    private const int PayloadLengthIndex = 5;
    private const int ParameterIdIndex = 7;
    private const int ValueIndex = 8;

    public static byte[] Encode(ReadParameterRequest request)
    {
        var payloadLength = (ushort)(1 + request.Arguments.Length);
        var frameLength = (ushort)(8 + request.Arguments.Length);

        List<byte> bytes = [CommandId, request.SequenceNumber, RequestStatus];
        LittleEndian.Write(bytes, frameLength);
        LittleEndian.Write(bytes, payloadLength);
        bytes.Add(request.ParameterId);
        bytes.AddRange(request.Arguments);
        return bytes.ToArray();
    }

    public static ReadParameterResponse Decode(ReadOnlySpan<byte> frame)
    {
        var payloadLength = BinaryPrimitives.ReadUInt16LittleEndian(frame[PayloadLengthIndex..]);
        var valueLength = payloadLength - 1;
        var value = frame.Slice(ValueIndex, valueLength).ToArray();

        return new ReadParameterResponse(frame[StatusIndex], frame[ParameterIdIndex], value);
    }
}
