using System;
using System.Threading.Tasks;
using Haus.Core;
using Haus.Core.Models;
using Haus.Core.Models.ExternalMessages;
using Haus.Mqtt.Client;
using Haus.Testing.Support;
using Haus.Web.Host.Common.Mqtt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using Xunit;

namespace Haus.Web.Host.Tests.Common.Mqtt;

public class MqttMessageRouterTests
{
    private readonly CapturingLoggerFactory _loggerFactory = new();
    private readonly TestableMqttMessageRouter _router;

    public MqttMessageRouterTests()
    {
        var provider = new ServiceCollection()
            .AddHausCore(opts => opts.UseInMemoryDatabase($"{Guid.NewGuid()}"))
            .AddLogging()
            .AddSingleton<ILoggerFactory>(_loggerFactory)
            .BuildServiceProvider();

        _router = new TestableMqttMessageRouter(
            new UncalledHausMqttClientFactory(),
            provider.GetRequiredService<IServiceScopeFactory>(),
            provider.GetRequiredService<ILogger<MqttMessageRouter>>()
        );
    }

    [Fact]
    public async Task WhenMessageCannotBeRoutedThenWarningIsLoggedWithTopicAndPayload()
    {
        var message = new MqttApplicationMessage
        {
            Topic = "haus/events",
            PayloadSegment = HausJsonSerializer.SerializeToBytes(new HausEvent("some_unmapped_type")),
        };

        await _router.InvokeOnMessageReceived(message);

        Assert.Contains(
            _loggerFactory.Entries,
            entry =>
                entry.Level == LogLevel.Warning
                && entry.Message.Contains("haus/events")
                && entry.Message.Contains("some_unmapped_type")
        );
    }

    private class TestableMqttMessageRouter(
        IHausMqttClientFactory hausMqttClientFactory,
        IServiceScopeFactory scopeFactory,
        ILogger<MqttMessageRouter> logger
    ) : MqttMessageRouter(hausMqttClientFactory, scopeFactory, logger)
    {
        public Task InvokeOnMessageReceived(MqttApplicationMessage message)
        {
            return OnMessageReceived(message);
        }
    }

    private class UncalledHausMqttClientFactory : IHausMqttClientFactory
    {
        public Task<IHausMqttClient> CreateClient() => throw new NotSupportedException();

        public Task<IHausMqttClient> CreateClient(string url) => throw new NotSupportedException();

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
