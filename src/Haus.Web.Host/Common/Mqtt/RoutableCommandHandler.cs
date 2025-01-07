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

internal class RoutableCommandHandler(IOptions<HausMqttSettings> mqttOptions, IHausMqttClientFactory clientFactory)
    : IEventHandler<RoutableCommand>
{
    private string CommandsTopic => mqttOptions.Value.CommandsTopic;

    public async Task Handle(RoutableCommand notification, CancellationToken cancellationToken)
    {
        var hausMqttClient = await clientFactory.CreateClient();
        await hausMqttClient.PublishAsync(new MqttApplicationMessage
        {
            Topic = CommandsTopic,
            PayloadSegment = HausJsonSerializer.SerializeToBytes(notification.HausCommand)
        });
    }
}