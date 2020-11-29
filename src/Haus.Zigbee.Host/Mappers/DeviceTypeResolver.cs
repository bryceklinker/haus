using Haus.Core.Models.Devices;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;

namespace Haus.Zigbee.Host.Mappers
{
    public interface IDeviceTypeResolver
    {
        DeviceType Resolve(Zigbee2MqttMessage zigbee2MqttMessage);
    }
    
    public class DeviceTypeResolver : IDeviceTypeResolver
    {
        public DeviceType Resolve(Zigbee2MqttMessage zigbee2MqttMessage)
        {
            return DeviceType.Unknown;
        }
    }
}