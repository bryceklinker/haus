using System.Threading.Tasks;
using AngleSharp.Dom.Events;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Site.Host.Devices.List;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;

namespace Haus.Site.Host.Tests.Devices.List;

public class DevicesListViewTests : HausSiteTestContext
{
    private const string DevicesUrl = "/api/devices";

    [Fact]
    public async Task WhenRenderedThenShowsLoadingDevices()
    {
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>(), opts => opts.WithDelayMs(1000));

        var page = Context.RenderComponent<DevicesListView>();

        page.FindAllByComponent<MudProgressCircular>().Should().HaveCount(1);
    }

    [Fact]
    public async Task WhenRenderedThenShowsListOfDevices()
    {
        await HausApiHandler.SetupGetAsJson(
            DevicesUrl,
            new ListResult<DeviceModel>(
                [HausModelFactory.DeviceModel(), HausModelFactory.DeviceModel(), HausModelFactory.DeviceModel()]
            )
        );

        var page = Context.RenderComponent<DevicesListView>();

        Eventually.Assert(() =>
        {
            page.FindAllByComponent<MudListItem<DeviceModel>>().Should().HaveCount(3);
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsNameAndTypeOfDevice()
    {
        await HausApiHandler.SetupGetAsJson(
            DevicesUrl,
            new ListResult<DeviceModel>(
                [HausModelFactory.DeviceModel() with { DeviceType = DeviceType.MotionSensor, Name = "Motions" }]
            )
        );

        var page = Context.RenderComponent<DevicesListView>();

        Eventually.Assert(() =>
        {
            var item = page.FindByComponent<MudListItem<DeviceModel>>();
            item.Instance.Text.Should().Contain("Motions");
            item.Instance.SecondaryText.Should().Contain("Motion Sensor");
        });
    }

    [Fact]
    public async Task WhenDeviceSelectedThenShowsDeviceDetails()
    {
        await HausApiHandler.SetupGetAsJson(
            DevicesUrl,
            new ListResult<DeviceModel>(
                [HausModelFactory.DeviceModel() with { Id = 5, DeviceType = DeviceType.MotionSensor, Name = "Motions" }]
            )
        );

        var page = Context.RenderComponent<DevicesListView>();
        var deviceItem = page.FindByComponent<MudListItem<DeviceModel>>();
        await page.InvokeAsync(async () =>
        {
            await deviceItem.Instance.OnClick.InvokeAsync(new MouseEventArgs());
        });

        Eventually.Assert(() =>
        {
            var navigation = page.Services.GetRequiredService<FakeNavigationManager>();
            navigation.Uri.Should().EndWith("/devices/5");
        });
    }
}
