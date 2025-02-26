using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Mqtt.Client;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mqtt;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Services;

public class ZigbeeToHausRelay(
    IMqttMessageMapper mqttMessageMapper,
    IZigbeeMqttClientFactory zigbeeMqttFactory,
    ILogger<ZigbeeToHausRelay> logger
) : BackgroundService
{
    private IHausMqttClient? _zigbeeMqttClient;
    private IHausMqttClient? _hausMqttClient;

    private IHausMqttClient ZigbeeMqttClient
    {
        get
        {
            ArgumentNullException.ThrowIfNull(_zigbeeMqttClient);
            return _zigbeeMqttClient;
        }
    }

    private IHausMqttClient HausMqttClient
    {
        get
        {
            ArgumentNullException.ThrowIfNull(_hausMqttClient);
            return _hausMqttClient;
        }
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _zigbeeMqttClient = await zigbeeMqttFactory.CreateZigbeeClient();
        await ZigbeeMqttClient.SubscribeAsync("#", ZigbeeMessageHandler);

        _hausMqttClient = await zigbeeMqttFactory.CreateHausClient();
        await HausMqttClient.SubscribeAsync("#", HausMessageHandler);
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await HausMqttClient.DisposeAsync();
        await ZigbeeMqttClient.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }

    private async Task HausMessageHandler(MqttApplicationMessage arg)
    {
        await HandleMqttMessage(arg, ZigbeeMqttClient);
    }

    private async Task ZigbeeMessageHandler(MqttApplicationMessage arg)
    {
        await HandleMqttMessage(arg, HausMqttClient);
    }

    private async Task HandleMqttMessage(MqttApplicationMessage mqttMessage, IHausMqttClient targetMqtt)
    {
        var messageToSend = mqttMessageMapper.Map(mqttMessage);

        foreach (var message in messageToSend)
        {
            logger.LogInformation("Sending message to {@Topic}...", message.Topic);
            await targetMqtt.PublishAsync(message).ConfigureAwait(false);
            logger.LogInformation("Sent message to {@Topic}", message.Topic);
        }
    }
}
