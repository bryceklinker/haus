using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.DeviceSimulator;
using Haus.Site.Host.DeviceSimulator;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Haus.Site.Host.Tests.DeviceSimulator;

public class AddSimulatedDeviceTests : HausSiteTestContext
{
    private const string DeviceTypesUrl = "/api/deviceTypes";
    private const string DeviceSimulatorDevicesUrl = "/api/device-simulator/devices";
    private readonly ConcurrentBag<SimulatedDeviceModel> _savedDevices = [];

    [Fact]
    public async Task WhenLoadingTakesAwhileThenLoadingSpinnerIsShown()
    {
        await HausApiHandler.SetupGetAsJson(
            DeviceTypesUrl,
            new ListResult<DeviceType>(),
            opts => opts.WithDelay(TimeSpan.FromSeconds(1))
        );

        var page = Context.RenderComponent<AddSimulatedDevice>();

        Eventually.Assert(() =>
        {
            page.FindComponents<MudProgressCircular>().Should().HaveCount(1);
        });
    }

    [Fact]
    public async Task WhenRenderedThenDeviceTypesAreAvailable()
    {
        await SetupDeviceTypes(DeviceType.Switch, DeviceType.Light);

        var page = Context.RenderComponent<AddSimulatedDevice>();
        OpenDeviceTypes(page);

        Eventually.Assert(() =>
        {
            page.FindAllByClass("mud-list-item").Should().HaveCount(2);
        });
    }

    [Fact]
    public async Task WhenSavedThenSendsDeviceToSimulator()
    {
        await SetupDeviceTypes(DeviceType.Switch, DeviceType.Light);
        await SetupSaveSimulatedDevice();

        var page = Context.RenderComponent<AddSimulatedDevice>();
        await SelectDeviceType(page, DeviceType.Switch);
        await FindSaveButton(page).ClickAsync(new MouseEventArgs());

        _savedDevices.Should().HaveCount(1);
        _savedDevices.Should().ContainEquivalentOf(new SimulatedDeviceModel { DeviceType = DeviceType.Switch });
    }

    [Fact]
    public async Task WhenDeviceTypeHasNotBeenSelectedThenSaveIsPrevented()
    {
        await SetupDeviceTypes(DeviceType.Switch, DeviceType.Light);
        await SetupSaveSimulatedDevice();

        var page = Context.RenderComponent<AddSimulatedDevice>();
        await FindSaveButton(page).ClickAsync(new MouseEventArgs());

        _savedDevices.Should().HaveCount(0);
    }

    [Fact]
    public async Task WhenUserEntersDeviceMetadataThenDeviceMetadataIsSaved()
    {
        await SetupDeviceTypes(DeviceType.Switch, DeviceType.Light);
        await SetupSaveSimulatedDevice();

        var page = Context.RenderComponent<AddSimulatedDevice>();
        await FindAddMetadata(page).ClickAsync(new MouseEventArgs());
        await EnterMetadata(page, "external", "true");

        await SelectDeviceType(page, DeviceType.Light);
        await FindSaveButton(page).ClickAsync(new MouseEventArgs());

        _savedDevices.Should().HaveCount(1);
        _savedDevices
            .Should()
            .ContainEquivalentOf(
                new SimulatedDeviceModel
                {
                    DeviceType = DeviceType.Light,
                    Metadata = [new MetadataModel("external", "true")],
                }
            );
    }

    private async Task SetupDeviceTypes(params DeviceType[] deviceTypes)
    {
        await HausApiHandler.SetupGetAsJson(DeviceTypesUrl, new ListResult<DeviceType>(deviceTypes));
    }

    private async Task SetupSaveSimulatedDevice()
    {
        await HausApiHandler.SetupPostAsJson<object?>(
            DeviceSimulatorDevicesUrl,
            null,
            opts =>
                opts.WithCapture(async req =>
                {
                    var device =
                        req.Content != null ? await req.Content.ReadFromJsonAsync<SimulatedDeviceModel>() : null;
                    if (device != null)
                    {
                        _savedDevices.Add(device);
                    }
                })
        );
    }

    private void OpenDeviceTypes(IRenderedComponent<AddSimulatedDevice> page)
    {
        page.WaitForAssertion(() => page.FindAll("div.mud-popover").Should().HaveCountGreaterThan(0));
        page.Find("div.mud-input-control").Click();
        page.WaitForAssertion(() => page.FindAll("div.mud-popover-open").Should().HaveCountGreaterThan(0));
    }

    private async Task SelectDeviceType(IRenderedComponent<AddSimulatedDevice> page, DeviceType deviceType)
    {
        OpenDeviceTypes(page);

        var item = page.FindAllByClass("mud-list-item").First(i => i.TextContent.Contains(Enum.GetName(deviceType)));
        await item.ClickAsync(new MouseEventArgs());
    }

    private static IElement FindSaveButton(IRenderedComponent<AddSimulatedDevice> page)
    {
        return page.FindByRole("button", opts => opts.WithText("save"));
    }

    private static IElement FindAddMetadata(IRenderedComponent<AddSimulatedDevice> page)
    {
        return page.FindByRole("button", opts => opts.WithText("add metadata"));
    }

    private static async Task EnterMetadata(IRenderedComponent<AddSimulatedDevice> page, string key, string value)
    {
        var keyInput = page.FindByComponent<MudTextField<string>>(opts => opts.WithId("key"));
        await keyInput.InvokeAsync(() => keyInput.Instance.SetText(key));

        var valueInput = page.FindByComponent<MudTextField<string>>(opts => opts.WithId("value"));
        await valueInput.InvokeAsync(() => valueInput.Instance.SetText(value));
    }
}
