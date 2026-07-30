using System.Threading.Tasks;
using Haus.Acceptance.Tests.Support;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Flows;

[TestFixture]
public class DiscoverNewDevicesFlow : HausPageTest
{
    private DeconzSimulatorClient _deconzSimulator;

    [SetUp]
    public async Task BeforeEach()
    {
        await Context.StartTracingAsync();
        _deconzSimulator = GetDeconzSimulatorClient();
    }

    [Test]
    public async Task DiscoverNewDevice()
    {
        await Page.PerformLoginAsync();

        var devices = await Page.NavigateToDevicesAsync();
        var discovery = await devices.NavigateToDiscoveryAsync();

        var deviceId = await _deconzSimulator.JoinPhilipsLightAsync();

        await Expect(discovery.GetUnassignedDevicesDropZone())
            .ToContainTextAsync(deviceId, new LocatorAssertionsToContainTextOptions { Timeout = 10_000 });
    }

    [TearDown]
    public async Task AfterEach()
    {
        await Context.StopTracingAsync();
    }
}
