using System.Net.Http;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.DeviceSimulator;
using Haus.Site.Host.DeviceSimulator;
using Haus.Site.Host.Shared.Lighting;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.DeviceSimulator;

public class SimulatedDeviceViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedThenShowsSimulatedDeviceInformation()
    {
        var device = HausModelFactory.SimulatedDeviceModel() with { DeviceType = DeviceType.LightSensor };

        var view = RenderWithDevice(device);

        var idField = view.FindMudTextFieldById<string>("id").Instance;
        idField.Disabled.Should().BeTrue();
        idField.Value.Should().Be(device.Id);

        var deviceTypeField = view.FindMudTextFieldById<string>("deviceType").Instance;
        deviceTypeField.Disabled.Should().BeTrue();
        deviceTypeField.Value.Should().Be($"{DeviceType.LightSensor}");
    }

    [Fact]
    public void WhenRenderedThenShowsSimulatedDeviceMetadata()
    {
        var device = HausModelFactory.SimulatedDeviceModel() with { Metadata = [new MetadataModel("bill", "bob")] };

        var view = RenderWithDevice(device);

        var keyField = view.FindMudTextFieldById<string>("key").Instance;
        keyField.Value.Should().Be("bill");
        keyField.Disabled.Should().BeTrue();

        var valueField = view.FindMudTextFieldById<string>("value").Instance;
        valueField.Disabled.Should().BeTrue();
        valueField.Value.Should().Be("bob");
    }

    [Fact]
    public void WhenSimulatedDeviceIsALightThenShowsLighting()
    {
        var device = HausModelFactory.SimulatedDeviceModel() with
        {
            DeviceType = DeviceType.Light,
            Lighting = HausModelFactory.LightingModel(),
        };

        var view = RenderWithDevice(device);

        var lighting = view.FindByComponent<LightingView>();
        lighting.Instance.Lighting.Should().Be(device.Lighting);
        lighting.Instance.Disabled.Should().BeTrue();
    }

    [Fact]
    public void WhenSimulatedDeviceIsMotiongSensorThenAllowsOccupancyToggle()
    {
        var device = HausModelFactory.SimulatedDeviceModel() with
        {
            DeviceType = DeviceType.MotionSensor,
            IsOccupied = true,
        };

        var view = RenderWithDevice(device);

        var toggle = view.FindByComponent<MudSwitch<bool>>();
        toggle.Instance.Value.Should().BeTrue();
        toggle.Instance.Disabled.Should().BeTrue();
    }

    [Fact]
    public async Task WhenSimulatedDeviceTriggersOccupancyChangeThenChangesDeviceOccupancy()
    {
        var device = HausModelFactory.SimulatedDeviceModel() with
        {
            DeviceType = DeviceType.MotionSensor,
            IsOccupied = true,
        };
        HttpRequestMessage? request = null;
        await HausApiHandler.SetupPostAsJson(
            $"/api/device-simulator/devices/{device.Id}/trigger-occupancy-change",
            new { },
            opts => opts.WithCapture(r => request = r)
        );

        var view = RenderWithDevice(device);
        await view.InvokeAsync(async () =>
        {
            await view.FindMudButtonByText("trigger occupancy change").ClickAsync();
        });

        Eventually.Assert(() =>
        {
            request.Should().NotBeNull();
        });
    }

    private IRenderedComponent<SimulatedDeviceView> RenderWithDevice(SimulatedDeviceModel device)
    {
        return RenderView<SimulatedDeviceView>(opts =>
        {
            opts.Add(o => o.SimulatedDevice, device);
        });
    }
}
