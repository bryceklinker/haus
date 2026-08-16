using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Support.Pages;

public class DevicesPage(IPage page)
{
    public async Task NavigateAsync()
    {
        await page.ClickLinkAsync("Devices");
    }

    public async Task<DevicesDiscoveryPage> NavigateToDiscoveryAsync()
    {
        await NavigateAsync();
        var discovery = new DevicesDiscoveryPage(page);
        await discovery.NavigateAsync();
        return discovery;
    }

    public async Task<DeviceDetailPage> NavigateToDeviceAsync(string externalId)
    {
        var deviceId = await FindDeviceIdAsync(externalId);
        await page.GotoAsync($"/devices/{deviceId}");
        return new DeviceDetailPage(page, deviceId);
    }

    public async Task<bool> ContainsDeviceAsync(string externalId)
    {
        var items = await GetDeviceListItemsAsync();
        return items.EnumerateArray().Any(item => item.GetProperty("externalId").GetString() == externalId);
    }

    private async Task<long> FindDeviceIdAsync(string externalId)
    {
        var items = await GetDeviceListItemsAsync();
        return items
            .EnumerateArray()
            .First(item => item.GetProperty("externalId").GetString() == externalId)
            .GetProperty("id")
            .GetInt64();
    }

    private async Task<JsonElement> GetDeviceListItemsAsync()
    {
        var response = await page.RunAndWaitForResponseAsync(
            NavigateAsync,
            r => r.Request.Method == "GET" && r.Url.EndsWith("/api/devices")
        );
        var json = await response.JsonAsync();
        return json!.Value.GetProperty("items");
    }
}
