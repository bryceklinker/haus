namespace Haus.Zigbee.Serial.Frames;

public enum NetworkState : byte
{
    Disconnected = 0,
    Connecting = 1,
    Connected = 2,
    Disconnecting = 3,
}

public static class ChangeNetworkStateFrameCodec
{
    private const byte CommandId = 0x08;
    private const byte FrameLength = 0x06;

    public static byte[] Encode(NetworkState networkState, byte sequenceNumber)
    {
        return [CommandId, sequenceNumber, 0x00, FrameLength, 0x00, (byte)networkState];
    }
}
