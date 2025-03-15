using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.DeviceSimulator;
using Haus.Site.Host.DeviceSimulator;
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

    private IRenderedComponent<SimulatedDeviceView> RenderWithDevice(SimulatedDeviceModel device)
    {
        return RenderView<SimulatedDeviceView>(opts =>
        {
            opts.Add(o => o.SimulatedDevice, device);
        });
    }
}
