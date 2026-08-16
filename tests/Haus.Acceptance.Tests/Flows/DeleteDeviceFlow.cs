using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Haus.Acceptance.Tests.Support;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Flows;

[TestFixture]
public class DeleteDeviceFlow : HausPageTest
{
    private DeconzSimulatorClient _deconzSimulator;

    [SetUp]
    public void BeforeEach()
    {
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

        // No manual reload here: the site pushes the deletion over SignalR, so the detail page
        // should navigate itself back to /devices and the list should drop the device live.
        await Expect(Page)
            .ToHaveURLAsync(new Regex("/devices$"), new PageAssertionsToHaveURLOptions { Timeout = 10_000 });
        await Expect(devices.GetDeviceListItem(externalId))
            .Not.ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 10_000 });

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
}
