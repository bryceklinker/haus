using System;
using System.Threading.Tasks;
using Haus.Acceptance.Tests.Support;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Flows;

[TestFixture]
public class AssignDevicesToRoomFlow : HausPageTest
{
    private DeconzSimulatorClient _deconzSimulator;

    [SetUp]
    public void BeforeEach()
    {
        _deconzSimulator = GetDeconzSimulatorClient();
    }

    [Test]
    public async Task AssignDeviceToRoom()
    {
        var roomName = $"{Guid.NewGuid()}";
        await Page.PerformLoginAsync(HausUser.Default);

        var lightId = await _deconzSimulator.JoinPhilipsLightAsync();
        var sensorId = await _deconzSimulator.JoinPhilipsMotionSensorAsync();
        var rooms = await Page.NavigateToRoomsAsync();
        await rooms.AddRoomAsync(roomName);

        var devices = await Page.NavigateToDevicesAsync();
        var discovery = await devices.NavigateToDiscoveryAsync();
        await discovery.AssignDeviceToRoomAsync(lightId, roomName);
        await discovery.AssignDeviceToRoomAsync(sensorId, roomName);

        await Expect(discovery.GetRoomDropZone(roomName))
            .ToContainTextAsync(lightId, new LocatorAssertionsToContainTextOptions { Timeout = 10_000 });
        await Expect(discovery.GetRoomDropZone(roomName))
            .ToContainTextAsync(sensorId, new LocatorAssertionsToContainTextOptions { Timeout = 10_000 });
    }
}
