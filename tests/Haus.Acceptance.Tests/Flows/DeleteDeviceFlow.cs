using System;
using System.Threading.Tasks;
using Haus.Acceptance.Tests.Support;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Flows;

[TestFixture]
public class DeleteDeviceFlow : HausPageTest
{
    private DeconzSimulatorClient _deconzSimulator;

    [SetUp]
    public async Task BeforeEach()
    {
        await Context.StartTracingAsync();
        _deconzSimulator = GetDeconzSimulatorClient();
    }

    [Test]
    public async Task DeleteDevice()
    {
        await Page.PerformLoginAsync();

        var externalId = await _deconzSimulator.JoinPhilipsLightAsync();
        var devices = await Page.NavigateToDevicesAsync();
        var detail = await devices.NavigateToDeviceAsync(externalId);

        await detail.DeleteAsync();

        await devices.ReloadAsync();
        await Expect(devices.GetDeviceListItem(externalId)).Not.ToBeVisibleAsync();

        await detail.ReloadAsync();
        await Expect(Page.CssLocator(".device-detail")).Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task DeletingDeviceRemovesItFromItsRoom()
    {
        var roomName = $"{Guid.NewGuid()}";
        await Page.PerformLoginAsync();

        var rooms = await Page.NavigateToRoomsAsync();
        await rooms.AddRoomAsync(roomName);

        var externalId = await _deconzSimulator.JoinPhilipsLightAsync();
        var devices = await Page.NavigateToDevicesAsync();
        var discovery = await devices.NavigateToDiscoveryAsync();
        await discovery.AssignDeviceToRoomAsync(externalId, roomName);

        await Expect(discovery.GetRoomDropZone(roomName))
            .ToContainTextAsync(externalId, new LocatorAssertionsToContainTextOptions { Timeout = 10_000 });

        var devicesAgain = await Page.NavigateToDevicesAsync();
        var detail = await devicesAgain.NavigateToDeviceAsync(externalId);
        await detail.DeleteAsync();

        var devicesAfterDelete = await Page.NavigateToDevicesAsync();
        var discoveryAfterDelete = await devicesAfterDelete.NavigateToDiscoveryAsync();

        await Expect(discoveryAfterDelete.GetRoomDropZone(roomName))
            .Not.ToContainTextAsync(externalId, new LocatorAssertionsToContainTextOptions { Timeout = 10_000 });
    }

    [TearDown]
    public async Task AfterEach()
    {
        await Context.StopTracingAsync();
    }
}
