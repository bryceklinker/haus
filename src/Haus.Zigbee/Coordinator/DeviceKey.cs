using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Coordinator;

// Shared device identity for everything that needs to key per-device state off either address
// form a command destination or an inbound indication can carry -- DeviceCommandQueue's
// per-device lock, SleepyCommandHold's per-device pending slot, and the wake-signal listener that
// resolves an indication's SourceNwkAddress back to the device it belongs to.
public readonly record struct DeviceKey(bool IsIeee, ulong Address)
{
    public static DeviceKey FromIeee(IeeeAddress ieee) => new(true, ieee.Value);

    public static DeviceKey FromNwk(ushort networkAddress) => new(false, networkAddress);

    public static DeviceKey? FromDestination(ApsDestination destination)
    {
        return destination.Mode switch
        {
            DeconzAddressMode.Ieee => FromIeee(destination.IeeeAddress),
            DeconzAddressMode.Nwk => FromNwk(destination.ShortAddress),
            _ => null,
        };
    }
}
