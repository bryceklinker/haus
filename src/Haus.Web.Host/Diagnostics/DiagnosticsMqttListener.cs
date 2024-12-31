using System.Threading.Tasks;
using Haus.Core.Diagnostics.Factories;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;

namespace Haus.Web.Host.Diagnostics;

public class DiagnosticsMqttListener : MqttBackgroundServiceListener
{
    public DiagnosticsMqttListener(
        IHausMqttClientFactory hausMqttClientFactory,
        IServiceScopeFactory scopeFactory)
        : base(hausMqttClientFactory, scopeFactory)
    {
    }

    protected override async Task OnMessageReceived(MqttApplicationMessage message)
    {
        using var scope = CreateScope();
        var hub = scope.GetService<IHubContext<DiagnosticsHub>>();
        var messageFactory = scope.GetService<IMqttDiagnosticsMessageFactory>();
        var model = messageFactory.Create(message.Topic, message.PayloadSegment);
        await hub.BroadcastAsync("OnMqttMessage", model);
    }
}