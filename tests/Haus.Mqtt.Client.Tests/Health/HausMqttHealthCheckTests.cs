using System;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Mqtt.Client.Health;
using Haus.Mqtt.Client.Settings;
using Haus.Testing.Support.Fakes;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MQTTnet.Diagnostics;
using Xunit;

namespace Haus.Mqtt.Client.Tests.Health
{
    public class HausMqttHealthCheckTests
    {
        private readonly FakeMqttClient _mqttClient;
        private readonly HausMqttHealthCHeck _healthCheck;
        
        public HausMqttHealthCheckTests()
        {
            var options = Options.Create(new HausMqttSettings
            {
                Server = "mqtt://localhost"
            });
            var fakeMqttClientFactory = new FakeMqttClientFactory();
            _mqttClient = fakeMqttClientFactory.Client;
            
            var hausMqttClientFactory = new HausMqttClientFactory(options, fakeMqttClientFactory, new MqttNetLogger());
            _healthCheck = new HausMqttHealthCHeck(hausMqttClientFactory);
        }

        [Fact]
        public async Task WhenConnectionToMqttWorksThenReturnsHealthy()
        {
            var result = await _healthCheck.CheckHealthAsync(null);

            result.Status.Should().Be(HealthStatus.Healthy);
        }

        [Fact]
        public async Task WhenClientIsNotConnectedThenReturnsUnhealthy()
        {
            _mqttClient.ConnectException = new Exception();
            
            var result = await _healthCheck.CheckHealthAsync(null);

            result.Status.Should().Be(HealthStatus.Unhealthy);
        }

        [Fact]
        public async Task WhenUnableToPingMqttThenReturnsUnhealthy()
        {
            _mqttClient.PingException = new Exception();

            var result = await _healthCheck.CheckHealthAsync(null);

            result.Status.Should().Be(HealthStatus.Unhealthy);
        }
    }
}