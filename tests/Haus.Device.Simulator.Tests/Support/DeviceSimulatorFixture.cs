using System.Threading.Tasks;
using Haus.Mqtt.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Device.Simulator.Tests.Support
{
    public class DeviceSimulatorWebApplication : WebApplicationFactory<Startup>
    {
        public Task<IHausMqttClient> GetMqttClientAsync()
        {
            var factory = Services.GetRequiredService<IHausMqttClientFactory>();
            return factory.CreateClient();
        }
        
        public async Task<HubConnection> CreateHubConnection(string hub)
        {
            CreateClient();
            var connection = new HubConnectionBuilder()
                .WithUrl($"http://localhost/hubs/{hub}", opts =>
                {
                    opts.HttpMessageHandlerFactory = _ => Server.CreateHandler();
                })
                .Build();
            await connection.StartAsync();
            return connection;
        }
    }
}