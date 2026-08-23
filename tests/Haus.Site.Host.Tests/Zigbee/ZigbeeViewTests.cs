using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Zigbee;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Zigbee;
using Haus.Site.Host.Zigbee.Activity;
using Haus.Site.Host.Zigbee.ConnectionStatus;
using Haus.Site.Host.Zigbee.Devices;
using Haus.Testing.Support;

namespace Haus.Site.Host.Tests.Zigbee;

public class ZigbeeViewTests : HausSiteTestContext
{
    [Fact]
    public async Task WhenRenderedThenShowsConnectionStatusDevicesAndActivity()
    {
        await HausApiHandler.SetupGetAsJson("/api/zigbee/status", HausModelFactory.ZigbeeConnectionStatusModel());
        await HausApiHandler.SetupGetAsJson("/api/zigbee/devices", new ListResult<ZigbeeKnownDeviceModel>());
        await HausApiHandler.SetupGetAsJson("/api/zigbee/activity", new ListResult<ZigbeeActivityEntryModel>());

        var view = RenderView<ZigbeeView>();

        Eventually.Assert(() =>
        {
            Assert.Single(view.FindAllByComponent<ZigbeeConnectionStatusView>());
            Assert.Single(view.FindAllByComponent<ZigbeeDevicesView>());
            Assert.Single(view.FindAllByComponent<ZigbeeActivityView>());
        });
    }
}
