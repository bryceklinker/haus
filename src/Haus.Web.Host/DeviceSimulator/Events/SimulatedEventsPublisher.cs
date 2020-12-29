using System.Threading;
using System.Threading.Tasks;
using Haus.Core.DeviceSimulator.Events;
using Haus.Cqrs.Events;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Settings;
using Microsoft.Extensions.Options;

namespace Haus.Web.Host.DeviceSimulator.Events
{
    internal class SimulatedEventsPublisher : IEventHandler<SimulatedEvent>
    {
        private readonly IHausMqttClientFactory _mqttClientFactory;
        private readonly IOptions<HausMqttSettings> _options;

        public string EventsTopic => _options.Value.EventsTopic;
        
        public SimulatedEventsPublisher(IHausMqttClientFactory mqttClientFactory, IOptions<HausMqttSettings> options)
        {
            _mqttClientFactory = mqttClientFactory;
            _options = options;
        }

        public async Task Handle(SimulatedEvent notification, CancellationToken cancellationToken)
        {
            var client = await _mqttClientFactory.CreateClient().ConfigureAwait(false);
            await client.PublishAsync(EventsTopic, notification.HausEvent).ConfigureAwait(false);
        }
    }
}