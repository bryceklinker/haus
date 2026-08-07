using Haus.Zigbee.Simulator;
using Haus.Zigbee.Simulator.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SimulatorOptions>(builder.Configuration.GetSection("Simulator"));

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields =
        HttpLoggingFields.RequestMethod | HttpLoggingFields.RequestPath | HttpLoggingFields.ResponseStatusCode;
});
builder.Services.AddControllers();
builder.Services.AddSingleton<DeconzResponder>();
builder.Services.AddHostedService<TcpDongleListener>();

var app = builder.Build();

app.UseHttpLogging();
app.MapControllers();

app.Run();

// Exposes the top-level-statement entry point to WebApplicationFactory<Program> in tests.
public partial class Program { }
