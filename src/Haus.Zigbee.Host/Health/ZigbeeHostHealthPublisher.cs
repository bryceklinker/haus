using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Health;
using Haus.Mqtt.Client;
using Haus.Zigbee.Host.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Health;

public class ZigbeeHostHealthPublisher(IHausMqttClientFactory mqttClientFactory, IOptions<HausOptions> hausOptions)
    : IHealthCheckPublisher
{
    private string HealthTopic => hausOptions.Value.HealthTopic;

    public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken = default)
    {
        var mqttClient = await mqttClientFactory.CreateClient().ConfigureAwait(false);
        var hausReport = HausHealthReportModel.FromHealthReport(report);
        await mqttClient.PublishAsync(HealthTopic, hausReport).ConfigureAwait(false);
    }
}
