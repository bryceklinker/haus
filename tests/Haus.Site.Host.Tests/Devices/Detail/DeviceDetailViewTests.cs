using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Lighting;
using Haus.Site.Host.Devices.Detail;
using Haus.Site.Host.Shared.Lighting;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using Microsoft.AspNetCore.Components.Web;
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
        Assert.Equal(
            device.ExternalId,
            page.FindMudTextFieldById<string>("externalId").Instance.GetState(x => x.Value)
        );
        Assert.Equal(
            device.LightType,
            page.FindMudTextFieldById<LightType>("lightType").Instance.GetState(x => x.Value)
        );
        Assert.Equal(device.RoomId, page.FindMudTextFieldById<long?>("roomId").Instance.GetState(x => x.Value));
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

        Assert.Equal("Watts", page.FindMudTextFieldById<string>("key").Instance.GetState(x => x.Value));
        Assert.Equal("Lots", page.FindMudTextFieldById<string>("value").Instance.GetState(x => x.Value));
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

    [Fact]
    public async Task WhenDeleteButtonClickedThenShowsDeleteConfirmationDialogForTheDevice()
    {
        var device = HausModelFactory.DeviceModel();

        var dialogProvider = Context.Render<MudDialogProvider>();
        var page = Context.Render<DeviceDetailView>(opts =>
        {
            opts.Add(p => p.Device, device);
        });

        await page.InvokeAsync(async () =>
        {
            await page.FindByComponent<MudIconButton>().Instance.OnClick.InvokeAsync(new MouseEventArgs());
        });

        Eventually.Assert(() =>
        {
            var dialog = dialogProvider.FindComponent<DeleteDeviceDialogView>();
            Assert.Equal(device, dialog.Instance.Device);
        });
    }

    [Fact]
    public void WhenDeviceIsLightWithFullLightingThenOnlyOneDeleteButtonIsRendered()
    {
        var device = HausModelFactory.DeviceModel() with
        {
            DeviceType = DeviceType.Light,
            Lighting = new LightingModel(
                State: LightingState.On,
                Level: new LevelLightingModel(),
                Temperature: new TemperatureLightingModel(),
                Color: new ColorLightingModel()
            ),
        };

        var page = Context.Render<DeviceDetailView>(opts =>
        {
            opts.Add(p => p.Device, device);
        });

        // LightingView's MudSwitch also renders markup carrying MudBlazor's generic
        // "mud-icon-button" class, so the delete button needs its own selector to stay
        // uniquely identifiable (regression test for that collision).
        Assert.Single(page.FindAll(".delete-device-button"));
    }

    [Fact]
    public void WhenDeviceTypeIsLightThenShowsLightingView()
    {
        var device = HausModelFactory.DeviceModel() with { DeviceType = DeviceType.Light };

        var page = Context.Render<DeviceDetailView>(opts =>
        {
            opts.Add(p => p.Device, device);
        });

        Assert.NotEmpty(page.FindAllByComponent<LightingView>());
    }

    [Fact]
    public void WhenDeviceTypeIsNotLightThenDoesNotShowLightingView()
    {
        var device = HausModelFactory.DeviceModel() with { DeviceType = DeviceType.MotionSensor };

        var page = Context.Render<DeviceDetailView>(opts =>
        {
            opts.Add(p => p.Device, device);
        });

        Assert.Empty(page.FindAllByComponent<LightingView>());
    }

    [Fact]
    public async Task WhenLightingIsChangedThenDeviceLightingIsUpdated()
    {
        HttpRequestMessage? lightingRequest = null;
        await HausApiHandler.SetupPutAsJson(
            "/api/devices/8/lighting",
            new { },
            opts => opts.WithCapture(r => lightingRequest = r)
        );
        var device = HausModelFactory.DeviceModel() with { Id = 8, DeviceType = DeviceType.Light };
        await HausApiHandler.SetupGetAsJson("/api/devices/8", device);

        var page = Context.Render<DeviceDetailView>(opts =>
        {
            opts.Add(p => p.Device, device);
        });

        var lighting = HausModelFactory.LightingModel();
        await page.InvokeAsync(async () =>
        {
            await page.FindByComponent<LightingView>().Instance.OnLightingChanged.InvokeAsync(lighting);
        });

        await Eventually.AssertAsync(async () =>
        {
            var newLighting =
                lightingRequest?.Content == null
                    ? null
                    : await lightingRequest.Content.ReadFromJsonAsync<LightingModel>();

            Assert.Equal(lighting, newLighting);
        });
    }
}
