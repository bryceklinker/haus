using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Health
{
    public class Zigbee2MqttHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}