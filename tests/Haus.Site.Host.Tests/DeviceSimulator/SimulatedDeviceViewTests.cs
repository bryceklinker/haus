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
using MudBlazor.Extensions;

namespace Haus.Site.Host.Tests.DeviceSimulator;

public class SimulatedDeviceViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedThenShowsSimulatedDeviceInformation()
    {
        var device = HausModelFactory.SimulatedDeviceModel() with { DeviceType = DeviceType.LightSensor };

        var view = RenderWithDevice(device);

        var idField = view.FindMudTextFieldById<string>("id").Instance;
        Assert.True(idField.GetState(x => x.Disabled));
        Assert.Equal(device.Id, idField.GetState(x => x.Value));

        var deviceTypeField = view.FindMudTextFieldById<string>("deviceType").Instance;
        Assert.True(deviceTypeField.GetState(x => x.Disabled));
        Assert.Equal($"{DeviceType.LightSensor}", deviceTypeField.GetState(x => x.Value));
    }

    [Fact]
    public void WhenRenderedThenShowsSimulatedDeviceMetadata()
    {
        var device = HausModelFactory.SimulatedDeviceModel() with { Metadata = [new MetadataModel("bill", "bob")] };

        var view = RenderWithDevice(device);

        var keyField = view.FindMudTextFieldById<string>("key").Instance;
        Assert.Equal("bill", keyField.GetState(x => x.Value));
        Assert.True(keyField.GetState(x => x.Disabled));

        var valueField = view.FindMudTextFieldById<string>("value").Instance;
        Assert.True(valueField.GetState(x => x.Disabled));
        Assert.Equal("bob", valueField.GetState(x => x.Value));
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
        Assert.Equal(device.Lighting, lighting.Instance.Lighting);
        Assert.True(lighting.Instance.Disabled);
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
        Assert.True(toggle.Instance.GetState(x => x.Value));
        Assert.True(toggle.Instance.GetState(x => x.Disabled));
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
            Assert.NotNull(request);
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
