using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Devices.Events;
using Haus.Cqrs.Events;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Settings;
using Microsoft.Extensions.Options;

namespace Haus.Web.Host.Zigbee;

public class ZigbeeKnownDeviceSeedPublisher(
    IHausMqttClientFactory mqttClientFactory,
    IOptions<HausMqttSettings> options
) : IEventHandler<ZigbeeKnownDeviceSeedRequestedEvent>
{
    public async Task Handle(ZigbeeKnownDeviceSeedRequestedEvent notification, CancellationToken cancellationToken)
    {
        var client = await mqttClientFactory.CreateClient().ConfigureAwait(false);
        await client
            .PublishHausEventAsync(notification.Seed, options.Value.ZigbeeKnownDeviceSeedTopic, retain: true)
            .ConfigureAwait(false);
    }
}
