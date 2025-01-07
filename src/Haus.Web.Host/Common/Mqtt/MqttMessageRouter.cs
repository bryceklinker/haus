using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Cqrs;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;

namespace Haus.Web.Host.Common.Mqtt;

public class MqttMessageRouter(
    IHausMqttClientFactory hausMqttClientFactory,
    IServiceScopeFactory scopeFactory)
    : MqttBackgroundServiceListener(hausMqttClientFactory, scopeFactory)
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
            return;

        var hausBus = scope.GetService<IHausBus>();
        await hausBus.PublishAsync(@event, CancellationToken.None);
    }
}