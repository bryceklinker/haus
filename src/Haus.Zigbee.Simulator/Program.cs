using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee;
using Haus.Zigbee.Simulator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

var tcpPort = builder.Configuration.GetValue<int?>("Simulator:TcpPort") ?? 4901;

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields =
        HttpLoggingFields.RequestMethod | HttpLoggingFields.RequestPath | HttpLoggingFields.ResponseStatusCode;
});
builder.Services.AddSingleton<DeconzResponder>();
builder.Services.AddHostedService(provider => new TcpDongleListener(
    provider.GetRequiredService<DeconzResponder>(),
    provider.GetRequiredService<ILoggerFactory>(),
    tcpPort
));

var app = builder.Build();

app.UseHttpLogging();

app.MapGet("/", () => Results.Ok(new { status = "running", tcpPort }));

app.MapGet(
    "/aps-requests",
    (DeconzResponder responder) => Results.Ok(responder.SentApsRequests.Select(request => Convert.ToHexString(request)))
);

app.MapPost(
    "/parameters/{id}",
    (byte id, byte[] value, DeconzResponder responder) =>
    {
        responder.SetParameter(id, value);
        return Results.NoContent();
    }
);

app.MapPost(
    "/indications",
    (IndicationRequest request, DeconzResponder responder) =>
    {
        byte[] asdu;
        try
        {
            asdu = Convert.FromHexString(request.AsduHex);
        }
        catch (FormatException)
        {
            return Results.BadRequest("asduHex must be a valid hex string");
        }

        responder.EnqueueIndication(
            new IndicationBody(request.SourceNwk, request.SourceEndpoint, request.ProfileId, request.ClusterId, asdu)
        );
        return Results.Accepted();
    }
);

app.MapPost(
    "/devices/join",
    async (JoinDeviceRequest request, DeconzResponder responder) =>
    {
        if (!IeeeAddress.TryParse(request.Ieee, out var ieeeAddress))
            return Results.BadRequest("ieee must be a 0x-prefixed 16-hex-digit address");

        var networkAddress = responder.AllocateNetworkAddress();
        var targetApsRequestCount = responder.ApsRequestCount + DeviceJoinScenario.ApsRequestsPerJoin;
        DeviceJoinScenario.SimulateJoin(responder, ieeeAddress, networkAddress, request.Vendor, request.Model);

        var completed = await WaitUntilAsync(
            () => responder.ApsRequestCount >= targetApsRequestCount,
            TimeSpan.FromSeconds(10)
        );

        return completed
            ? Results.Ok(new { networkAddress })
            : Results.Problem("Simulated device interview did not complete in time", statusCode: 504);
    }
);

app.Run();

static async Task<bool> WaitUntilAsync(Func<bool> condition, TimeSpan timeout)
{
    using var cts = new CancellationTokenSource(timeout);
    while (!condition() && !cts.IsCancellationRequested)
        await Task.Delay(20, CancellationToken.None);

    return condition();
}

internal record IndicationRequest(
    ushort SourceNwk,
    byte SourceEndpoint,
    ushort ProfileId,
    ushort ClusterId,
    string AsduHex
);

internal record JoinDeviceRequest(string Ieee, string Vendor, string Model);

// Exposes the top-level-statement entry point to WebApplicationFactory<Program> in tests.
public partial class Program { }
