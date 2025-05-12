using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading.Tasks;
using Haus.Mqtt.Client.Settings;
using Haus.Mqtt.Client.Wrappers;
using Microsoft.Extensions.Options;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Mqtt.Client;

public interface IHausMqttClientFactory : IAsyncDisposable
{
    Task<IHausMqttClient> CreateClient();
    Task<IHausMqttClient> CreateClient(string url);
}

internal class HausMqttClientFactory(
    IOptions<HausMqttSettings> options,
    IMqttFactory mqttFactory,
    IMqttNetLogger logger
) : IHausMqttClientFactory
{
    private readonly ConcurrentDictionary<string, Task<IHausMqttClient>> _clients = new();

    private string MqttServer => options.Value.Server;

    public Task<IHausMqttClient> CreateClient()
    {
        return CreateClient(MqttServer);
    }

    public async Task<IHausMqttClient> CreateClient(string url)
    {
        return await _clients.GetOrAdd(url, CreateMqttClientWithRetry).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var (_, task) in _clients)
        {
            var client = await task.ConfigureAwait(false);
            await client.DisposeAsync().ConfigureAwait(false);
        }
    }

    private async Task<IHausMqttClient> CreateMqttClientWithRetry(string url)
    {
        var client = await CreateMqttClient(url);
        var retryCount = 1;
        while (!client.IsConnected && retryCount < 5)
        {
            await Task.Delay(10 * retryCount);
            retryCount++;
        }

        return client;
    }

    private async Task<IHausMqttClient> CreateMqttClient(string url)
    {
        var uri = new Uri(url);
        var client = mqttFactory.CreateManagedMqttClient(logger);
        var options1 = new ManagedMqttClientOptionsBuilder()
            .WithClientOptions(opts =>
            {
                opts.WithTcpServer(uri.Host, uri.Port, AddressFamily.InterNetwork);
            })
            .Build();
        await client.StartAsync(options1).ConfigureAwait(false);
        return new HausMqttClient(client, options, () => RemovedDisposedClient(url));
    }

    private void RemovedDisposedClient(string url)
    {
        _clients.TryRemove(url, out _);
    }
}
