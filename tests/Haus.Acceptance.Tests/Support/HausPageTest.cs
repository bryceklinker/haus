using System;
using System.Threading.Tasks;
using Haus.Acceptance.Tests.Support.Zigbee2Mqtt;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Haus.Acceptance.Tests.Support;

public class HausPageTest : PageTest
{
    private readonly IServiceProvider _provider;

    public HausPageTest()
    {
        _provider = new ServiceCollection()
            .AddHausMqtt()
            .AddLogging()
            .AddOptions()
            .Configure<HausMqttSettings>(opts =>
            {
                opts.Server = "mqtt://localhost:1883";
            })
            .BuildServiceProvider();
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = "http://localhost:5002",
            ScreenSize = new ScreenSize { Width = 1920, Height = 1080 },
        };
    }

    public async Task<Zigbee2MqttPublisher> GetZigbee2MqttPublisher()
    {
        var factory = _provider.GetRequiredService<IHausMqttClientFactory>();
        var client = await factory.CreateClient();
        return new Zigbee2MqttPublisher(client);
    }
}
