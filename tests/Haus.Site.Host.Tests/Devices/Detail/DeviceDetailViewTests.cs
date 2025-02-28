using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Site.Host.Devices.Detail;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Devices.Detail;

public class DeviceDetailViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedThenShowsDeviceInformation()
    {
        var device = HausModelFactory.DeviceModel();

        var page = Context.RenderComponent<DeviceDetailView>(opts =>
        {
            opts.Add(p => p.Device, device);
        });

        page.FindMudTextFieldById<string>("name").Instance.Value.Should().Be(device.Name);
        page.FindMudTextFieldById<DeviceType?>("deviceType").Instance.Value.Should().Be(device.DeviceType);
        page.FindMudTextFieldById<string>("externalId").Instance.Value.Should().Be(device.ExternalId);
        page.FindMudTextFieldById<LightType>("lightType").Instance.Value.Should().Be(device.LightType);
        page.FindMudTextFieldById<long?>("roomId").Instance.Value.Should().Be(device.RoomId);
    }

    [Fact]
    public void WhenRenderedThenShowsTheDevicesMetadataValues()
    {
        var device = HausModelFactory.DeviceModel() with
        {
            Metadata = [new MetadataModel(Key: "Watts", Value: "Lots")],
        };

        var page = Context.RenderComponent<DeviceDetailView>(opts =>
        {
            opts.Add(p => p.Device, device);
        });

        page.FindMudTextFieldById<string>("key").Instance.Value.Should().Be("Watts");
        page.FindMudTextFieldById<string>("value").Instance.Value.Should().Be("Lots");
    }

    [Fact]
    public void WhenRenderedThenShowsEachPieceOfMetadata()
    {
        var device = HausModelFactory.DeviceModel() with
        {
            Metadata =
            [
                HausModelFactory.MetadataModel(),
                HausModelFactory.MetadataModel(),
                HausModelFactory.MetadataModel(),
            ],
        };

        var page = Context.RenderComponent<DeviceDetailView>(opts =>
        {
            opts.Add(p => p.Device, device);
        });

        page.FindAllByComponent<MudListItem<MetadataModel>>().Should().HaveCount(3);
    }
}
