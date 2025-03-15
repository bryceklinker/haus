using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.DeviceSimulator;
using Haus.Site.Host.DeviceSimulator;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Tests.Support.Realtime;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.DeviceSimulator;

public class DeviceSimulatorDashboardViewTests : HausSiteTestContext
{
    private readonly InMemoryRealtimeDataSubscriber _simulatorSubscriber;

    public DeviceSimulatorDashboardViewTests()
    {
        _simulatorSubscriber = GetSubscriber(HausRealtimeSources.DeviceSimulator);
    }

    [Fact]
    public void WhenConnectingToRealtimeDataThenShowsLoading()
    {
        _simulatorSubscriber.ConfigureStartDelayMs(500);

        var view = RenderView<DeviceSimulatorDashboardView>();

        Eventually.Assert(() =>
        {
            view.FindAllByComponent<MudProgressCircular>().Should().HaveCount(1);
        });
    }

    [Fact]
    public void WhenRenderedThenConnectsToRealtimeDeviceSimulator()
    {
        RenderView<DeviceSimulatorDashboardView>();

        Eventually.Assert(() =>
        {
            _simulatorSubscriber.IsStarted.Should().BeTrue();
        });
    }

    [Fact]
    public async Task WhenDeviceSimulatorStateReceivedThenShowsSimulatedDevices()
    {
        var simulatorState = HausModelFactory.DeviceSimulatorStateModel() with
        {
            Devices =
            [
                HausModelFactory.SimulatedDeviceModel(),
                HausModelFactory.SimulatedDeviceModel(),
                HausModelFactory.SimulatedDeviceModel(),
            ],
        };

        var view = RenderView<DeviceSimulatorDashboardView>();
        await _simulatorSubscriber.SimulateAsync(DeviceSimulatorEventNames.OnState, simulatorState);

        Eventually.Assert(() =>
        {
            view.FindAllByComponent<SimulatedDeviceView>().Should().HaveCount(3);
        });
    }
}
