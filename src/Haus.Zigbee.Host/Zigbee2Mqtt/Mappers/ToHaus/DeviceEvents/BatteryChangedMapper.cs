using Haus.Core.Models;
using Haus.Core.Models.Devices.Sensors.Battery;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents;

public class BatteryChangedMapper
{
    public BatteryChangedModel? Map(Zigbee2MqttMessage message)
    {
        if (message.Battery.IsNull())
            return null;

        return new BatteryChangedModel(message.GetFriendlyNameFromTopic(), message.Battery.GetValueOrDefault());
    }
}
