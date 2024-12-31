using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Models;
using Haus.Cqrs.Events;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Settings;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Web.Host.Common.Mqtt;

internal class RoutableCommandHandler : IEventHandler<RoutableCommand>
{
    private readonly IOptions<HausMqttSettings> _mqttOptions;
    private readonly IHausMqttClientFactory _clientFactory;

    private string CommandsTopic => _mqttOptions.Value.CommandsTopic;

    public RoutableCommandHandler(IOptions<HausMqttSettings> mqttOptions, IHausMqttClientFactory clientFactory)
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
            PayloadSegment = HausJsonSerializer.SerializeToBytes(notification.HausCommand)
        });
    }
}