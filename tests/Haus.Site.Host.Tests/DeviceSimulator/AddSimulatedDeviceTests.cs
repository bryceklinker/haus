using System;
using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Site.Host.DeviceSimulator;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Tests.Support.Http;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Haus.Site.Host.Tests.DeviceSimulator;

public class AddSimulatedDeviceTests : HausSiteTestContext
{
    private const string DeviceTypesUrl = "/api/deviceTypes";
    
    [Fact]
    public async Task WhenLoadingTakesAwhileThenLoadingSpinnerIsShown()
    {
        await HausApiHandler.SetupGetAsJson(DeviceTypesUrl, new ListResult<DeviceType>(),
            new ConfigureHttpResponseWithStatus
            {
                Delay = TimeSpan.FromSeconds(1)
            });

        var page = Context.RenderComponent<AddSimulatedDevice>();

        page.WaitForAssertion(() => page.FindComponents<MudProgressCircular>().Should().HaveCount(1));
    }
    
    [Fact]
    public async Task WhenRenderedThenDeviceTypesAreAvailable()
    {
        await SetupDeviceTypes(DeviceType.Switch, DeviceType.Light);

        var page = Context.RenderComponent<AddSimulatedDevice>();

        page.WaitForAssertion(() => page.FindAll("div.mud-popover").Should().HaveCountGreaterThan(0));
        page.Find("div.mud-input-control").Click();
        page.WaitForAssertion(() => page.FindAll("div.mud-popover-open").Should().HaveCountGreaterThan(0));
        await page.FindAllByClass("mud-list-item").First(i => i.TextContent.Contains("Light")).ClickAsync(new MouseEventArgs());
        
        page.FindByTag("input").GetAttribute("value").Should().Be(nameof(DeviceType.Light));
    }

    private async Task SetupDeviceTypes(params DeviceType[] deviceTypes)
    {
        await HausApiHandler.SetupGetAsJson(DeviceTypesUrl, new ListResult<DeviceType>(deviceTypes));
    }
}