using Haus.Core.Models;
using Haus.Core.Models.Devices.Sensors.Light;
using Haus.Zigbee.Host.Zigbee.Models;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;

public class IlluminanceChangedMapper
{
    public IlluminanceChangedModel? Map(Zigbee2MqttMessage message)
    {
        if (message.Illuminance.IsNull())
            return null;

        return new IlluminanceChangedModel(
            message.GetFriendlyNameFromTopic(),
            message.Illuminance.GetValueOrDefault(),
            message.IlluminanceLux.GetValueOrDefault()
        );
    }
}
