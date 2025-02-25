using System;
using System.Threading.Tasks;
using Haus.Mqtt.Client.Settings;
using Haus.Mqtt.Client.Wrappers;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Mqtt.Client.Tests.Support;

public class SupportFactory
{
    public FakeMqttClientFactory FakeClientFactory { get; } = new();
    public FakeMqttClient FakeClient => FakeClientFactory.Client;

    public Task<IHausMqttClient> CreateClient()
    {
        var factory = CreateFactory();
        return factory.CreateClient();
    }

    public IHausMqttClientFactory CreateFactory()
    {
        var provider = GetProvider();
        return provider.GetRequiredService<IHausMqttClientFactory>();
    }

    private IServiceProvider GetProvider()
    {
        return new ServiceCollection()
            .AddHausMqtt()
            .AddLogging()
            .Replace<IMqttFactory>(FakeClientFactory)
            .Configure<HausMqttSettings>(opts =>
            {
                opts.Server = "mqtt://localhost:1883";
            })
            .BuildServiceProvider();
    }
}
