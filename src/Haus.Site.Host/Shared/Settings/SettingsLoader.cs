using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Haus.Site.Host.Shared.Settings;

public class SettingsLoader(IHttpClientFactory httpClientFactory, IWebAssemblyHostEnvironment env)
{
    private string SettingsUrl => $"{env.BaseAddress}/settings";
    
    public async Task<SettingsState> LoadAsync()
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync(SettingsUrl);
        var json = await response.Content.ReadAsStringAsync();
        var result = await response.Content.ReadFromJsonAsync<SettingsState>();
        if (result == null)
            throw new InvalidOperationException($"Settings could not be loaded from: {SettingsUrl}");
        return result;
    }
}