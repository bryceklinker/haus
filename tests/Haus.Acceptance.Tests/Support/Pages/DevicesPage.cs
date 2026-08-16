using System;
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
        // Goes through the browser (not GetDevicesAsync directly) so the returned ids line up with
        // what's on screen. A GotoAsync forces a fresh navigation even when already on /devices --
        // clicking the "Devices" nav link again would be a same-route no-op for Blazor's router and
        // never re-fetch. DevicesApiClient also always passes a (possibly empty) QueryParameters
        // instance, so the real request URL is ".../api/devices?" even with no filters -- match on
        // the path, not the raw URL.
        var response = await page.RunAndWaitForResponseAsync(
            () => page.GotoAsync("/devices"),
            r => r.Request.Method == "GET" && new Uri(r.Url).AbsolutePath.EndsWith("/api/devices")
        );
        var json = await response.JsonAsync();
        return json!.Value.GetProperty("items");
    }
}
