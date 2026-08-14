using System;
using System.Net.Http;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Haus.Acceptance.Tests.Support;

public class HausPageTest : PageTest
{
    // Shared across every test fixture instance rather than one per instance: the base address is
    // constant, and HttpClient is designed to be reused rather than created/disposed per use.
    private static readonly HttpClient SimulatorHttpClient = new() { BaseAddress = new("http://localhost:15004") };

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
        return new DeconzSimulatorClient(SimulatorHttpClient);
    }
}
