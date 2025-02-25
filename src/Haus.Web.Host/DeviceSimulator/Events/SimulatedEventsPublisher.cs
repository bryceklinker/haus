using System.Threading;
using System.Threading.Tasks;
using Haus.Core.DeviceSimulator.Events;
using Haus.Cqrs.Events;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Settings;
using Microsoft.Extensions.Options;

namespace Haus.Web.Host.DeviceSimulator.Events;

internal class SimulatedEventsPublisher(IHausMqttClientFactory mqttClientFactory, IOptions<HausMqttSettings> options)
    : IEventHandler<SimulatedEvent>
{
    public string EventsTopic => options.Value.EventsTopic;

    public async Task Handle(SimulatedEvent notification, CancellationToken cancellationToken)
    {
        var client = await mqttClientFactory.CreateClient().ConfigureAwait(false);
        await client.PublishAsync(EventsTopic, notification.HausEvent).ConfigureAwait(false);
    }
}
