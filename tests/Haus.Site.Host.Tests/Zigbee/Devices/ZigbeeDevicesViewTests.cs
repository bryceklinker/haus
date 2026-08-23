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
}
