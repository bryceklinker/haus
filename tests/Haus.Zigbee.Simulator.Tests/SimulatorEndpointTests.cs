using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Haus.Zigbee.Simulator.Tests;

public class SimulatorEndpointTests : IDisposable
{
    // Each test gets its own factory/host rather than sharing one via IClassFixture: a shared
    // factory's derived WithWebHostBuilder instances raced on the same TestServer across tests.
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SimulatorEndpointTests()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration(
                (_, configuration) =>
                    configuration.AddInMemoryCollection(new Dictionary<string, string?> { ["Simulator:TcpPort"] = "0" })
            )
        );
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task PostIndications_AsduHexIsNotValidHex_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/indications",
            new
            {
                SourceNwk = 1,
                SourceEndpoint = 1,
                ProfileId = 0,
                ClusterId = 0,
                AsduHex = "not-hex",
            }
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostIndications_AsduHexIsValid_ReturnsAccepted()
    {
        var response = await _client.PostAsJsonAsync(
            "/indications",
            new
            {
                SourceNwk = 1,
                SourceEndpoint = 1,
                ProfileId = 0,
                ClusterId = 0,
                AsduHex = "0a0b",
            }
        );

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
    }

    [Fact]
    public async Task PostDevicesJoin_IeeeIsNotValid_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/devices/join",
            new
            {
                Ieee = "not-an-address",
                Vendor = "acme",
                Model = "widget",
            }
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetApsRequests_ReturnsOk()
    {
        var response = await _client.GetAsync("/aps-requests");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostParameter_SetsParameterValue()
    {
        var response = await _client.PostAsJsonAsync("/parameters/1", new byte[] { 0x01, 0x02 });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
