using System;
using System.Net.Http;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Haus.Acceptance.Tests.Support;

public class HausPageTest : PageTest
{
    private readonly HttpClient _simulatorHttpClient = new() { BaseAddress = new("http://localhost:15004") };

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = "https://localhost:15003",
            IgnoreHTTPSErrors = true,
            ScreenSize = new ScreenSize { Width = 1920, Height = 1080 },
        };
    }

    public DeconzSimulatorClient GetDeconzSimulatorClient()
    {
        return new DeconzSimulatorClient(_simulatorHttpClient);
    }
}
