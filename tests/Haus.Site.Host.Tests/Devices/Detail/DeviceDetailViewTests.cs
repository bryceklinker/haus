using System.Linq;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Site.Host.Devices.Detail;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using MudBlazor;
using MudBlazor.Extensions;

namespace Haus.Site.Host.Tests.Devices.Detail;

public class DeviceDetailViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedThenShowsDeviceInformation()
    {
        var device = HausModelFactory.DeviceModel();

        var page = Context.Render<DeviceDetailView>(opts =>
        {
            opts.Add(p => p.Device, device);
        });

        Assert.Equal(device.Name, page.FindMudTextFieldById<string>("name").Instance.GetState(x => x.Value));
        Assert.Equal(
            device.DeviceType,
            page.FindMudTextFieldById<DeviceType?>("deviceType").Instance.GetState(x => x.Value)
        );
        Assert.Equal(device.ExternalId, page.FindMudTextFieldById<string>("externalId").Instance.Value);
        Assert.Equal(device.LightType, page.FindMudTextFieldById<LightType>("lightType").Instance.Value);
        Assert.Equal(device.RoomId, page.FindMudTextFieldById<long?>("roomId").Instance.Value);
    }

    [Fact]
    public void WhenRenderedThenShowsTheDevicesMetadataValues()
    {
        var device = HausModelFactory.DeviceModel() with
        {
            Metadata = [new MetadataModel(Key: "Watts", Value: "Lots")],
        };

        var page = Context.Render<DeviceDetailView>(opts =>
        {
            opts.Add(p => p.Device, device);
        });

        Assert.Equal("Watts", page.FindMudTextFieldById<string>("key").Instance.Value);
        Assert.Equal("Lots", page.FindMudTextFieldById<string>("value").Instance.Value);
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

        var page = Context.Render<DeviceDetailView>(opts =>
        {
            opts.Add(p => p.Device, device);
        });

        Assert.Equal(3, page.FindAllByComponent<MudListItem<MetadataModel>>().Count());
    }
}
