using System.Collections.Concurrent;

namespace Haus.Zigbee.Simulator;

// Firmware parameters a real deCONZ dongle reports (MAC address, PAN id, channel, ...): seeded
// with the defaults a coordinator ships with, then mutable via the ReadParameter/WriteParameter
// serial commands.
public class DeconzParameterStore
{
    private const byte MacAddressParameterId = 0x01;
    private const byte PanIdParameterId = 0x05;
    private const byte ChannelParameterId = 0x1c;

    private readonly ConcurrentDictionary<byte, byte[]> _parameters = new();

    public DeconzParameterStore()
    {
        Set(MacAddressParameterId, [0x22, 0x11, 0x00, 0xff, 0xff, 0x2e, 0x21, 0x00]);
        Set(PanIdParameterId, [0x62, 0x1a]);
        Set(ChannelParameterId, [0x0f]);
    }

    public void Set(byte parameterId, byte[] value)
    {
        _parameters[parameterId] = value;
    }

    public byte[] Get(byte parameterId)
    {
        return _parameters.TryGetValue(parameterId, out var value) ? value : [];
    }
}
