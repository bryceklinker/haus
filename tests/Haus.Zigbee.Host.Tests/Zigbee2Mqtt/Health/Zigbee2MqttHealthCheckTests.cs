using System.Threading.Tasks;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Health
{
    public class Zigbee2MqttHealthCheckTests
    {
        private readonly FakeNodeZigbeeProcess _nodeZigbeeProcess;
        private readonly Zigbee2MqttHealthCheck _check;

        public Zigbee2MqttHealthCheckTests()
        {
            _nodeZigbeeProcess = new FakeNodeZigbeeProcess();
            _check = new Zigbee2MqttHealthCheck(_nodeZigbeeProcess);
        }
        
        [Fact]
        public async Task WhenZigbeeNodeProcessIsNotRunningThenReturnsUnhealthy()
        {
            _nodeZigbeeProcess.IsRunning = false;

            var result = await _check.CheckHealthAsync(new HealthCheckContext());
            
            Assert.Equal(HealthStatus.Unhealthy, result.Status);
        }

        [Fact]
        public async Task WhenZigbeeNodeProcessIsRunningThenReturnsHealthy()
        {
            _nodeZigbeeProcess.IsRunning = true;

            var result = await _check.CheckHealthAsync(new HealthCheckContext());

            Assert.Equal(HealthStatus.Healthy, result.Status);
        }
    }
}