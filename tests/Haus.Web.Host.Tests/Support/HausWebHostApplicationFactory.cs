using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Api.Client.Options;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Storage.Commands;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Rooms;
using Haus.Core.Models.Rooms.Events;
using Haus.Cqrs;
using Haus.Mqtt.Client;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Haus.Web.Host.Tests.Support;

public class HausWebHostApplicationFactory : WebApplicationFactory<Startup>
{
    private readonly FakeClock _clock = new();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var hausBus = scope.GetService<IHausBus>();
        hausBus.ExecuteCommandAsync(new InitializeCommand()).Wait();
        hausBus.ExecuteCommandAsync(new ClearDatabaseCommand()).Wait();
        return host;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IClock>(_clock);
            services.AddHausApiClient(opts =>
            {
                opts.BaseUrl = Server.BaseAddress.ToString();
            });
            services
                .RemoveAll(typeof(IHttpClientFactory))
                .AddSingleton<IHttpClientFactory>(new FakeHttpClientFactory(CreateHttpClientWithAuth));

            services
                .AddAuthentication(opts =>
                {
                    opts.DefaultAuthenticateScheme = TestingAuthenticationHandler.TestingScheme;
                    opts.DefaultChallengeScheme = TestingAuthenticationHandler.TestingScheme;
                    opts.DefaultScheme = TestingAuthenticationHandler.TestingScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestingAuthenticationHandler>(
                    TestingAuthenticationHandler.TestingScheme,
                    _ => { }
                );
            services.Configure<HealthCheckPublisherOptions>(opts =>
            {
                opts.Delay = TimeSpan.Zero;
                opts.Period = TimeSpan.FromSeconds(1);
            });
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
            .AddJsonProtocol(opts =>
            {
                opts.PayloadSerializerOptions = HausJsonSerializer.DefaultOptions;
            })
            .WithUrl(
                $"http://localhost/hubs/{hub}",
                o =>
                {
                    o.HttpMessageHandlerFactory = _ => Server.CreateHandler();
                    o.AccessTokenProvider = () => Task.FromResult<string?>(TestingAuthenticationHandler.TestingScheme);
                }
            )
            .Build();
        await connection.StartAsync();
        return connection;
    }

    public async Task<IHausMqttClient> GetMqttClient()
    {
        var creator = Services.GetRequiredService<IHausMqttClientFactory>();
        return await creator.CreateClient();
    }

    public async Task<(RoomModel, DeviceModel)> AddRoomWithDevice(string roomName, DeviceType deviceType)
    {
        var (room, devices) = await AddRoomWithDevices(roomName, deviceType);
        return (room, devices.Single());
    }

    public async Task<(RoomModel, DeviceModel[])> AddRoomWithDevices(string roomName, params DeviceType[] deviceTypes)
    {
        var discoverDeviceTasks = deviceTypes.Select(type => WaitForDeviceToBeDiscovered(type));

        var devices = await Task.WhenAll(discoverDeviceTasks);

        var apiClient = CreateAuthenticatedClient();
        var createResponse = await apiClient.CreateRoomAsync(new RoomModel(Name: roomName));
        var room = await createResponse.Content.ReadFromJsonAsync<RoomModel>();
        if (room == null)
            throw new InvalidOperationException("failed to create room");

        await apiClient.AddDevicesToRoomAsync(room.Id, devices.Select(d => d.Id).ToArray());

        return (room, devices);
    }

    public async Task SubscribeToHausCommandsAsync<T>(string commandType, Action<HausCommand<T>> handler)
    {
        var mqttClient = await GetMqttClient();
        await mqttClient.SubscribeToHausCommandsAsync(commandType, handler);
    }

    public async Task SubscribeToHausEventsAsync<T>(string eventType, Action<HausEvent<T>> handler)
    {
        var mqttClient = await GetMqttClient();
        await mqttClient.SubscribeToHausEventsAsync(eventType, handler);
    }

    public async Task SubscribeToRoomLightingChangedCommandsAsync(Action<HausCommand<RoomLightingChangedEvent>> handler)
    {
        await SubscribeToHausCommandsAsync(RoomLightingChangedEvent.Type, handler);
    }

    public async Task PublishHausEventAsync<T>(IHausEventCreator<T> creator)
    {
        var client = await GetMqttClient();
        await client.PublishHausEventAsync(creator);
    }

    public async Task<DeviceModel> WaitForDeviceToBeDiscovered(
        DeviceType deviceType = DeviceType.Unknown,
        string? externalId = null
    )
    {
        var actualId = string.IsNullOrWhiteSpace(externalId) ? $"{Guid.NewGuid()}" : externalId;
        await PublishHausEventAsync(new DeviceDiscoveredEvent(actualId, deviceType));

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
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            TestingAuthenticationHandler.TestingScheme
        );
        return client;
    }
}
