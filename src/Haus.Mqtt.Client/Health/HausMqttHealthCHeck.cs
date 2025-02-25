using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Haus.Mqtt.Client.Health;

public class HausMqttHealthCHeck(IHausMqttClientFactory clientFactory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var client = await clientFactory.CreateClient();
            await client.PingAsync(cancellationToken);

            return client.IsConnected
                ? HealthCheckResult.Healthy("Mqtt client is health and working as expected")
                : HealthCheckResult.Unhealthy("Mqtt client is not connected");
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy(
                "An exception occurred when attempting to send ping or creating client",
                e
            );
        }
    }
}
