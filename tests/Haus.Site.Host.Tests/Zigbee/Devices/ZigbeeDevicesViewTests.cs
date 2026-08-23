using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Zigbee;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Zigbee.Devices;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Zigbee.Devices;

public class ZigbeeDevicesViewTests : HausSiteTestContext
{
    private const string DevicesUrl = "/api/zigbee/devices";

    [Fact]
    public async Task WhenRenderedThenShowsLoadingDevices()
    {
        await HausApiHandler.SetupGetAsJson(
            DevicesUrl,
            new ListResult<ZigbeeKnownDeviceModel>(),
            opts => opts.WithDelayMs(1000)
        );

        var view = RenderView<ZigbeeDevicesView>();

        Assert.Single(view.FindAllByComponent<MudProgressCircular>());
    }

    [Fact]
    public async Task WhenNoDevicesThenShowsEmptyMessage()
    {
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<ZigbeeKnownDeviceModel>());

        var view = RenderView<ZigbeeDevicesView>();

        Eventually.Assert(() =>
        {
            view.FindByComponent<MudText>(opts => opts.WithText("No zigbee devices discovered yet"));
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsEachKnownDevice()
    {
        await HausApiHandler.SetupGetAsJson(
            DevicesUrl,
            new ListResult<ZigbeeKnownDeviceModel>([
                HausModelFactory.ZigbeeKnownDeviceModel(),
                HausModelFactory.ZigbeeKnownDeviceModel(),
                HausModelFactory.ZigbeeKnownDeviceModel(),
            ])
        );

        var view = RenderView<ZigbeeDevicesView>();

        Eventually.Assert(() =>
        {
            Assert.Equal(3, view.FindAllByComponent<ZigbeeDeviceView>().Count());
        });
    }
}
