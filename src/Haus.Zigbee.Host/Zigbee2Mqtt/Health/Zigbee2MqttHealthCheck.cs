using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Host.Zigbee2Mqtt.Node;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Health
{
    public class Zigbee2MqttHealthCheck : IHealthCheck
    {
        private readonly INodeZigbeeProcess _nodeZigbeeProcess;

        public Zigbee2MqttHealthCheck(INodeZigbeeProcess nodeZigbeeProcess)
        {
            _nodeZigbeeProcess = nodeZigbeeProcess;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = _nodeZigbeeProcess.IsRunning
                ? HealthCheckResult.Healthy("Zigbee2Mqtt process is running")
                : HealthCheckResult.Unhealthy("Zigbee2Mqtt process is not running");
            
            return Task.FromResult(result);
        }
    }
}