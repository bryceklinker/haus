using System.Linq;
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
            Assert.Single(view.FindAllByComponent<MudProgressCircular>());
        });
    }

    [Fact]
    public void WhenRenderedThenConnectsToRealtimeDeviceSimulator()
    {
        RenderView<DeviceSimulatorDashboardView>();

        Eventually.Assert(() =>
        {
            Assert.True(_simulatorSubscriber.IsStarted);
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
            Assert.Equal(3, view.FindAllByComponent<SimulatedDeviceView>().Count());
        });
    }

    [Fact]
    public async Task WhenAddingDeviceThenOpensAddSimulatedDeviceDialog()
    {
        var view = RenderView<DeviceSimulatorDashboardView>();

        await view.InvokeAsync(async () =>
        {
            await view.FindMudButtonByText("add simulated device").ClickAsync();
        });

        Eventually.Assert(() =>
        {
            Assert.EndsWith("/device-simulator/add", NavigationManager.Uri);
        });
    }
}
