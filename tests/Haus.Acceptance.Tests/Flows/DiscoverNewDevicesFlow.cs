using System;
using System.Threading.Tasks;
using Haus.Acceptance.Tests.Support;
using Haus.Acceptance.Tests.Support.Zigbee2Mqtt;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Flows;

[TestFixture]
public class DiscoverNewDevicesFlow : HausPageTest
{
    private Zigbee2MqttPublisher _zigbee2MqttPublisher;

    [SetUp]
    public async Task BeforeEach()
    {
        await Context.StartTracingAsync();
        _zigbee2MqttPublisher = await GetZigbee2MqttPublisher();
    }

    [Test]
    public async Task DiscoverNewDevice()
    {
        var deviceId = $"{Guid.NewGuid()}";
        await Page.PerformLoginAsync();

        var devices = await Page.NavigateToDevicesAsync();
        var discovery = await devices.NavigateToDiscoveryAsync();

        await _zigbee2MqttPublisher.PublishPhilipsLight(deviceId);

        await Expect(discovery.GetUnassignedDevicesDropZone()).ToContainTextAsync(deviceId);
    }

    [TearDown]
    public async Task AfterEach()
    {
        await Context.StopTracingAsync();
    }
}
