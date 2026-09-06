using System.Threading.Tasks;
using Haus.Acceptance.Tests.Support;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Flows;

[TestFixture]
public class ZigbeeDiagnosticsFlow : HausPageTest
{
    private DeconzSimulatorClient _deconzSimulator;

    [SetUp]
    public void BeforeEach()
    {
        _deconzSimulator = GetDeconzSimulatorClient();
    }

    [Test]
    public async Task ShowsConnectedStatus()
    {
        await Page.PerformLoginAsync();

        var zigbee = await Page.NavigateToZigbeeAsync();

        await Expect(zigbee.GetConnectionStatus())
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 20_000 });
    }

    [Test]
    public async Task ShowsJoinedDeviceInDevicesList()
    {
        await Page.PerformLoginAsync();

        var ieeeAddress = await _deconzSimulator.JoinPhilipsMotionSensorAsync();
        var zigbee = await Page.NavigateToZigbeeAsync();

        await Expect(zigbee.GetDeviceListItem(ieeeAddress))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 15_000 });
    }

    [Test]
    public async Task ShowsJoinedDeviceInActivityFeedLive()
    {
        await Page.PerformLoginAsync();

        var zigbee = await Page.NavigateToZigbeeAsync();

        var ieeeAddress = await _deconzSimulator.JoinPhilipsMotionSensorAsync();

        // Entry content sits inside a collapsed MudExpansionPanel, so it's attached but not
        // visible until expanded -- attachment alone proves it. The page is never reloaded after
        // navigating, so this entry can only have arrived via the live realtime subscription, not
        // the initial REST-backed history load. Unlike the other two tests in this fixture, this
        // is the only assertion that depends on a live SignalR push racing a fixed deadline
        // (ShowsJoinedDeviceInDevicesList joins before navigating, so it only needs the initial
        // REST-backed load). That live round trip is exposed to CI runner/docker-compose resource
        // contention that has caused comparable device-address locator timeouts on main before
        // (e.g. AssignDevicesToRoomFlow.AssignDeviceToRoom, CI runs #32024628357 and #31976167118)
        // -- given a generous but still condition-based Playwright poll, not a fixed sleep.
        await Expect(zigbee.GetActivityEntryContaining(ieeeAddress))
            .ToBeAttachedAsync(new LocatorAssertionsToBeAttachedOptions { Timeout = 30_000 });
    }
}
