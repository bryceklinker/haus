using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Web.Host.Common.Mqtt
{
    public class RoutableCommandHandler : IEventHandler<RoutableCommand>
    {
        private readonly IOptions<MqttOptions> _mqttOptions;
        private readonly IHausMqttClientFactory _clientFactory;

        private string CommandsTopic => _mqttOptions.Value.CommandsTopic;
        
        public RoutableCommandHandler(IOptions<MqttOptions> mqttOptions, IHausMqttClientFactory clientFactory)
        {
            _mqttOptions = mqttOptions;
            _clientFactory = clientFactory;
        }

        public async Task Handle(RoutableCommand notification, CancellationToken cancellationToken)
        {
            var hausMqttClient = await _clientFactory.CreateClient();
            await hausMqttClient.PublishAsync(new MqttApplicationMessage
            {
                Topic = CommandsTopic,
                Payload = HausJsonSerializer.SerializeToBytes(notification.HausCommand)
            });
        }
    }
}