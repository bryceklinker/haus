using System.Net.Http;
using System.Threading.Tasks;
using Haus.Site.Host.Devices.Detail;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Devices.Detail;

public class DeleteDeviceDialogViewTests : HausSiteTestContext
{
    [Fact]
    public async Task WhenOpenedThenShowsTheDeviceName()
    {
        var device = HausModelFactory.DeviceModel() with { Name = "Living Room Lamp" };
        var parameters = new DialogParameters<DeleteDeviceDialogView> { { x => x.Device, device } };

        var view = await RenderDialogAsync<DeleteDeviceDialogView>(parameters);

        Assert.Contains("Living Room Lamp", view.Markup);
    }

    [Fact]
    public async Task WhenDeleteIsConfirmedThenDeletesDeviceFromApi()
    {
        var device = HausModelFactory.DeviceModel();
        var parameters = new DialogParameters<DeleteDeviceDialogView> { { x => x.Device, device } };
        HttpRequestMessage? request = null;
        await HausApiHandler.SetupDeleteAsJson<object?>(
            $"/api/devices/{device.Id}",
            opts => opts.WithCapture(r => request = r)
        );

        var view = await RenderDialogAsync<DeleteDeviceDialogView>(parameters);

        await view.InvokeAsync(async () =>
        {
            await view.FindMudButtonByText("delete").ClickAsync();
        });

        await Eventually.AssertAsync(async () =>
        {
            Assert.NotNull(request);
            Assert.Equal(HttpMethod.Delete, request?.Method);
            await Task.CompletedTask;
        });
    }

    [Fact]
    public async Task WhenDeleteIsConfirmedThenClosesDialog()
    {
        var device = HausModelFactory.DeviceModel();
        var parameters = new DialogParameters<DeleteDeviceDialogView> { { x => x.Device, device } };
        await HausApiHandler.SetupDeleteAsJson<object?>($"/api/devices/{device.Id}");

        var view = await RenderDialogAsync<DeleteDeviceDialogView>(parameters);

        await view.InvokeAsync(async () =>
        {
            await view.FindMudButtonByText("delete").ClickAsync();
        });

        Eventually.Assert(() =>
        {
            Assert.Empty(view.FindAllByComponent<MudDialog>());
        });
    }

    [Fact]
    public async Task WhenDeleteIsCancelledThenDialogIsClosedWithoutCallingApi()
    {
        var device = HausModelFactory.DeviceModel();
        var parameters = new DialogParameters<DeleteDeviceDialogView> { { x => x.Device, device } };
        HttpRequestMessage? request = null;
        await HausApiHandler.SetupDeleteAsJson<object?>(
            $"/api/devices/{device.Id}",
            opts => opts.WithCapture(r => request = r)
        );

        var view = await RenderDialogAsync<DeleteDeviceDialogView>(parameters);

        await view.InvokeAsync(async () =>
        {
            await view.FindMudButtonByText("cancel").ClickAsync();
        });

        Eventually.Assert(() =>
        {
            Assert.Empty(view.FindAllByComponent<MudDialog>());
        });
        Assert.Null(request);
    }
}
