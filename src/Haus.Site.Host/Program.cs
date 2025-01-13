using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using Haus.Api.Client;
using Haus.Site.Host;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddFluxor(opts =>
{
    opts.ScanAssemblies(typeof(Program).Assembly)
        .UseReduxDevTools();
});
builder.Services.AddHausApiClient(opts =>
{
    opts.BaseUrl = builder.HostEnvironment.BaseAddress;
});

await builder.Build().RunAsync();
