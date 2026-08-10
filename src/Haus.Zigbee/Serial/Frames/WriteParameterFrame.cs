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

        List<byte> bytes = [CommandId, request.SequenceNumber, RequestStatus];
        LittleEndian.Write(bytes, frameLength);
        LittleEndian.Write(bytes, payloadLength);
        bytes.Add(request.ParameterId);
        bytes.AddRange(request.Value);
        return bytes.ToArray();
    }

    // This response comes straight off the wire, so a truncated frame must produce a null result
    // here rather than throw -- see ZclFrameHeaderCodec.Decode for why an exception here would
    // silently stop delivery to every other IndicationReceived subscriber.
    public static WriteParameterResponse? Decode(ReadOnlySpan<byte> frame)
    {
        if (frame.Length <= ParameterIdIndex)
            return null;

        return new WriteParameterResponse(frame[StatusIndex], frame[ParameterIdIndex]);
    }
}
