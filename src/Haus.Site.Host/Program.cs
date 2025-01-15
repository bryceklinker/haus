using Haus.Site.Host;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddHausSiteServices(builder.Configuration);

builder.RootComponents.AddHausSiteComponents();

var host = builder.Build();

await host.RunAsync();
