using System.Threading.Tasks;
using Haus.Core.Health.Commands;
using Haus.Core.Models;
using Haus.Core.Models.Health;
using Haus.Cqrs;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Services;
using Haus.Mqtt.Client.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Web.Host.Health;

public class HealthListener(
    IHausMqttClientFactory hausMqttClientFactory,
    IServiceScopeFactory scopeFactory,
    IOptions<HausMqttSettings> hausMqttSettings
) : MqttBackgroundServiceListener(hausMqttClientFactory, scopeFactory)
{
    private string HealthTopic => hausMqttSettings.Value.HealthTopic;

    protected override async Task OnMessageReceived(MqttApplicationMessage message)
    {
        if (message.Topic != HealthTopic)
            return;

        var healthReport = HausJsonSerializer.Deserialize<HausHealthReportModel>(message.PayloadSegment);
        using var scope = CreateScope();
        var bus = scope.GetService<IHausBus>();
        await bus.ExecuteCommandAsync(new StoreHealthReportCommand(healthReport));
    }
}
