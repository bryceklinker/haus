namespace Haus.Zigbee.Zdp;

// Shared by the Device_annce capability byte (DeviceAnnounceParser) and the Node Descriptor's MAC
// Capability Flags byte (NodeDescriptorResponseCodec) -- both carry this bit at the same position.
public static class MacCapabilityFlags
{
    private const byte ReceiverOnWhenIdleBit = 0x08;

    public static bool IsReceiverOnWhenIdle(byte capabilities) => (capabilities & ReceiverOnWhenIdleBit) != 0;
}
