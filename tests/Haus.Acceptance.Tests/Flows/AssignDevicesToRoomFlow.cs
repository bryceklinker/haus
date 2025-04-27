using System;
using System.Threading.Tasks;
using Haus.Acceptance.Tests.Support;
using Haus.Acceptance.Tests.Support.Zigbee2Mqtt;

namespace Haus.Acceptance.Tests.Flows;

[TestFixture]
public class AssignDevicesToRoomFlow : HausPageTest
{
    private Zigbee2MqttPublisher _zigbee2MqttPublisher;

    [SetUp]
    public async Task BeforeEach()
    {
        await Context.StartTracingAsync();
        _zigbee2MqttPublisher = await GetZigbee2MqttPublisher();
    }

    [Test]
    public async Task AssignDeviceToRoom()
    {
        var lightId = $"{Guid.NewGuid()}";
        var sensorId = $"{Guid.NewGuid()}";
        var roomName = $"{Guid.NewGuid()}";
        await Page.PerformLoginAsync(HausUser.Default);

        await _zigbee2MqttPublisher.PublishPhilipsLight(lightId);
        await _zigbee2MqttPublisher.PublishPhilipsMotionSensor(sensorId);
        await AddRoom(roomName);

        await Page.ClickLinkAsync("Devices");
        await Page.ClickLinkAsync("Discovery");

        await Page.GetByText(lightId).DragToAsync(Page.GetByText(roomName));
        await Page.GetByText(sensorId).DragToAsync(Page.GetByText(roomName));

        await Expect(Page.CssLocatorWithText(".mud-drop-zone", roomName)).ToContainTextAsync(lightId);
        await Expect(Page.CssLocatorWithText(".mud-drop-zone", roomName)).ToContainTextAsync(sensorId);
    }

    [TearDown]
    public async Task AfterEach()
    {
        await Context.StopTracingAsync();
    }

    private async Task AddRoom(string roomName)
    {
        await Page.ClickLinkAsync("Rooms");
        await Page.CssLocator(".mud-fab").ClickAsync();
        await Page.EnterTextAsync("name", roomName);
        await Page.ClickButtonAsync("Save");
    }
}
