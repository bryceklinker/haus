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
        // the initial REST-backed history load.
        await Expect(zigbee.GetActivityEntryContaining(ieeeAddress))
            .ToBeAttachedAsync(new LocatorAssertionsToBeAttachedOptions { Timeout = 15_000 });
    }
}
