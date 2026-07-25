using System.Collections.Generic;

namespace Haus.Zigbee.Serial.Frames;

public sealed record ReadParameterRequest(byte SequenceNumber, byte ParameterId, byte[] Arguments);

public static class ReadParameterFrame
{
    private const byte CommandId = 0x0A;
    private const byte RequestStatus = 0x00;

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
}
