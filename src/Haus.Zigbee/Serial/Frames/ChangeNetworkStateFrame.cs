using System;

namespace Haus.Zigbee.Serial.Frames;

public record ChangeNetworkStateResponse(byte Status, NetworkState NetworkState);

public static class ChangeNetworkStateFrameCodec
{
    private const byte CommandId = 0x08;
    private const byte FrameLength = 0x06;
    private const int StatusIndex = 2;
    private const int NetworkStateIndex = 5;

    public static byte[] Encode(NetworkState networkState, byte sequenceNumber)
    {
        return [CommandId, sequenceNumber, 0x00, FrameLength, 0x00, (byte)networkState];
    }

    public static ChangeNetworkStateResponse Decode(ReadOnlySpan<byte> frame)
    {
        return new ChangeNetworkStateResponse(frame[StatusIndex], (NetworkState)frame[NetworkStateIndex]);
    }
}
