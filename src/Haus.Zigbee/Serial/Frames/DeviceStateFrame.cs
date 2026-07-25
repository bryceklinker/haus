namespace Haus.Zigbee.Serial.Frames;

public static class DeviceStateCodec
{
    private const byte StatusCommandId = 0x07;

    public static byte[] EncodePollRequest(byte sequenceNumber)
    {
        return [StatusCommandId, sequenceNumber, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00];
    }
}
