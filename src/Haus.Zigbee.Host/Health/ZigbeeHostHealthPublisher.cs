using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Health;
using Haus.Mqtt.Client;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Host.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Health;

// AddCheck<T>-based health check discovery doesn't reach this Worker Service's HealthReport, so
// the Zigbee coordinator's health is checked here directly rather than trusted to already be in
// `report`.
public class ZigbeeHostHealthPublisher(
    IHausMqttClientFactory mqttClientFactory,
    IOptions<HausOptions> hausOptions,
    IZigbeeCoordinator coordinator
) : IHealthCheckPublisher
{
    private const string ZigbeeCheckName = "Zigbee";

    private string HealthTopic => hausOptions.Value.HealthTopic;

    public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken = default)
    {
        var mqttClient = await mqttClientFactory.CreateClient().ConfigureAwait(false);
        var zigbeeCheck = CreateCheckModel(coordinator.IsConnected);

        var hausReport = HausHealthReportModel.FromHealthReport(report).AppendChecks([zigbeeCheck]);
        await mqttClient.PublishAsync(HealthTopic, hausReport).ConfigureAwait(false);
    }

    private static HausHealthCheckModel CreateCheckModel(bool isConnected)
    {
        var status = isConnected ? HealthStatus.Healthy : HealthStatus.Unhealthy;
        return new HausHealthCheckModel(ZigbeeCheckName, status, 0);
    }
}
