using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Node;
using Microsoft.Extensions.Hosting;

namespace Haus.Zigbee.Host.Services
{
    public class ZigBeeWorker : BackgroundService
    {
        private readonly INodeZigbeeProcess _zigbeeProcess;
        private readonly IZigbee2MqttConfigurationWriter _zigbee2MqttConfigurationWriter;

        public ZigBeeWorker(INodeZigbeeProcess zigbeeProcess, IZigbee2MqttConfigurationWriter zigbee2MqttConfigurationWriter)
        {
            _zigbeeProcess = zigbeeProcess;
            _zigbee2MqttConfigurationWriter = zigbee2MqttConfigurationWriter;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _zigbee2MqttConfigurationWriter.WriteConfigAsync();
            _zigbeeProcess.Start();
            
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _zigbeeProcess.Stop();
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _zigbeeProcess.Dispose();
            base.Dispose();
        }
    }
}
