using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Haus.Mqtt.Client.Health
{
    public class HausMqttHealthCHeck : IHealthCheck
    {
        private readonly IHausMqttClientFactory _clientFactory;

        public HausMqttHealthCHeck(IHausMqttClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = await _clientFactory.CreateClient();
                await client.PingAsync(cancellationToken);
                
                if (client.IsStarted && client.IsConnected)
                    return HealthCheckResult.Healthy("Mqtt client is health and working as expected");

                return HealthCheckResult.Unhealthy("Mqtt client is not connected or not started");
            }
            catch (Exception e)
            {
                return HealthCheckResult.Unhealthy(
                    "An exception occurred when attempting to send ping or creating client", e);
            }
            
        }
    }
}