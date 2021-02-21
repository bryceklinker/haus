using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Health;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mqtt;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Health
{
    public class ZigbeeHostHealthPublisher : IHealthCheckPublisher
    {
        private readonly IZigbeeMqttClientFactory _zigbeeMqttClientFactory;
        private readonly IOptions<HausOptions> _hausOptions;

        private string HealthTopic => _hausOptions.Value.HealthTopic;

        public ZigbeeHostHealthPublisher(
            IZigbeeMqttClientFactory zigbeeMqttClientFactory,
            IOptions<HausOptions> hausOptions)
        {
            _zigbeeMqttClientFactory = zigbeeMqttClientFactory;
            _hausOptions = hausOptions;
        }

        public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken = default)
        {
            var mqttClient = await _zigbeeMqttClientFactory.CreateHausClient().ConfigureAwait(false);
            var hausReport = HausHealthReportModel.FromHealthReport(report);
            await mqttClient.PublishAsync(HealthTopic, hausReport).ConfigureAwait(false);
        }
    }
}