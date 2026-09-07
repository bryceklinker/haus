using System.Threading.Tasks;
using Haus.Acceptance.Tests.Support;
using Haus.Testing.Support;
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

        // ZigbeeActivityView only creates and starts its live SignalR subscriber *after* its
        // initial REST-backed history load completes (both awaited in the same OnInitializedAsync),
        // so a join fired immediately after this navigation can race a subscription that isn't
        // listening yet. When that happens the event isn't delayed, it's missed for good -- no
        // amount of waiting recovers it. Confirmed with real evidence, not assumed: CI run
        // #34037014074 still failed this exact assertion after its fixed timeout was doubled from
        // 15s to 30s (https://github.com/bryceklinker/haus/actions/runs/34037014074), and the
        // failing snapshot showed 5 live "zigbee_device_joined" entries for 6 connected devices --
        // one join's event was genuinely never delivered to this page's feed, not just slow.
        //
        // Retrying with a *fresh* device join closes that race deterministically instead of just
        // waiting longer for the same one: once any join has been observed live, the subscription
        // is definitely up, so every subsequent join can only ever be slow, never lost.
        await Eventually.AssertAsync(
            async () =>
            {
                var ieeeAddress = await _deconzSimulator.JoinPhilipsMotionSensorAsync();

                // Entry content sits inside a collapsed MudExpansionPanel, so it's attached but
                // not visible until expanded -- attachment alone proves it.
                await Expect(zigbee.GetActivityEntryContaining(ieeeAddress))
                    .ToBeAttachedAsync(new LocatorAssertionsToBeAttachedOptions { Timeout = 5_000 });
            },
            timeout: 30_000,
            delay: 0
        );
    }
}
