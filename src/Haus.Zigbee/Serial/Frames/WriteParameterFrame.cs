using System;
using System.Collections.Generic;

namespace Haus.Zigbee.Serial.Frames;

public record WriteParameterRequest(byte SequenceNumber, byte ParameterId, byte[] Value);

public record WriteParameterResponse(byte Status, byte ParameterId);

public static class WriteParameterFrame
{
    private const byte CommandId = 0x0B;
    private const byte RequestStatus = 0x00;
    private const int StatusIndex = 2;
    private const int ParameterIdIndex = 7;

    public static byte[] Encode(WriteParameterRequest request)
    {
        var payloadLength = (ushort)(1 + request.Value.Length);
        var frameLength = (ushort)(8 + request.Value.Length);

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
        bytes.AddRange(request.Value);
        return bytes.ToArray();
    }

    public static WriteParameterResponse Decode(ReadOnlySpan<byte> frame)
    {
        return new WriteParameterResponse(frame[StatusIndex], frame[ParameterIdIndex]);
    }
}
