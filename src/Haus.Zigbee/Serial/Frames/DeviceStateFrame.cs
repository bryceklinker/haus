using System;

namespace Haus.Zigbee.Serial.Frames;

public enum NetworkState
{
    Disconnected = 0,
    Connecting = 1,
    Connected = 2,
    Disconnecting = 3,
}

public record DeviceStateFrame(
    byte CommandId,
    NetworkState NetworkState,
    bool ApsDataConfirmAvailable,
    bool ApsDataIndicationAvailable,
    bool ConfigurationChanged,
    bool ApsFreeSlotsAvailable
);

public static class DeviceStateCodec
{
    private const byte StatusCommandId = 0x07;

    private const int DeviceStateOffset = 5;

    private const byte NetworkStateMask = 0x03;
    private const byte ApsDataConfirmMask = 0x04;
    private const byte ApsDataIndicationMask = 0x08;
    private const byte ConfigurationChangedMask = 0x10;
    private const byte ApsFreeSlotsMask = 0x20;

    public static byte[] EncodePollRequest(byte sequenceNumber)
    {
        return [StatusCommandId, sequenceNumber, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00];
    }

    // This response comes straight off the wire, so a truncated frame must produce a null result
    // here rather than throw -- see ZclFrameHeaderCodec.Decode for why an exception here would
    // silently stop delivery to every other IndicationReceived subscriber.
    public static DeviceStateFrame? Decode(ReadOnlySpan<byte> frame)
    {
        if (frame.Length <= DeviceStateOffset)
            return null;

        var commandId = frame[0];
        var deviceState = frame[DeviceStateOffset];

        return new DeviceStateFrame(
            commandId,
            (NetworkState)(deviceState & NetworkStateMask),
            (deviceState & ApsDataConfirmMask) != 0,
            (deviceState & ApsDataIndicationMask) != 0,
            (deviceState & ConfigurationChangedMask) != 0,
            (deviceState & ApsFreeSlotsMask) != 0
        );
    }
}
