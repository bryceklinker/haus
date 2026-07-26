using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Mqtt.Client;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Host.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee.Services;

public class ZigbeeToHausRelay(
    IZigbeeCoordinator coordinator,
    ZigbeeInboundRelay inboundRelay,
    ZigbeeOutboundRelay outboundRelay,
    IHausMqttClientFactory hausMqttClientFactory,
    IOptions<HausOptions> hausOptions,
    ILogger<ZigbeeToHausRelay> logger
) : BackgroundService
{
    private IHausMqttClient? _hausMqttClient;

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
        await coordinator.ConnectAsync(cancellationToken);
        coordinator.DeviceJoined += OnDeviceJoined;
        coordinator.AttributeReported += OnAttributeReported;

        _hausMqttClient = await hausMqttClientFactory.CreateClient();
        await HausMqttClient.SubscribeAsync(hausOptions.Value.CommandsTopic, HandleHausCommandAsync);
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        coordinator.DeviceJoined -= OnDeviceJoined;
        coordinator.AttributeReported -= OnAttributeReported;
        await HausMqttClient.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }

    private Task HandleHausCommandAsync(MqttApplicationMessage message)
    {
        return outboundRelay.HandleCommandAsync(message, CancellationToken.None);
    }

    private async void OnDeviceJoined(object? sender, Haus.Zigbee.ZigbeeDeviceJoined joined)
    {
        try
        {
            await inboundRelay.HandleDeviceJoinedAsync(joined);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to relay device-joined for {@Address}", joined.IeeeAddress);
        }
    }

    private async void OnAttributeReported(object? sender, Haus.Zigbee.ZigbeeAttributeReport report)
    {
        try
        {
            await inboundRelay.HandleAttributeReportedAsync(report);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to relay attribute report from {@Address}", report.SourceNwkAddress);
        }
    }
}
