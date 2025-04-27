using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Mqtt.Client;
using MQTTnet;

namespace Haus.Acceptance.Tests.Support.Zigbee2Mqtt;

public class Zigbee2MqttPublisher(IHausMqttClient client)
{
    public async Task PublishAsJsonAsync(Zigbee2MqttMessage message)
    {
        await client.PublishAsync(
            new MqttApplicationMessage
            {
                Topic = message.Topic,
                PayloadSegment = HausJsonSerializer.SerializeToBytes(message),
            }
        );
    }
}
