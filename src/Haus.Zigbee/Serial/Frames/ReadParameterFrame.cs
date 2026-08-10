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
    private const int StatusByteCountInPayload = 1;

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

    // This response comes straight off the wire, so a truncated frame or a corrupted payload
    // length must produce a null result here rather than throw -- see ZclFrameHeaderCodec.Decode
    // for why an exception here would silently stop delivery to every other IndicationReceived
    // subscriber.
    public static ReadParameterResponse? Decode(ReadOnlySpan<byte> frame)
    {
        if (frame.Length <= ParameterIdIndex)
            return null;

        var payloadLength = BinaryPrimitives.ReadUInt16LittleEndian(frame[PayloadLengthIndex..]);
        if (payloadLength < StatusByteCountInPayload)
            return null;

        var valueLength = payloadLength - StatusByteCountInPayload;
        if (frame.Length < ValueIndex + valueLength)
            return null;

        var value = frame.Slice(ValueIndex, valueLength).ToArray();
        return new ReadParameterResponse(frame[StatusIndex], frame[ParameterIdIndex], value);
    }
}
