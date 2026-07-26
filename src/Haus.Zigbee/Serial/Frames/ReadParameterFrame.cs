using System;
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

        var bytes = new List<byte>
        {
            CommandId,
            request.SequenceNumber,
            RequestStatus,
            (byte)(frameLength & 0xff),
            (byte)(frameLength >> 8),
            (byte)(payloadLength & 0xff),
            (byte)(payloadLength >> 8),
            request.ParameterId,
        };
        bytes.AddRange(request.Arguments);
        return bytes.ToArray();
    }

    public static ReadParameterResponse Decode(ReadOnlySpan<byte> frame)
    {
        var payloadLength = (ushort)(frame[PayloadLengthIndex] | (frame[PayloadLengthIndex + 1] << 8));
        var valueLength = payloadLength - 1;
        var value = frame.Slice(ValueIndex, valueLength).ToArray();

        return new ReadParameterResponse(frame[StatusIndex], frame[ParameterIdIndex], value);
    }
}
