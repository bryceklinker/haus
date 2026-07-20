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
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Haus.Site.Host.Tests.DeviceSimulator;

public class AddSimulatedDeviceViewTests : HausSiteTestContext
{
    private const string DeviceTypesUrl = "/api/device-types";
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

        var page = RenderView<AddSimulatedDeviceView>();

        Eventually.Assert(() =>
        {
            Assert.Equal(1, page.FindComponents<MudProgressCircular>().Count);
        });
    }

    [Fact]
    public async Task WhenRenderedThenDeviceTypesAreAvailable()
    {
        await SetupDeviceTypes(DeviceType.Switch, DeviceType.Light);

        var popover = RenderView<MudPopoverProvider>();
        var page = RenderView<AddSimulatedDeviceView>();
        await OpenDeviceTypes(page);

        Eventually.Assert(() =>
        {
            Assert.Equal(2, popover.FindAllByComponent<MudSelectItem<DeviceType?>>().Count());
        });
    }

    [Fact]
    public async Task WhenSavedThenSendsDeviceToSimulator()
    {
        await SetupDeviceTypes(DeviceType.Switch, DeviceType.Light);
        await SetupSaveSimulatedDevice();

        var page = RenderView<AddSimulatedDeviceView>();
        await SelectDeviceType(page, DeviceType.Switch);
        await FindSaveButton(page).ClickAsync(new MouseEventArgs());

        Assert.Equal(1, _savedDevices.Count);
        Assert.Equivalent(new SimulatedDeviceModel { DeviceType = DeviceType.Switch }, _savedDevices.Single());
    }

    [Fact]
    public async Task WhenSavedThenGoesBackToPreviousPage()
    {
        await SetupDeviceTypes(DeviceType.Switch, DeviceType.Light);
        await SetupSaveSimulatedDevice();

        var page = RenderView<AddSimulatedDeviceView>();
        await SelectDeviceType(page, DeviceType.Switch);
        await FindSaveButton(page).ClickAsync(new MouseEventArgs());

        Assert.EndsWith("/device-simulator", NavigationManager.Uri);
    }

    [Fact]
    public async Task WhenDeviceTypeHasNotBeenSelectedThenSaveIsPrevented()
    {
        await SetupDeviceTypes(DeviceType.Switch, DeviceType.Light);
        await SetupSaveSimulatedDevice();

        var page = RenderView<AddSimulatedDeviceView>();
        await FindSaveButton(page).ClickAsync(new MouseEventArgs());

        Assert.Equal(0, _savedDevices.Count);
    }

    [Fact]
    public async Task WhenUserEntersDeviceMetadataThenDeviceMetadataIsSaved()
    {
        await SetupDeviceTypes(DeviceType.Switch, DeviceType.Light);
        await SetupSaveSimulatedDevice();

        var page = RenderView<AddSimulatedDeviceView>();
        await FindAddMetadata(page).ClickAsync(new MouseEventArgs());
        await EnterMetadata(page, "external", "true");

        await SelectDeviceType(page, DeviceType.Light);
        await FindSaveButton(page).ClickAsync(new MouseEventArgs());

        Assert.Equal(1, _savedDevices.Count);
        Assert.Equivalent(
            new SimulatedDeviceModel
            {
                DeviceType = DeviceType.Light,
                Metadata = [new MetadataModel("external", "true")],
            },
            _savedDevices.Single()
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

    private async Task OpenDeviceTypes(IRenderedComponent<AddSimulatedDeviceView> view)
    {
        await view.InvokeAsync(async () =>
        {
            await view.FindByComponent<MudSelect<DeviceType?>>().Instance.OpenMenu();
        });
    }

    private async Task SelectDeviceType(IRenderedComponent<AddSimulatedDeviceView> view, DeviceType deviceType)
    {
        await OpenDeviceTypes(view);

        await view.InvokeAsync(async () =>
        {
            await view.FindByComponent<MudSelect<DeviceType?>>().Instance.ValueChanged.InvokeAsync(deviceType);
        });
    }

    private static IElement FindSaveButton(IRenderedComponent<AddSimulatedDeviceView> page)
    {
        return page.FindByRole("button", opts => opts.WithText("save"));
    }

    private static IElement FindAddMetadata(IRenderedComponent<AddSimulatedDeviceView> page)
    {
        return page.FindByRole("button", opts => opts.WithText("add metadata"));
    }

    private static async Task EnterMetadata(IRenderedComponent<AddSimulatedDeviceView> page, string key, string value)
    {
        var keyInput = page.FindByComponent<MudTextField<string>>(opts => opts.WithId("key"));
        await keyInput.InvokeAsync(() => keyInput.Instance.SetTextAsync(key));

        var valueInput = page.FindByComponent<MudTextField<string>>(opts => opts.WithId("value"));
        await valueInput.InvokeAsync(() => valueInput.Instance.SetTextAsync(value));
    }
}
