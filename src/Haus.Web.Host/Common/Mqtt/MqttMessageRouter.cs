using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Cqrs;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace Haus.Web.Host.Common.Mqtt;

public class MqttMessageRouter(
    IHausMqttClientFactory hausMqttClientFactory,
    IServiceScopeFactory scopeFactory,
    ILogger<MqttMessageRouter> logger
) : MqttBackgroundServiceListener(hausMqttClientFactory, scopeFactory)
{
    protected override async Task OnMessageReceived(MqttApplicationMessage message)
    {
        await RouteMqttMessage(message);
    }

    private async Task RouteMqttMessage(MqttApplicationMessage message)
    {
        using var scope = CreateScope();
        var eventFactory = scope.GetService<IRoutableEventFactory>();
        var @event = eventFactory.Create(message.PayloadSegment);
        if (@event == null)
        {
            logger.LogWarning(
                "No routable event mapping for MQTT message on topic {Topic}: {Payload}",
                message.Topic,
                Encoding.UTF8.GetString(message.PayloadSegment)
            );
            return;
        }

        var hausBus = scope.GetService<IHausBus>();
        await hausBus.PublishAsync(@event, CancellationToken.None);
    }
}
