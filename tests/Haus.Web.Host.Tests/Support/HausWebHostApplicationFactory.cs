using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Api.Client.Options;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Rooms;
using Haus.Mqtt.Client;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Haus.Web.Host.Tests.Support
{
    public class HausWebHostApplicationFactory : WebApplicationFactory<Startup>
    {
        private readonly FakeClock _clock;

        public HausWebHostApplicationFactory()
        {
            _clock = new FakeClock();
        }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IClock>(_clock);
                services.AddHausApiClient(opts => { opts.BaseUrl = Server.BaseAddress.ToString(); })
                    .RemoveAll(typeof(IHttpClientFactory))
                    .AddSingleton<IHttpClientFactory>(new FakeHttpClientFactory(CreateHttpClientWithAuth));

                services.AddAuthentication(opts =>
                    {
                        opts.DefaultAuthenticateScheme = TestingAuthenticationHandler.TestingScheme;
                        opts.DefaultChallengeScheme = TestingAuthenticationHandler.TestingScheme;
                        opts.DefaultScheme = TestingAuthenticationHandler.TestingScheme;
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestingAuthenticationHandler>(
                        TestingAuthenticationHandler.TestingScheme, opts => { });
                services.AddDbContext<HausDbContext>(opts => opts.UseInMemoryDatabase("HausDB"));
            });
        }

        public IHausApiClient CreateAuthenticatedClient()
        {
            return Services.GetRequiredService<IHausApiClient>();
        }

        public IHausApiClient CreateUnauthenticatedClient()
        {
            var factory = new FakeHttpClientFactory(CreateClient);
            var options = Services.GetRequiredService<IOptions<HausApiClientSettings>>();
            return new HausApiClientFactory(factory, options).Create();
        }

        public async Task<HubConnection> CreateHubConnection(string hub)
        {
            CreateClient();
            var connection = new HubConnectionBuilder()
                .WithUrl(
                    $"http://localhost/hubs/{hub}",
                    o =>
                    {
                        o.HttpMessageHandlerFactory = _ => Server.CreateHandler();
                        o.AccessTokenProvider = () => Task.FromResult(TestingAuthenticationHandler.TestingScheme);
                    })
                .Build();
            await connection.StartAsync();
            return connection;
        }

        public async Task<IHausMqttClient> GetMqttClient()
        {
            var creator = Services.GetRequiredService<IHausMqttClientFactory>();
            return await creator.CreateClient();
        }

        public async Task<(RoomModel, DeviceModel)> AddRoomWithDevice(
            string roomName,
            DeviceType deviceType)
        {
            var (room, devices) = await AddRoomWithDevices(roomName, deviceType);
            return (room, devices.Single());
        }
        
        public async Task<(RoomModel, DeviceModel[])> AddRoomWithDevices(
            string roomName,
            params DeviceType[] deviceTypes)
        {
            var discoverDeviceTasks = deviceTypes
                .Select(type => WaitForDeviceToBeDiscovered(type));
            
            var devices = await Task.WhenAll(discoverDeviceTasks);

            var apiClient = CreateAuthenticatedClient();
            var createResponse = await apiClient.CreateRoomAsync(new RoomModel {Name = roomName});
            var room = await createResponse.Content.ReadFromJsonAsync<RoomModel>();
            await apiClient.AddDevicesToRoomAsync(room.Id, devices.Select(d => d.Id).ToArray());

            return (room, devices);
        }

        public async Task SubscribeToHausCommandsAsync<T>(Action<HausCommand<T>> handler)
        {
            var mqttClient = await GetMqttClient();
            await mqttClient.SubscribeToHausCommandsAsync(handler);
        }

        public async Task SubscribeToHausEventsAsync<T>(Action<HausEvent<T>> handler)
        {
            var mqttClient = await GetMqttClient();
            await mqttClient.SubscribeToHausEventsAsync(handler);
        }

        public async Task PublishHausEventAsync<T>(IHausEventCreator<T> creator)
        {
            var client = await GetMqttClient();
            await client.PublishHausEventAsync(creator);
        }

        public async Task<DeviceModel> WaitForDeviceToBeDiscovered(
            DeviceType deviceType = DeviceType.Unknown,
            string externalId = null)
        {
            var actualId = string.IsNullOrWhiteSpace(externalId) ? $"{Guid.NewGuid()}" : externalId;
            await PublishHausEventAsync(new DeviceDiscoveredModel(actualId, deviceType));

            var apiClient = CreateAuthenticatedClient();
            return await WaitFor.ResultAsync(async () =>
            {
                var devices = await apiClient.GetDevicesAsync(actualId);
                return devices.Items.Single();
            });
        }

        public void SetClockTime(DateTime time)
        {
            _clock.SetNow(time);
        }

        private HttpClient CreateHttpClientWithAuth()
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestingAuthenticationHandler.TestingScheme);
            return client;
        }
    }
}