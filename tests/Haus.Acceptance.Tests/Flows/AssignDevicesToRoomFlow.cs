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
        var rooms = await Page.NavigateToRoomsAsync();
        await rooms.AddRoomAsync(roomName);

        var devices = await Page.NavigateToDevicesAsync();
        var discovery = await devices.NavigateToDiscoveryAsync();
        await discovery.AssignDeviceToRoomAsync(lightId, roomName);
        await discovery.AssignDeviceToRoomAsync(sensorId, roomName);

        await Expect(discovery.GetRoomDropZone(roomName)).ToContainTextAsync(lightId);
        await Expect(discovery.GetRoomDropZone(roomName)).ToContainTextAsync(sensorId);
    }

    [TearDown]
    public async Task AfterEach()
    {
        await Context.StopTracingAsync();
    }
}
