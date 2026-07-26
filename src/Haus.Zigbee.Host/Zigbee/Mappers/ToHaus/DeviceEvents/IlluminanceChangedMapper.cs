using System;
using Haus.Core.Models.Devices.Sensors.Light;
using Haus.Zigbee.Zcl;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;

public class IlluminanceChangedMapper
{
    private const ulong TooLowToBeMeasured = 0x0000;
    private const ulong Invalid = 0xffff;
    private const double LuxFormulaDivisor = 10000.0;

    public IlluminanceChangedModel Map(string deviceId, ZclAttributeValue value)
    {
        var measuredValue = value.AsUnsigned();
        return new IlluminanceChangedModel(deviceId, (long)measuredValue, ComputeLux(measuredValue));
    }

    private static long? ComputeLux(ulong measuredValue)
    {
        if (measuredValue is TooLowToBeMeasured or Invalid)
            return null;

        return (long)Math.Round(Math.Pow(10, (measuredValue - 1) / LuxFormulaDivisor));
    }
}
