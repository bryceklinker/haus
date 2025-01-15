using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Site.Host.DeviceSimulator;
using Haus.Site.Host.Shared.State;
using Haus.Site.Host.Tests.Support;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Haus.Site.Host.Tests.DeviceSimulator;

public class AddSimulatedDeviceTests : HausSiteTestContext
{
    [Fact]
    public async Task WhenRenderedThenDeviceTypesAreAvailable()
    {
        await SetupDeviceTypes(DeviceType.Switch, DeviceType.Light);

        var page = RenderComponent<AddSimulatedDevice>();
        page.WaitForAssertion(() => GetCapturedActions<LoadListSuccessAction<DeviceType>>().Should().HaveCount(1));
        await page.InvokeAsync(async () =>
        {
            await page.FindComponent<MudSelect<DeviceType?>>().Instance.ValueChanged.InvokeAsync(DeviceType.Light);
        });
        
        page.FindByTag("input").GetAttribute("value").Should().Be(nameof(DeviceType.Light));
    }

    private async Task SetupDeviceTypes(params DeviceType[] deviceTypes)
    {
        await HausApiHandler.SetupGetAsJson("/api/deviceTypes", new ListResult<DeviceType>(deviceTypes));
    }
}