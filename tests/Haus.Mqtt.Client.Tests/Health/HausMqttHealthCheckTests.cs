using System;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Mqtt.Client.Health;
using Haus.Mqtt.Client.Tests.Support;
using Haus.Testing.Support.Fakes;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Haus.Mqtt.Client.Tests.Health;

public class HausMqttHealthCheckTests
{
    private readonly FakeMqttClient _mqttClient;
    private readonly HausMqttHealthCHeck _healthCheck;

    public HausMqttHealthCheckTests()
    {
        var supportFactory = new SupportFactory();
        _mqttClient = supportFactory.FakeClient;

        var hausMqttClientFactory = supportFactory.CreateFactory();
        _healthCheck = new HausMqttHealthCHeck(hausMqttClientFactory);
    }

    [Fact]
    public async Task WhenConnectionToMqttWorksThenReturnsHealthy()
    {
        var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task WhenClientIsNotConnectedThenReturnsUnhealthy()
    {
        _mqttClient.ConnectException = new Exception();

        var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Unhealthy);
    }

    [Fact]
    public async Task WhenUnableToPingMqttThenReturnsUnhealthy()
    {
        _mqttClient.PingException = new Exception();

        var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Unhealthy);
    }
}