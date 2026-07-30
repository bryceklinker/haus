using System;
using System.Linq;
using Haus.Zigbee.Simulator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

var tcpPort = builder.Configuration.GetValue<int?>("Simulator:TcpPort") ?? 4901;

builder.Services.AddSingleton<DeconzResponder>();
builder.Services.AddHostedService(provider => new TcpDongleListener(
    provider.GetRequiredService<DeconzResponder>(),
    provider.GetRequiredService<ILoggerFactory>(),
    tcpPort
));

var app = builder.Build();

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
        responder.EnqueueIndication(
            new IndicationBody(
                request.SourceNwk,
                request.SourceEndpoint,
                request.ProfileId,
                request.ClusterId,
                Convert.FromHexString(request.AsduHex)
            )
        );
        return Results.Accepted();
    }
);

app.Run();

internal record IndicationRequest(
    ushort SourceNwk,
    byte SourceEndpoint,
    ushort ProfileId,
    ushort ClusterId,
    string AsduHex
);
