using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet;

namespace Haus.Mqtt.Client.Services;

public abstract class MqttBackgroundServiceListener(
    IHausMqttClientFactory hausMqttClientFactory,
    IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var mqttClient = await hausMqttClientFactory.CreateClient();
        await mqttClient.SubscribeAsync("#", OnMessageReceived);
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested) await Task.Delay(1000, stoppingToken);
    }

    protected IServiceScope CreateScope()
    {
        return scopeFactory.CreateScope();
    }

    protected abstract Task OnMessageReceived(MqttApplicationMessage message);
}