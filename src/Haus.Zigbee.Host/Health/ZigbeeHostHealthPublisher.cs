using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Health;
using Haus.Mqtt.Client;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Health;

// AddCheck<T>-based health check discovery doesn't reach this Worker Service's HealthReport, so
// every check we care about is invoked here directly rather than trusted to already be in `report`.
public class ZigbeeHostHealthPublisher(
    IHausMqttClientFactory mqttClientFactory,
    IOptions<HausOptions> hausOptions,
    ZigbeeCoordinatorHealthCheck zigbeeCoordinatorHealthCheck
) : IHealthCheckPublisher
{
    private const string ZigbeeCheckName = "Zigbee";

    private string HealthTopic => hausOptions.Value.HealthTopic;

    public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken = default)
    {
        var mqttClient = await mqttClientFactory.CreateClient().ConfigureAwait(false);
        var zigbeeResult = await zigbeeCoordinatorHealthCheck
            .CheckHealthAsync(new HealthCheckContext(), cancellationToken)
            .ConfigureAwait(false);

        var hausReport = HausHealthReportModel.FromHealthReport(report).AppendChecks([CreateCheckModel(zigbeeResult)]);
        await mqttClient.PublishAsync(HealthTopic, hausReport).ConfigureAwait(false);
    }

    private static HausHealthCheckModel CreateCheckModel(HealthCheckResult result)
    {
        return new HausHealthCheckModel(
            ZigbeeCheckName,
            result.Status,
            0,
            result.Description,
            result.Exception?.Message
        );
    }
}
